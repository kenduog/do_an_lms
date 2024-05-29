using Entities.DomainEntities;
using Interface.DbContext;
using Interface.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Repository
{
    public class AppRepository<T> : DomainRepository<T>, IAppRepository<T> where T : Entities.DomainEntities.DomainEntities, new()  
    {
        public AppRepository(IAppDbContext context) : base(context)
        {

        }

    }
}
