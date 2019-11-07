﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerCommitmentsV2.Web.Exceptions;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.DraftApprenticeship;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Mappers.DraftApprenticeship
{
    public class DeleteDraftApprenticeshipViewModelMapper : IMapper<DeleteDraftApprenticeshipRequest, DeleteDraftApprenticeshipViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public DeleteDraftApprenticeshipViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<DeleteDraftApprenticeshipViewModel> Map(DeleteDraftApprenticeshipRequest source)
        {
            var cohort = await _commitmentsApiClient.GetCohort(source.CohortId);

            if (cohort.WithParty != Party.Employer)
            {
                throw new CohortEmployerUpdateDeniedException($"Cohort {cohort.CohortId} is not With the Employer");
            }

            var draftApprenticeship = await _commitmentsApiClient.GetDraftApprenticeship(source.CohortId, source.DraftApprenticeshipId);

            return new DeleteDraftApprenticeshipViewModel()
            {
                FirstName = draftApprenticeship.FirstName,
                LastName = draftApprenticeship.LastName,
                AccountHashedId = source.AccountHashedId,
                DraftApprenticeshipHashedId = source.DraftApprenticeshipHashedId,
                CohortReference = source.CohortReference
            };


        }
    }
}
