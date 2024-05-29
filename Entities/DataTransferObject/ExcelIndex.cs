using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObject
{
    public class ExcelIndex
    {
        public ExcelIndex(int row, int col)
        {
            this.col = col;
            this.row = row;
        }

        public int col { get; set; }
        public int row { get; set; }
    }
}
