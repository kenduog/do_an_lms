using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Interface.UnitOfWork;
using Utilities;
using AutoMapper;
using Service.Services.DomainServices;
using Entities.Search;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static Utilities.CoreContants;
using System.Linq.Expressions;
using Entities.DomainEntities;

namespace Service.Services
{
    public class PaymentBankService : DomainService<tbl_PaymentBank, BaseSearch>, IPaymentBankService
    {
        public PaymentBankService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        protected override string GetStoreProcName()
        {
            return "Get_PaymentBank";
        }
    }
}
