using Entities;
using Extensions;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Service.Services.DomainServices;
using Entities.Search;
using AppDbContext.Migrations;
using Entities.DomainEntities;
using ExcelDataReader.Log;
using System.Net;

namespace Service.Services
{
    public class LogService : DomainService<sys_Log, BaseSearch>, ILogService
    {
        public LogService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_LogDayBoard";
        }

        public async Task InsertLog(Exception exception)
        {
            await this.unitOfWork.Repository<sys_Log>().CreateAsync(
                new sys_Log(exception)
            );
            await this.unitOfWork.SaveAsync();
        }

        public async Task InsertLog(sys_Log log)
        {
            await this.unitOfWork.Repository<sys_Log>().CreateAsync(log);
            await this.unitOfWork.SaveAsync();
        }
    }
}
