using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.DomainRequests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace Interface.Services
{
    public interface INecessaryService : IDomainService<tbl_Necessary, BaseSearch>
    {
        Task SendMail(SendMailModel model);
    }
}
