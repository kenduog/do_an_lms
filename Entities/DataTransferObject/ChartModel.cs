using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObject
{
    public class ColumChart
    {
        public List<KeyValuePair> data { get; set; }
    }
    public class PieChart
    {
        public List<KeyValuePair> data { get; set; }
    }

    public class KeyValuePair
    {
        public string key { get; set; }
        public double? value { get; set; }
    }
}
