using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObject
{
	public class AdminDTO : DomainEntities.DomainEntities
	{
		public string code { get; set; }
		public string username { get; set; }
		public string fullName { get; set; }
		public string firstName { get; set; }
		public string lastName { get; set; }
		public string phone { get; set; }
		public string email { get; set; }
		public string address { get; set; }
		public int status { get; set; }
		public string statusName { get; set; }
		public double? birthday { get; set; }
		public int gender { get; set; }
		public string thumbnail { get; set; }
		public Guid? cityId { get; set; }
		public Guid? districtId { get; set; }
		public Guid? wardId { get; set; }
		public string hashQRCode { get; set; }
		public string genderName { get; set; }
		public string thumbnailResize { get; set; }
    }
}
