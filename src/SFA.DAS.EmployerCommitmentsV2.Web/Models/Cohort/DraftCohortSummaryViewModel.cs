﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Models.Cohort
{
    public class DraftCohortSummaryViewModel
    {
        public string CohortReference { get; set; }
        public string ProviderName { get; set; }
        public int NumberOfApprentices { get; set; }
    }
}
