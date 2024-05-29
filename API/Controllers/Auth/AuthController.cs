using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers.Auth
{
    [Route("api/v{version:apiVersion}/Authenticate")]
    [ApiController]
    [Description("Authenticate")]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthController : BaseAPI.Controllers.Auth.AuthController
    {
        public AuthController(IServiceProvider serviceProvider, IConfiguration configuration, IMapper mapper, ILogger<AuthController> logger) : base(serviceProvider, configuration, mapper, logger)
        {
        }
    }
}