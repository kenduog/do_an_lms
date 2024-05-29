using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.DomainEntities
{
    public interface IBaseSearch
    {
        int pageIndex { set; get; }
        int pageSize { set; get; }
        string searchContent { set; get; }
        int orderBy { set; get; }
    }
    public interface IBaseOptionSearch
    {
        Guid? branchId { set; get; }
        Guid? schoolYearId { set; get; }
    }

    public class BaseSearch : IBaseSearch
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        [DefaultValue(1)]
        public int pageIndex { set; get; } = 1;

        /// <summary>
        /// Số lượng item trên 1 trang
        /// </summary>
        [DefaultValue(20)]
        public int pageSize { set; get; } = 20;

        /// <summary>
        /// Nội dung tìm kiếm chung
        /// </summary>
        [StringLength(1000, ErrorMessage = "Nội dung không vượt quá 1000 kí tự")]
        public virtual string searchContent { set; get; }

        /// <summary>
        /// Không dùng
        /// </summary>
        [DefaultValue(0)]
        public virtual int orderBy { set; get; } = 0;
    }

    public class OptionSearch : IBaseOptionSearch
    {
        public Guid? branchId { set; get; }
        public Guid? schoolYearId { set; get; }
        
    }
}
