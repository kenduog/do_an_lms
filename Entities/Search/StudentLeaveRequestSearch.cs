﻿using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Search
{
    public class StudentLeaveRequestSearch : BaseSearch
    {
        public Guid? studentId { get; set; }
    }
}
