﻿using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerCommitmentsV2.Web.Extensions;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice;
using SFA.DAS.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Mappers.Apprentice
{
    public class ApprenticeshipDetailsRequestToViewModelMapper : IMapper<ApprenticeshipDetailsRequest, ApprenticeshipDetailsRequestViewModel>
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IEncodingService _encodingService;
        private readonly ILogger<ApprenticeshipDetailsRequestToViewModelMapper> _logger;

        public ApprenticeshipDetailsRequestToViewModelMapper(ICommitmentsApiClient commitmentsApiClient, IEncodingService encodingService, ILogger<ApprenticeshipDetailsRequestToViewModelMapper> logger)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _encodingService = encodingService;
            _logger = logger;
        }

        public async Task<ApprenticeshipDetailsRequestViewModel> Map(ApprenticeshipDetailsRequest source)
        {
            try
            {
                var apprenticeshipId = _encodingService.Decode(source.ApprenticeshipHashedId, EncodingType.ApprenticeshipId);

                var apprenticeshipTask = _commitmentsApiClient.GetApprenticeship(apprenticeshipId, CancellationToken.None);
                var priceEpisodesTask = _commitmentsApiClient.GetPriceEpisodes(apprenticeshipId, CancellationToken.None);
                var apprenticeshipUpdatesTask = _commitmentsApiClient.GetApprenticeshipUpdates(apprenticeshipId, new GetApprenticeshipUpdatesRequest() { Status = ApprenticeshipUpdateStatus.Pending }, CancellationToken.None);
                var apprenticeshipDataLocksStatusTask = _commitmentsApiClient.GetApprenticeshipDatalocksStatus(apprenticeshipId, CancellationToken.None);
                var changeofPartyRequestsTask = _commitmentsApiClient.GetChangeOfPartyRequests(apprenticeshipId, CancellationToken.None);

                await Task.WhenAll(apprenticeshipTask, priceEpisodesTask, apprenticeshipUpdatesTask, apprenticeshipDataLocksStatusTask, changeofPartyRequestsTask);

                var apprenticeship = apprenticeshipTask.Result;
                var priceEpisodes = priceEpisodesTask.Result;
                var apprenticeshipUpdates = apprenticeshipUpdatesTask.Result;
                var apprenticeshipDataLocksStatus = apprenticeshipDataLocksStatusTask.Result;
                var changeofPartyRequests = changeofPartyRequestsTask.Result;

                var getTrainingProgramme = await _commitmentsApiClient.GetTrainingProgramme(apprenticeship.CourseCode, CancellationToken.None);
                PendingChanges pendingChange = GetPendingChanges(apprenticeshipUpdates);

                var statusText = MapApprenticeshipStatus(apprenticeship.Status);

                bool dataLockCourseTriaged = apprenticeshipDataLocksStatus.DataLocks.HasDataLockCourseTriaged();
                bool dataLockCourseChangedTraiged = apprenticeshipDataLocksStatus.DataLocks.HasDataLockCourseChangeTriaged();
                bool dataLockPriceTriaged = apprenticeshipDataLocksStatus.DataLocks.HasDataLockPriceTriaged();

                bool enableEdit = EnableEdit(apprenticeship, pendingChange, dataLockCourseTriaged, dataLockCourseChangedTraiged, dataLockPriceTriaged);

                var pendingChangeOfProviderRequest = changeofPartyRequests.ChangeOfPartyRequests?.Where(x => x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeProvider && x.Status == ChangeOfPartyRequestStatus.Pending).FirstOrDefault();
                var approvedChangeOfProviderRequest = changeofPartyRequests.ChangeOfPartyRequests?.Where(x => x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeProvider && x.Status == ChangeOfPartyRequestStatus.Approved).FirstOrDefault();
                var pendingChangeOfEmployerRequest = changeofPartyRequests.ChangeOfPartyRequests?.Where(x => x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeEmployer && x.Status == ChangeOfPartyRequestStatus.Pending).FirstOrDefault();
                var approvedChangeOfEmployerRequest = changeofPartyRequests.ChangeOfPartyRequests?.Where(x => x.ChangeOfPartyType == ChangeOfPartyRequestType.ChangeEmployer && x.Status == ChangeOfPartyRequestStatus.Approved).FirstOrDefault();

                var result = new ApprenticeshipDetailsRequestViewModel
                {
                    HashedApprenticeshipId = source.ApprenticeshipHashedId,
                    AccountHashedId = source.AccountHashedId,
                    ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}",
                    ULN = apprenticeship.Uln,
                    DateOfBirth = apprenticeship.DateOfBirth,
                    StartDate = apprenticeship.StartDate,
                    EndDate = apprenticeship.EndDate,
                    StopDate = apprenticeship.StopDate,
                    PauseDate = apprenticeship.PauseDate,
                    CompletionDate = apprenticeship.CompletionDate,
                    TrainingName = getTrainingProgramme.TrainingProgramme.Name,
                    TrainingType = getTrainingProgramme.TrainingProgramme.ProgrammeType,
                    Cost = priceEpisodes.PriceEpisodes.GetPrice(),
                    ApprenticeshipStatus = apprenticeship.Status,
                    Status = statusText,
                    ProviderName = apprenticeship.ProviderName,
                    PendingChanges = pendingChange,
                    EmployerReference = apprenticeship.EmployerReference,
                    CohortReference = _encodingService.Encode(apprenticeship.CohortId, EncodingType.CohortReference),
                    EnableEdit = enableEdit,
                    CanEditStatus = (apprenticeship.Status == ApprenticeshipStatus.Live || 
                                     apprenticeship.Status == ApprenticeshipStatus.WaitingToStart || 
                                     apprenticeship.Status == ApprenticeshipStatus.Paused),                    
                    EndpointAssessorName = apprenticeship.EndpointAssessorName,
                    MadeRedundant = apprenticeship.MadeRedundant,
                    HasPendingChangeOfProviderRequest = pendingChangeOfProviderRequest != null,
                    PendingChangeOfProviderRequestWithParty = pendingChangeOfProviderRequest?.WithParty,
                    HasApprovedChangeOfProviderRequest = approvedChangeOfProviderRequest != null,
                    HashedNewApprenticeshipId = approvedChangeOfProviderRequest?.NewApprenticeshipId != null
                            ? _encodingService.Encode(approvedChangeOfProviderRequest.NewApprenticeshipId.Value, EncodingType.ApprenticeshipId)
                            : null,
                    IsContinuation = apprenticeship.ContinuationOfId.HasValue,
                    HashedPreviousApprenticeshipId = apprenticeship.ContinuationOfId.HasValue
                            ? _encodingService.Encode(apprenticeship.ContinuationOfId.Value, EncodingType.ApprenticeshipId)
                            : null,
                    HasPendingChangeOfEmployerRequest = pendingChangeOfEmployerRequest != null,
                    PendingChangeOfEmployerRequestWithParty = pendingChangeOfEmployerRequest?.WithParty,
                    HasApprovedChangeOfEmployerRequest = approvedChangeOfEmployerRequest != null,
                    PendingDataLockChange = dataLockPriceTriaged || dataLockCourseChangedTraiged,
                    PendingDataLockRestart = dataLockCourseTriaged                   
                };

                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error mapping for accountId {source.AccountHashedId}  and apprenticeship {source.ApprenticeshipHashedId} to ApprenticeshipDetailsRequestViewModel");
                throw;
            }
        }

        private static bool EnableEdit(CommitmentsV2.Api.Types.Responses.GetApprenticeshipResponse apprenticeship, PendingChanges pendingChange, 
            bool dataLockCourseTriaged, bool dataLockCourseChangedTraiged, bool dataLockPriceTriaged)
        {         
            return pendingChange == PendingChanges.None
                            && !dataLockCourseTriaged
                            && !dataLockCourseChangedTraiged
                            && !dataLockPriceTriaged
                            && new[] { ApprenticeshipStatus.WaitingToStart, ApprenticeshipStatus.Live, ApprenticeshipStatus.Paused }.Contains(apprenticeship.Status);

        }

        private static PendingChanges GetPendingChanges(CommitmentsV2.Api.Types.Responses.GetApprenticeshipUpdatesResponse apprenticeshipUpdates)
        {
            var pendingChange = PendingChanges.None;
            if (apprenticeshipUpdates.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Employer))
                pendingChange = PendingChanges.WaitingForApproval;
            if (apprenticeshipUpdates.ApprenticeshipUpdates.Any(x => x.OriginatingParty == Party.Provider))
                pendingChange = PendingChanges.ReadyForApproval;
            return pendingChange;
        }      

        private string MapApprenticeshipStatus(ApprenticeshipStatus paymentStatus)
        {
            switch (paymentStatus)
            {
                
                case ApprenticeshipStatus.WaitingToStart:
                    return "Waiting to start";
                case ApprenticeshipStatus.Live:
                    return "Live";               
                case ApprenticeshipStatus.Paused:
                    return "Paused";
                case ApprenticeshipStatus.Stopped:
                    return "Stopped";
                case ApprenticeshipStatus.Completed:
                    return "Completed";
                default:
                    return string.Empty;
            }
        }

    }
   
}
