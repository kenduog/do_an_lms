﻿using Entities.DomainEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utilities.CoreContants;

namespace Entities.Search
{
    public class ChildAssessmentDetailSearch : BaseSearch
    {
        public Guid? assessmentTopicId { get; set; }
        public Guid? studentId { get; set; }

    }
}
