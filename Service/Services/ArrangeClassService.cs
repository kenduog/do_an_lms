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
using Entities.DataTransferObject;
using Entities.AuthEntities;

namespace Service.Services
{
    public class ArrangeClassService : IArrangeClassService
    {
        private readonly IAppUnitOfWork unitOfWork;
        protected IAppDbContext coreDbContext;
        public ArrangeClassService(IAppUnitOfWork unitOfWork, IAppDbContext coreDbContext) 
        {
            this.unitOfWork = unitOfWork;
            this.coreDbContext = coreDbContext;
        }

        public async Task<(int, List<StudentAvailableDTO>)> GetStudentWhenArrangeClass(StudentWhenArrangeClassSearch baseSearch)
        {
            var userId = LoginContext.Instance.CurrentUser.userId;
            string myBranchIds = "";
            var user = await unitOfWork.Repository<tbl_Users>()
                .GetQueryable().FirstOrDefaultAsync(x => x.id == userId);
            var adminGroup = await unitOfWork.Repository<tbl_Group>()
                .GetQueryable().FirstOrDefaultAsync(x => x.code == "QTV" && x.deleted == false);
            var hasGroup = await unitOfWork.Repository<tbl_UserGroup>()
                .GetQueryable().AnyAsync(x => x.userId == userId && x.groupId == adminGroup.id && x.deleted == false);
            if(!hasGroup && user.isSuperUser == false)
                myBranchIds = user.branchIds;
            return await Task.Run(() =>
            {
                var result = (0, new List<StudentAvailableDTO>());
                DataTable dataTable = new DataTable();
                Microsoft.Data.SqlClient.SqlConnection connection = null;
                Microsoft.Data.SqlClient.SqlCommand command = null;
                try
                {
                    connection = (Microsoft.Data.SqlClient.SqlConnection)coreDbContext.Database.GetDbConnection();
                    command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = $"Get_Student_WhenArrangeClass";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pageIndex", baseSearch.pageIndex);
                    command.Parameters.AddWithValue("@pageSize", baseSearch.pageSize);
                    command.Parameters.AddWithValue("@searchContent", baseSearch.searchContent);
                    command.Parameters.AddWithValue("@fromBirthday", baseSearch.fromBirthday);
                    command.Parameters.AddWithValue("@toBirthday", baseSearch.toBirthday);
                    command.Parameters.AddWithValue("@method", baseSearch.method);
                    command.Parameters.AddWithValue("@gradeId", baseSearch.gradeId);
                    command.Parameters.AddWithValue("@branchId", baseSearch.branchId);
                    command.Parameters.AddWithValue("@gender", baseSearch.gender);
                    command.Parameters.AddWithValue("@status", baseSearch.status);
                    command.Parameters.AddWithValue("@myBranchIds", myBranchIds);
                    Microsoft.Data.SqlClient.SqlDataAdapter sqlDataAdapter = new Microsoft.Data.SqlClient.SqlDataAdapter(command);
                    sqlDataAdapter.Fill(dataTable);
                    var admins = ConvertToList(dataTable);
                    int totalItem = 0;
                    if (admins.Any())
                        totalItem = admins[0].totalItem;
                    result = (totalItem, admins);
                }
                finally
                {
                    if (connection != null && connection.State == System.Data.ConnectionState.Open)
                        connection.Close();

                    if (command != null)
                        command.Dispose();
                }
                return result;
            });
        }
        public static List<StudentAvailableDTO> ConvertToList(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(StudentAvailableDTO).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<StudentAvailableDTO>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch
                        {
                        }
                    }
                }
                return objT;
            }).ToList();
        }

    }
}
