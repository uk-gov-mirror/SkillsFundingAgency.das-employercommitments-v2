﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Cohort;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Mappers.Cohort
{
    public class DetailsViewModelMapper : IMapper<DetailsRequest, DetailsViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IEncodingService _encodingService;

        public DetailsViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _encodingService = encodingService;
        }

        public async Task<DetailsViewModel> Map(DetailsRequest source)
        {
            var cohortTask = _commitmentsApiClient.GetCohort(source.CohortId);
            var draftApprenticeshipsTask = _commitmentsApiClient.GetDraftApprenticeships(source.CohortId);

            await Task.WhenAll(cohortTask, draftApprenticeshipsTask);

            var cohort = await cohortTask;
            var draftApprenticeships = (await draftApprenticeshipsTask).DraftApprenticeships;

            return new DetailsViewModel
            {
                AccountHashedId = source.AccountHashedId,
                CohortReference = source.CohortReference,
                WithParty = cohort.WithParty,
                AccountLegalEntityHashedId = _encodingService.Encode(cohort.AccountLegalEntityId, EncodingType.PublicAccountLegalEntityId),
                LegalEntityName = cohort.LegalEntityName,
                ProviderName = cohort.ProviderName,
                TransferSenderHashedId = cohort.TransferSenderId == null ? null : _encodingService.Encode(cohort.TransferSenderId.Value, EncodingType.PublicAccountId),
                Message = cohort.LatestMessageCreatedByProvider,
                Courses = GroupCourses(draftApprenticeships),
                PageTitle = draftApprenticeships.Count == 1
                    ? "Approve apprentice details"
                    : $"Approve {draftApprenticeships.Count} apprentices' details",
                IsApprovedByProvider = cohort.IsApprovedByProvider,
            };
        }

        private IReadOnlyCollection<DetailsViewCourseGroupingModel> GroupCourses(IEnumerable<DraftApprenticeshipDto> draftApprenticeships)
        {
            return draftApprenticeships
                .GroupBy(a => new {a.CourseCode, a.CourseName})
                .Select(course => new DetailsViewCourseGroupingModel
                {
                    CourseCode = course.Key.CourseCode,
                    CourseName = course.Key.CourseName,
                    DraftApprenticeships = course 
                        // Sort before on raw properties rather than use displayName property post select for performance reasons
                        .OrderBy(a => a.FirstName)
                        .ThenBy(a => a.LastName)
                        .Select(a => new CohortDraftApprenticeshipViewModel
                        {
                            Id = a.Id,
                            DraftApprenticeshipHashedId = _encodingService.Encode(a.Id, EncodingType.ApprenticeshipId),
                            FirstName = a.FirstName,
                            LastName = a.LastName,
                            Cost = a.Cost,
                            DateOfBirth = a.DateOfBirth,
                            EndDate = a.EndDate,
                            StartDate = a.StartDate
                        })
                        .ToList()
                })
                .OrderBy(c => c.CourseName)
                .ToList();
        }
    }
}