using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Utilities.CoreContants;

namespace Utilities
{
    public static class ExcelConstant
    {
        public const string Import = "Import";
        public const string Export = "Export";

        #region Export
        public const string Export_ScaleMeasure = "Export_ScaleMeasure.xlsx";
        public const string Export_ValueEntry = "Export_ValueEntry.xlsx";
        public const string Export_Inventory = "Export_Inventory.xlsx";
        public const string Export_Staff = "Export_Staff.xlsx";
        public const string Export_Teacher = "Export_Teacher.xlsx";
        public const string Export_Student = "Export_Student.xlsx";
        #endregion

        #region Import
        public const string Import_Staff = "Import_Staff.xlsx";
        public const string Import_Teacher = "Import_Teacher.xlsx";
        public const string Import_Student = "Import_Student.xlsx";
        public const string Import_Student_In_Bill = "Import_Student_In_Bill.xlsx";
        #endregion
    }
}
