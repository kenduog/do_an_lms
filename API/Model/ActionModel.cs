using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Collections.Generic;

namespace API.Model
{
    public class ActionModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool grant { get; set; }
    }
}
