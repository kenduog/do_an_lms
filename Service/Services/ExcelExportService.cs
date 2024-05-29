using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Service.Services.DomainServices;
using Entities.Search;
using Newtonsoft.Json;
using Entities.DomainEntities;
using System.Runtime.CompilerServices;
using System.IO;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;
using static Utilities.CoreContants;
using System.Net.Http;
using Microsoft.CodeAnalysis.Operations;
using Entities.DataTransferObject;
using OfficeOpenXml.LoadFunctions.Params;
using ExcelDataReader;
using OfficeOpenXml.DataValidation;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Service.Services
{

    public class ExcelExportService : IExcelExportService
    {
        private readonly IAppDbContext appDbContext;
        private readonly IConfiguration configuration;
        private readonly string contentRoot = "";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExcelExportService(IAppDbContext appDbContext, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.appDbContext = appDbContext;
            this.configuration = configuration;
            this.contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            this._httpContextAccessor = httpContextAccessor;

        }


        private void Fetch<T>(Stream stream, ExcelPayload<T> payload)
        {
            if (string.IsNullOrEmpty(payload.templateName))
                throw new ArgumentNullException(nameof(payload.templateName));
            string template = Path.Combine(contentRoot, "Template", "Export", payload.templateName);
            using (var templateDocumentStream = File.OpenRead(template))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var xlPackage = new ExcelPackage(templateDocumentStream))
                {


                    var sheet = xlPackage.Workbook.Worksheets[0];
                    var rowIndex = payload.fromRow;
                    var column = payload.fromCol;
                    int index = 1;
                    foreach (var model in payload.items)
                    {
                        // Check if the column is within a valid range
                        if (column < 1 || column > sheet.Dimension.Columns)
                            break;
                        sheet.Cells[rowIndex, column].Value = index++;
                        column++;
                        foreach (PropertyInfo item in model.GetType().GetProperties())
                        {
                            if (column < 1 || column > sheet.Dimension.Columns)
                                break;
                            var value = item.GetValue(model);
                            sheet.Cells[rowIndex, column].Value = value;
                            column++;
                        }
                        rowIndex++;
                        column = 1;
                    }
                    //binding other value
                    if (payload.keyValues != null && payload.keyValues.Count > 0)
                    {
                        foreach (var item in payload.keyValues)
                        {
                            sheet.Cells[item.Key.row, item.Key.col].Value = item.Value;
                        }
                    }

                    xlPackage.SaveAs(stream);

                }
            }
        }

        public string Export<T>(ExcelPayload<T> payload)
        {
            byte[] bytes;
            var contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            using (var stream = new MemoryStream())
            {
                Fetch(stream, payload);
                bytes = stream.ToArray();
            }
            string nameFileExport = $"{Guid.NewGuid()}.xlsx";
            string downloadFolder = Path.Combine(contentRoot, CoreContants.DOWNLOAD_FOLDER_NAME, payload.folderToSave);
            string exportFilePath = Path.Combine(downloadFolder, nameFileExport);

            // Save the file
            File.WriteAllBytes(exportFilePath, bytes);

            // Get the base URL dynamically
            var request = _httpContextAccessor.HttpContext.Request;

            string relativePath = Path.Combine(payload.folderToSave, nameFileExport).Replace("\\", "/");
            string downloadUrl = relativePath;

            return downloadUrl;
        }

        public Dictionary<int, string> InitTokenHeader(List<string> headers, int from)
        {
            if (headers == null || headers.Count == 0)
                return null;

            var result = headers
            .Select((value, index) => new { Index = index + from, Value = value })
            .ToDictionary(pair => pair.Index, pair => pair.Value);

            return result;
        }
        public List<T> FetchDataToModel<T>(IFormFile file, Dictionary<int, string> tokens, int fromRow) where T : new()
        {
            if (file == null)
                throw new AppException(MessageContants.nf_file);

            var result = new List<T>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            IExcelDataReader reader;
            Stream FileStream = file.OpenReadStream();
            DataSet dsexcelRecords = new DataSet();
            if (!file.FileName.EndsWith(".xls") && !file.FileName.EndsWith(".xlsx"))
                throw new AppException(MessageContants.invalid_file);

            reader = ExcelReaderFactory.CreateReader(FileStream);

            dsexcelRecords = reader.AsDataSet();
            reader.Close();
            FileStream.Close();

            if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
            {
                DataTable dtStudentRecords = dsexcelRecords.Tables[0];
                int rowCount = dtStudentRecords.Rows.Count;
                int colCount = dtStudentRecords.Columns.Count;
                for (int i = fromRow; i < rowCount; i++)
                {
                    T item = new T();
                    for (int j = 0; j < colCount; j++)
                    {

                        // Thêm dữ liệu tùy vào header
                        string header = "";
                        if (!tokens.TryGetValue(j, out header))
                            continue;

                        var propInfo = item.GetType().GetProperty(header);

                        if (propInfo != null)
                        {
                            var value = dtStudentRecords.Rows[i][j];
                            if (value != null && value.ToString() == "NULL")
                                value = DBNull.Value;

                            if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                if (value != DBNull.Value)
                                {
                                    var underlyingType = Nullable.GetUnderlyingType(propInfo.PropertyType);
                                    propInfo.SetValue(item, Convert.ChangeType(value, underlyingType), null);
                                }
                                else
                                {
                                    propInfo.SetValue(item, null, null);
                                }
                            }
                            else
                            {
                                propInfo.SetValue(item, Convert.ChangeType(value, propInfo.PropertyType), null);
                            }
                        }
                    }
                    result.Add(item);
                }
            }
            return result;
        }
        private void FetchTemplate<T>(Stream stream, ExcelPayload<T> payload, List<ListDropDown> dropdown)
        {
            if (string.IsNullOrEmpty(payload.templateName))
                throw new ArgumentNullException(nameof(payload.templateName));
            string template = Path.Combine(contentRoot, "Template", ExcelConstant.Import, payload.templateName);
            using (var templateDocumentStream = File.OpenRead(template))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var xlPackage = new ExcelPackage(templateDocumentStream))
                {
                    var sheet = xlPackage.Workbook.Worksheets[0];
                    if (dropdown.Any())
                    {
                        foreach (var dropdownItem in dropdown)
                        {
                            string colName = "ValidationList" + dropdownItem.columnName;
                            // Tạo danh sách giá trị cho dropdown cho cột
                            var validationList = sheet.Workbook.Worksheets.Add(colName);
                            for (int i = 0; i < dropdownItem.dataDropDown.Count; i++)
                            {
                                validationList.Cells[i + 1, 1].Value = dropdownItem.dataDropDown[i].name;
                            }
                            var range = validationList.Cells["A1:A" + dropdownItem.dataDropDown.Count];
                            var validation = sheet.DataValidations.AddListValidation($"{dropdownItem.columnName}:{dropdownItem.columnName}");
                            validation.ShowErrorMessage = true;
                            validation.ErrorStyle = ExcelDataValidationWarningStyle.stop;
                            validation.ErrorTitle = "Invalid Value";
                            validation.Error = "Please select a valid value from the list.";
                            validation.Formula.ExcelFormula = $"='{colName}'!$A$1:$A${dropdownItem.dataDropDown.Count}";
                            validationList.Hidden = eWorkSheetHidden.Hidden;
                        }
                    }
                    var rowIndex = payload.fromRow;
                    var column = payload.fromCol;
                    int index = 1;
                    foreach (var model in payload.items)
                    {
                        // Check if the column is within a valid range
                        if (column < 1 || column > sheet.Dimension.Columns)
                            break;
                        sheet.Cells[rowIndex, column].Value = index++;
                        column++;
                        foreach (PropertyInfo item in model.GetType().GetProperties())
                        {
                            if (column < 1 || column > sheet.Dimension.Columns)
                                break;
                            var value = item.GetValue(model);
                            sheet.Cells[rowIndex, column].Value = value;
                            column++;
                        }
                        rowIndex++;
                        column = 1;
                    }
                    //binding other value
                    if (payload.keyValues != null && payload.keyValues.Count > 0)
                    {
                        foreach (var item in payload.keyValues)
                        {
                            sheet.Cells[item.Key.row, item.Key.col].Value = item.Value;
                        }
                    }

                    xlPackage.SaveAs(stream);

                }
            }
        }
        public string ExportTemplate<T>(ExcelPayload<T> payload, List<ListDropDown> dropdown)
        {
            byte[] bytes;
            var contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            using (var stream = new MemoryStream())
            {
                FetchTemplate(stream, payload, dropdown);
                bytes = stream.ToArray();
            }
            string nameFileExport = $"{Guid.NewGuid()}.xlsx";
            string downloadFolder = Path.Combine(contentRoot, CoreContants.DOWNLOAD_FOLDER_NAME, payload.folderToSave);
            string exportFilePath = Path.Combine(downloadFolder, nameFileExport);

            // Save the file
            File.WriteAllBytes(exportFilePath, bytes);

            // Get the base URL dynamically
            var request = _httpContextAccessor.HttpContext.Request;

            string relativePath = Path.Combine(payload.folderToSave, nameFileExport).Replace("\\", "/");
            string downloadUrl = relativePath;

            return downloadUrl;
        }
    }
}
