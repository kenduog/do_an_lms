using Entities;
using Extensions;
using Interface.DbContext;
using Interface.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Service.Services
{
    public class LookupService: ILookupService
    {
        private readonly IAppDbContext appDbContext;
        public LookupService(IAppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<tbl_Lookup>> GetByTypeCode(string code)
           => await this.appDbContext.Set<tbl_Lookup>().Where(x => x.lookupTypeCode == code).ToListAsync();

        public async Task<tbl_Lookup> GetSingle(string code)
           => await this.appDbContext.Set<tbl_Lookup>().FirstOrDefaultAsync(x => x.code == code);
    }
}
