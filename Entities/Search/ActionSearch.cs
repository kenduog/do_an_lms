using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Entities.Search
{
    public class ActionSearch : BaseSearch
    {
        public string code { get; set; }
        public string controller { get; set; }
    }
}
