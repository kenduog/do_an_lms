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
using Microsoft.AspNetCore.DataProtection.XmlEncryption;

namespace Service.Services
{
    public class MultipleMessageService : IMultipleMessageService
    {
        private readonly IAppDbContext appDbContext;
        public MultipleMessageService(IAppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<sys_MultipleMessage> GetItemByCode(string code)
        {
            return await this.appDbContext.Set<sys_MultipleMessage>().FirstOrDefaultAsync(x => x.code == code);
        }

        public async Task<string> GetMessage(string code, int? language = 2)
        {
            var userLog = LoginContext.Instance.CurrentUser;
            if (userLog != null)
                language = userLog.language;
            string message = $"{code}";
            var record = await this.appDbContext.Set<sys_MultipleMessage>().FirstOrDefaultAsync(x => x.code == code);
            if (record == null)
                return code;
            return language == 1 ? record.en : record.vi;
        }
    }
}
