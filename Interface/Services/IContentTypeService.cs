using AutoMapper;
using Entities.AuthEntities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Interface.UnitOfWork;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IContentTypeService : IDomainService<tbl_ContentType, ContentTypeSearch>
    {
        Task<List<tbl_ContentType>> GetData(ContentTypeSearch baseSearch);
        Task<List<tbl_ContentType>> GetMenu(Guid? userId, bool isSuperUser, string groupCode);
        Task<List<tbl_ContentType>> GetAllMenu();
        Task<List<tbl_ContentType>> GetMenuByGroup(Guid? groupId);
    }
}
