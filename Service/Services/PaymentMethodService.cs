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
using System.Threading;
using AppDbContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Internal;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Service.Services
{
    public class PaymentMethodService : DomainService<tbl_PaymentMethod, PaymentMethodSearch>, IPaymentMethodService
    {
        public PaymentMethodService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_PaymentMethod";
        }

        public override async Task Validate(tbl_PaymentMethod model)
        {
            if (model.branchId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_Branch>().Validate(model.branchId.Value) ?? throw new AppException(MessageContants.nf_branch);
            }

            if (model.paymentBankId.HasValue)
            {
                var item = await this.unitOfWork.Repository<tbl_PaymentBank>().Validate(model.paymentBankId.Value) ?? throw new AppException(MessageContants.nf_paymentBank);
            }
        }

        public override async Task<tbl_PaymentMethod> GetByIdAsync(Guid id)
        {
            var data = await Queryable.Where(e => e.id == id).AsNoTracking().FirstOrDefaultAsync();
            if (data != null)
                data.branchName = (await unitOfWork.Repository<tbl_Branch>().GetQueryable()
                    .FirstOrDefaultAsync(x => x.id == data.branchId))?.name;
            return data;
        }
    }
}
