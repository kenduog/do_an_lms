﻿using Entities;
using Entities.DataTransferObject;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using Request.RequestCreate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Interface.Services
{
    public interface ITranscriptService : IDomainService<tbl_Transcript, TranscriptSearch>
    {
        Task<PagedList<tbl_Transcript>> GetOrGenerateTranscript(TranscriptSearch request);

    }
}
