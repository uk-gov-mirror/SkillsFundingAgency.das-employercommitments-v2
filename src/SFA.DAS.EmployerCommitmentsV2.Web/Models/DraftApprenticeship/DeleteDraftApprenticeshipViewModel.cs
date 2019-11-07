﻿using SFA.DAS.Authorization.ModelBinding;
using SFA.DAS.EmployerCommitmentsV2.Web.Enums;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Shared;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Models.DraftApprenticeship
{
    public class DeleteDraftApprenticeshipViewModel : DraftApprenticeshipViewModel, IAuthorizationContextModel
    {
        public string AccountHashedId { get; set; }
        public string DraftApprenticeshipHashedId { get; set; }
        public string BackLink { get; set; }
        public bool? ConfirmDelete { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
