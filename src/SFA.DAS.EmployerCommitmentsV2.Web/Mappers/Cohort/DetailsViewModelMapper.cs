﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Cohort;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Mappers.Cohort
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentsApiClient)
        {
            _commitmentsApiClient = commitmentsApiClient;
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            var cohortTask = _commitmentsApiClient.GetCohort(source.CohortId);
            var draftApprenticeshipsTask = _commitmentsApiClient.GetDraftApprenticeships(source.CohortId);

            await Task.WhenAll(new List<Task> { cohortTask, draftApprenticeshipsTask });

            var cohort = await cohortTask;
            var draftApprenticeships = (await draftApprenticeshipsTask).DraftApprenticeships;

            return new DetailsViewModel
            {
                AccountHashedId = source.AccountHashedId,
                CohortReference = source.CohortReference,
                WithParty = cohort.WithParty,
                LegalEntityName = cohort.LegalEntityName,
                ProviderName = cohort.ProviderName,
                Message = cohort.LatestMessageCreatedByProvider,
                DraftApprenticeships = draftApprenticeships.Select(a => new CohortDraftApprenticeshipViewModel
                    {
                        Id = a.Id,
                        FirstName = a.FirstName,
                        LastName = a.LastName,
                        Cost = a.Cost,
                        CourseCode = a.CourseCode,
                        CourseName = a.CourseName,
                        DateOfBirth = a.DateOfBirth,
                        EndDate = a.EndDate,
                        StartDate = a.StartDate
                    }).ToList()
            };
        }
    }
}
