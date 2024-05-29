using Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;

namespace Entities.DomainEntities
{
    public class DomainEntities
    {
        public DomainEntities()
        {
        }

        /// <summary>
        /// Tổng số item phân trang
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public int totalItem { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        /// <summary>
        /// Khóa chính
        /// </summary>
        public Guid id { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public double? created { get; set; }

        /// <summary>
        /// Tạo bởi
        /// </summary>
        public Guid? createdBy { get; set; }

        /// <summary>
        /// Ngày cập nhật
        /// </summary>
        public double? updated { get; set; }

        /// <summary>
        /// Người cập nhật
        /// </summary>
        public Guid? updatedBy { get; set; }

        /// <summary>
        /// Cờ xóa
        /// </summary>
        [JsonIgnore]
        public bool? deleted { get; set; } = false;

        /// <summary>
        /// Cờ active
        /// </summary>
        public bool? active { get; set; } = true;

    }
}
