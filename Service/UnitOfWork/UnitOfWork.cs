using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Interface.UnitOfWork;
using Interface.DbContext;
using Interface.Repository;
using Entities.DomainEntities;

namespace Service
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected IAppDbContext context;
        public UnitOfWork(IAppDbContext context)
        {
            this.context = context;
            if (this.context != null)
            {
                //this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                this.context.ChangeTracker.AutoDetectChangesEnabled = false;
            }
        }


        public abstract IDomainRepository<T> Repository<T>() where T : Entities.DomainEntities.DomainEntities, new();

        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            { 
                throw e; 
            }
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return context.SaveChanges(acceptAllChangesOnSuccess);
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
