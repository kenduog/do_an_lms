﻿using Entities;
using Newtonsoft.Json;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static Utilities.CoreContants;

namespace Request.RequestUpdate
{
    public class StudentUpdate : DomainUpdate
    {
        public string firstName { get; set; }
        public string thumbnail { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        /// <summary>
        /// Biệt danh
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// Loại
        /// 1 - Đúng tuyến
        /// 2 - Trái tuyến
        /// </summary>
        public int? type { get; set; }
        [JsonIgnore]
        public string typeName { get { return type == 1 ? "Đúng tuyến" : type == 2 ? "Trái tuyến" : ""; } }
        /// <summary>
        /// 1 - Nam 
        /// 2 - Nữ 
        /// 3 - Khác
        /// </summary>
        public int? gender { get; set; }
        public string genderName { get { return GetGenderName(gender ?? 0); } }
        /// <summary>
        /// Dân tộc
        /// </summary>
        public string ethnicity { get; set; }
        /// <summary>
        /// Ngày sinh
        /// </summary>
        public double? birthday { get; set; }
        /// <summary>
        /// Nơi sinh
        /// </summary>
        public string placeOfBirth { get; set; }
        /// <summary>
        /// Địa chỉ hiện tại
        /// </summary>
        public string address { get; set; }
        /// <summary>
        /// Quê quán
        /// </summary>
        public string hometown { get; set; }
        /// <summary>
        /// Hình thức
        /// 1 - Theo buổi
        /// 2 - Bán trú
        /// </summary>
        public int? method { get; set; }
        /// <summary>
        /// Ngày nhập học
        /// </summary>
        public double? enrollmentDate { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// Ngày bắt đầu học
        /// </summary>
        public double? startLearningDate { get; set; }
        /// <summary>
        /// Tình trạng học sinh
        /// 1 - Mới
        /// 2 - Đang học
        /// 3 - Bảo lưu
        /// 4 - Đã học xong
        /// 5 - Bỏ học
        /// </summary>
        public int? status { get; set; }
        public string statusName { get { return tbl_Student.GetStatusName(status ?? 0); } }
        public Guid? branchId { get; set; }
    }
}
