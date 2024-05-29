using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestUpdate
{
    public class TeachingFrameUpdate : DomainUpdate
    {
        /// <summary>
        /// Class per semester 
        /// </summary>
        public int? cps { get; set; }
        /// <summary>
        /// Class per week
        /// </summary>
        public int? cpw { get; set; }
    }
}
