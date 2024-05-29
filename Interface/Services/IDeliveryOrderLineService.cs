﻿using Entities;
using Entities.DomainEntities;
using Entities.Search;
using Interface.Services.DomainServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Interface.Services
{
    public interface IDeliveryOrderLineService : IDomainService<tbl_DeliveryOrderLine, DeliveryOrderLineSearch>
    {
    }
}
