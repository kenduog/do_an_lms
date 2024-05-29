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
    public interface IValueEntryService : IDomainService<tbl_ValueEntry, ValueEntrySearch>
    {
        Task<ValueEntryReport> GetValueEntry(ValueEntrySearch baseSearch);
        Task<string> Export(ValueEntrySearch baseSearch);
    }
}
