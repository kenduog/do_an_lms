using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Model
{
    public class MemberGroupNewsDTO
    {
        public Guid? userId { get; set; }
        public string userCode { get; set; }

        public string username { get; set; }

        public string fullName { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string phone { get; set; }

        public string email { get; set; }


        /// <summary>
        /// Trạng thái
        /// </summary>
        public int? status { get; set; }
        public string statusName { get; set; }
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public double? birthday { get; set; }

        /// <summary>
        /// 1 - Nam 
        /// 2 - Nữ 
        /// 3 - Khác
        /// </summary>
        public int? gender { get; set; }
        public string genderName { get; set; }
        public bool? isSuperUser { get; set; }
        public string thumbnail { get; set; }
        public int? typeUserGroup { get; set; }
        public string typeUserGroupName { get; set; }
        public string groupName { get; set; }

    }
}
