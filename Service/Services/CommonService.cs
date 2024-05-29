using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Service.Services.DomainServices;
using Entities.Search;
using Newtonsoft.Json;
using Entities.DomainEntities;
using System.Net.Mail;
using Request.DomainRequests;
using Microsoft.Extensions.Configuration;
using static Utilities.CoreContants;

namespace Service.Services
{
    public class CommonService :  ICommonService
    {
        private IAppDbContext appDbContext;
        public CommonService(IAppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }
        
        public async Task<string> GenerateStudentCode()
        {
            int count = await this.appDbContext.Set<tbl_Student>().CountAsync() + 1;
            string code = $"{STUDENT_CODE}000000";
            return $"{code.Remove(code.Length - count.ToString().Length)}{count}";
        }
    }
}
