using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Microsoft.AspNetCore.Http;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface IBillService : IDomainService<tbl_Bill, BillSearch>
    {
        Task Payment(PaymentsRequest itemModel);
        Task MobilePayment(PaymentsRequest itemModel);
        Task<PagedList<tbl_BillDetail>> GetDetail(BillDetailSearch itemModel);
        Task<PagedList<tbl_PaymentSession>> GetPaymentSession(PaymentSessionSearch itemModel);
        Task<List<ClassByGradeModel>> GetClassByGrade(ClassInBillSearch request);
        Task<PagedList<StudentByClassModel>> GetStudentByClass(FilterInBillSearch request);
        /*Task<List<ItemStudentImport>> PrepareData(IFormFile excelFile);
        Task<string> ExportTemplate();*/
    }
}
