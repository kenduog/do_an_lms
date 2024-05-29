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
using System.Net;
using Request.RequestCreate;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Interface.Services.DomainServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Service.Services
{
    public class WeekService : DomainService<tbl_Week, WeekSearch>, IWeekService
    {
        private readonly ISemesterService semesterService;
        public WeekService(IServiceProvider serviceProvider, IAppUnitOfWork unitOfWork,
            IMapper mapper
            ) : base(unitOfWork, mapper)
        {
            this.semesterService = serviceProvider.GetRequiredService<ISemesterService>();
        }
        protected override string GetStoreProcName()
        {
            return "Get_Week";
        }
        public async Task<AppDomainResult> GenerateWeek(GenerateWeekCreate generate, int number)
        {
            try
            {
                await this.Validate(new tbl_Week { schoolYearId = generate.schoolYearId, semesterId = generate.semesterId });
                AppDomainResult result = new AppDomainResult();
                List<tbl_Week> weeks = new List<tbl_Week>();
                DateTime now = DateTime.Now;
                // Thời học kỳ bắt đầu và kết thúc
                var semester = await this.semesterService.GetByIdAsync(generate.semesterId.Value);
                DateTime sSemesterTime = DateTimeOffset.FromUnixTimeMilliseconds((long)semester.sTime).UtcDateTime;
                DateTime eSemesterTime = DateTimeOffset.FromUnixTimeMilliseconds((long)semester.eTime).UtcDateTime;
                int numberWeek = number + 1;
                int totalDayOfWeek = 6;
                // Tạo tuần theo học kỳ
                for (DateTime i = sSemesterTime; i <= eSemesterTime; i = i.AddDays(1))
                {
                    int dayWeek = (int)i.DayOfWeek;
                    int totalAddDay = totalDayOfWeek - dayWeek;
                    if (eSemesterTime.Date <= i.AddDays(totalAddDay).Date)
                    {
                        totalAddDay = (eSemesterTime.Date - i.Date).Days;
                    }
                    var week = new tbl_Week()
                    {
                        name = "Tuần " + numberWeek,
                        schoolYearId = generate.schoolYearId,
                        weekNumber = numberWeek,
                        semesterId = generate.semesterId,
                        sTime = new DateTimeOffset(i).ToUnixTimeMilliseconds(),
                        eTime = new DateTimeOffset(i.AddDays(totalAddDay).AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999)).ToUnixTimeMilliseconds(),
                    };
                    i = i.AddDays(totalAddDay);
                    numberWeek++;
                    weeks.Add(week);
                }
                await this.unitOfWork.Repository<tbl_Week>().CreateAsync(weeks);
                await this.unitOfWork.SaveAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new AppException("Lỗi trong quá trình xử lý");
            }
        }
        public async Task<bool> AddWeek(tbl_Week itemModel)
        {
            try
            {
                await this.unitOfWork.Repository<tbl_Week>().CreateAsync(itemModel);
                await this.unitOfWork.SaveAsync();
                return true;

            }
            catch (Exception ex)
            {
                throw new AppException("Lỗi trong quá trình xử lý");
            }
        }
        public async Task<tbl_Week> UpdateWeek(tbl_Week itemModel)
        {
            try
            {
                var update = await this.unitOfWork.Repository<tbl_Week>().GetQueryable().FirstOrDefaultAsync(x=>x.id == itemModel.id);
                if (update != null)
                {
                    update.name = itemModel.name;
                    update.sTime = itemModel.sTime;
                    update.eTime = itemModel.eTime;
                    update.updatedBy = LoginContext.Instance.CurrentUser == null ? itemModel.updatedBy : LoginContext.Instance.CurrentUser.userId;
                    update.updated = Timestamp.Now();
                }
                this.unitOfWork.Repository<tbl_Week>().Update(update);
                await this.unitOfWork.SaveAsync();
                return itemModel;

            }
            catch (Exception ex)
            {
                throw new AppException("Lỗi trong quá trình xử lý");
            }
        }
        public override async Task DeleteItem(Guid id)
        {
            await this.unitOfWork.SaveAsync();
            await DeleteAsync(id);
            Thread clearWeek = new Thread(() =>
            BackgroundService.ClearWeek(id));
            clearWeek.Start();
        }
    }
}
