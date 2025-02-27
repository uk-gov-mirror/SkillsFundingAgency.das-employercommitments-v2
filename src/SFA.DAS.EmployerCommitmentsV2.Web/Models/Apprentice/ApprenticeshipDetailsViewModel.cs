﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice
{
    public class ApprenticeshipDetailsViewModel
    {
        public string EncodedApprenticeshipId { get; set; }
        public string ApprenticeName { get ; set ; }
        public string CourseName { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public string ProviderName { get; set; }
        public string Status { get; set; }
        public IEnumerable<string> Alerts { get; set; }
    }
}