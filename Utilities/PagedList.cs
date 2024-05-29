using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class PagedList<T> where T : class
    {
        public PagedList()
        {
        }
        public int pageIndex { set; get; }
        public int pageSize { set; get; }
        public int totalPage
        {
            get
            {
                decimal count = this.totalItem;
                if (count > 0)
                    return (int)Math.Ceiling(count / pageSize);
                else return 0;
            }
        }
        public int totalItem { set; get; }
        public IList<T> items { set; get; }
    }
}
