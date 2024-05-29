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

namespace Service.Services
{
    public class LikeInNewsService : DomainService<tbl_LikeInNews, BaseSearch>, ILikeInNewsService
    {
        public LikeInNewsService(IAppUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public async Task<bool> GetLiked(Guid newsId, Guid userId)
        {
            return await unitOfWork.Repository<tbl_LikeInNews>()
                    .GetQueryable().AnyAsync(x => x.newsId == newsId && x.userId == userId && x.deleted == false && x.isLike == true);
        }
    }
}
