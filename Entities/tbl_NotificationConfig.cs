using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class tbl_NotificationConfig : DomainEntities.DomainEntities
    {
        public string code { get; set; }
        /// <summary>
        /// Tên cài đặt
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Title noti 
        /// </summary>
        public string notiTitle { get; set; }

        /// <summary>
        /// Nội dung thông báo cho phụ huynh
        /// </summary>
        public string notiContentForParent { get; set; }

        /// <summary>
        /// Nội dung thông báo cho giáo viên
        /// </summary>
        public string notiContentForTeacher { get; set; }

        /// <summary>
        /// Nội dung thông báo cho tất cả
        /// </summary>
        public string notiContentForAll { get; set; }

        /// <summary>
        /// Tên file template email phụ huynh
        /// </summary>
        public string emailTemplateFileParent { get; set; }

        /// <summary>
        /// Tên file template email giáo viên
        /// </summary>
        public string emailTemplateFileTeacher { get; set; }

        /// <summary>
        /// Tên file template email tất cả
        /// </summary>
        public string emailTemplateFileAll { get; set; }

        /// <summary>
        /// Gửi thông báo cho phụ huynh
        /// </summary>
        public bool isSendNotiParent { get; set; }
        /// <summary>
        /// Gửi thông báo cho giáo viên
        /// </summary>
        public bool isSendNotiTeacher { get; set; }

        /// <summary>
        /// Gửi thông báo cho tất cả
        /// </summary>
        public bool isSendNotiAll { get; set; }

        /// <summary>
        /// Gửi email cho phụ huynh
        /// </summary>
        public bool isSendEmailParent { get; set; }

        /// <summary>
        /// Gửi email cho giáo viên
        /// </summary>
        public bool isSendEmailTeacher { get; set; }


        /// <summary>
        /// Gửi email cho tất cả
        /// </summary>
        public bool isSendEmailAll { get; set; }

        /// <summary>
        /// Link chi tiết của thông báo phụ huynh
        /// </summary>
        public string linkForParent { get; set; }

        /// <summary>
        /// Link chi tiết của thông báo giáo viên
        /// </summary>
        public string linkForTeacher { get; set; }

        /// <summary>
        /// Link chi tiết của thông báo tất cả
        /// </summary>
        public string linkForAll { get; set; }

        /// <summary>
        /// Link chi tiết của thông báo trên app mobile phụ huynh
        /// </summary>
        public string deepLinkForParent { get; set; }

        /// <summary>
        /// Link chi tiết của thông báo trên app mobile giáo viên
        /// </summary>
        public string deepLinkForTeacher { get; set; }

        /// <summary>
        /// Link chi tiết của thông báo trên app mobile tất cả
        /// </summary>
        public string deepLinkForAll { get; set; }

        public string screen { get; set; }
        public string param { get; set; }
    }
}
