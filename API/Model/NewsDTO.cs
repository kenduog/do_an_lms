

using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace API.Model
{
    public class NewsDTO : Entities.DomainEntities.DomainEntities
    {
        public Guid? groupNewsId { get; set; }
        /// <summary>
        /// Tiêu đề
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Nội dung
        /// </summary>
        public string content { get; set; }

        public bool? pinned { get; set; }
        public int? pinnedPosition { get; set; }
        /// <summary>
        /// Chi nhánh
        /// </summary>
        public string branchIds { get; set; }
        public Guid? userId { get; set; }
        public string groupName { get; set; }
        public string fullName { get; set; }
        public string userThumbnail { get; set; }
        /// <summary>
        /// Người dùng đã yêu thích
        /// </summary>
        public bool liked { get; set; }
        public int? countLike { get; set; }
        public int? countComment { get; set; }
        public List<BranchInNewsDTO> branch { get; set; }
        public List<DocumentNewsDTO> documentNews { get; set; }
    }
    public class BranchInNewsDTO
    {
        public Guid? id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
    }
    public class DocumentNewsDTO
    { 
        public string link { get; set; }
        public string typeCode { get; set; }
    }
}
