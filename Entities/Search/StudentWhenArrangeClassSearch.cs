using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class StudentWhenArrangeClassSearch
	{
		public int pageIndex { get; set; } = 1;
		public int pageSize { get; set; } = 20;
		public string searchContent { get; set; }
		public double fromBirthday { get; set; }
		public double toBirthday { get; set; }
		public int method { get; set; }
	 	public Guid? gradeId { get; set; }
		public Guid? branchId { get; set; }
		public Guid? schoolYearId { get; set; }
		public int gender { get; set; }
		public int status { get; set; }
    }
}
