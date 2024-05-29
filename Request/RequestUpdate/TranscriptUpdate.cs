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
    public class TranscriptUpdate : DomainUpdate
    {
        public int? score { get; set; }
        /// <summary>
        /// 1: thi giữa kỳ
        /// 2: thi học kỳ
        /// </summary>
        public string note { get; set; }
    }
}
