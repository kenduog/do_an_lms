using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Services
{
    public interface IMultipleMessageService
    {
        Task<sys_MultipleMessage> GetItemByCode(string code);
        Task<string> GetMessage(string code, int? language = 1);
    }
}
