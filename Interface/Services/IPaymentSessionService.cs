using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IPaymentSessionService : IDomainService<tbl_PaymentSession, PaymentSessionSearch>
    {
        Task<string> GetPrintContent(tbl_PaymentSession paymentSession, tbl_Bill bill);
    }
}
