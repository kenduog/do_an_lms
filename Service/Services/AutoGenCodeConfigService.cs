using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class AutoGenCodeConfigService : IAutoGenCodeConfigService
    {
        private readonly IAppUnitOfWork unitOfWork;

        public AutoGenCodeConfigService(IAppUnitOfWork unitOfWork)
        {
            this.unitOfWork= unitOfWork;
        }

        public async Task<string> AutoGenCode(string tableName, string prefix = "")
        {
            var config = await this.unitOfWork.Repository<tbl_AutoGenCodeConfig>().GetQueryable()
                .FirstOrDefaultAsync(d => d.tableName == tableName && (string.IsNullOrEmpty(prefix) || d.prefix == prefix));
            if (config == null) return string.Empty;
            var now = DateTime.Today;
            string newCode = string.Empty;
            newCode = $"{config.prefix}-";

            if (config.isDay.HasValue && config.isDay.Value)
                newCode += now.Day.ToString("00");
            if (config.isMonth.HasValue && config.isMonth.Value)
                newCode += now.Month.ToString("00");
            if (config.isYear.HasValue && config.isYear.Value)
                newCode += $"{now:yy}-";
            newCode += (config.currentCode + 1).ToString().PadLeft(config.autoNumberLength ?? 0, '0');

            //update config
            config.currentCode++;
            this.unitOfWork.Repository<tbl_AutoGenCodeConfig>().Update(config);
            return newCode;
        }
    }
}
