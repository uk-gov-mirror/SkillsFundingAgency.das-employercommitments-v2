﻿using SFA.DAS.Authorization.ModelBinding;
using System;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice
{
    public class ConfirmDetailsAndSendViewModel : IAuthorizationContextModel
    {
        public string AccountHashedId { get; set; }
        public string ApprenticeshipHashedId { get; set; }
        public string ProviderName { get; set; }
        public long ProviderId { get; set; }
        public bool Edit { get; set; }
       
        public string ApprenticeFullName { get; set; }
        public DateTime? ApprenticeshipStopDate { get; set; }
        public string CurrentProviderName { get; set; }
        public DateTime CurrentStartDate { get; set; }
        public DateTime CurrentEndDate { get; set; }
        public int CurrentPrice { get; set; }

        public DateTime NewStartDate { get; set; }
        public DateTime NewEndDate { get; set; }
        public int? NewPrice { get; set; }
    }

}
