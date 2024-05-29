﻿using Microsoft.AspNetCore.Http;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Request.RequestCreate
{
    public class GrantPermissionsGroupNewsCreate : DomainCreate
    {
        [Required(ErrorMessage = MessageContants.req_user)]
        public string userIds { get; set; }
        [Required(ErrorMessage = MessageContants.req_groupNews)]
        public Guid groupNewsId { get; set; }
    }
}
