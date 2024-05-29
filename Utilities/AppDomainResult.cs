using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Utilities
{
    public class AppDomainResult
    {
        public AppDomainResult()
        {
            //Messages = new List<string>();
        }
        public AppDomainResult(object data)
        {
            this.data = data;
        }
        public bool success { get; set; } = true;
        public object data { get; set; }
        public int resultCode { get; set; } = StatusCodes.Status200OK;
        public string resultMessage { get; set; } = MessageContants.success;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
