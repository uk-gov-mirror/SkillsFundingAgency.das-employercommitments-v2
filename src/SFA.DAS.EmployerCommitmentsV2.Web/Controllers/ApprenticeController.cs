﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.EmployerCommitmentsV2.Features;
using SFA.DAS.EmployerCommitmentsV2.Web.Cookies;
using SFA.DAS.EmployerCommitmentsV2.Web.Extensions;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice;
using SFA.DAS.EmployerCommitmentsV2.Web.RouteValues;
using SFA.DAS.EmployerUrlHelper;
using EditEndDateRequest = SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice.EditEndDateRequest;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Controllers
{
    [Route("{accountHashedId}/apprentices")]
    [SetNavigationSection(NavigationSection.ApprenticesHome)]
    [DasAuthorize(EmployerUserRole.OwnerOrTransactor)]
    public class ApprenticeController : Controller
    {
        private readonly IModelMapper _modelMapper;
        private readonly ICookieStorageService<IndexRequest> _cookieStorage;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly ILinkGenerator _linkGenerator;

        public ApprenticeController(IModelMapper modelMapper, ICookieStorageService<IndexRequest> cookieStorage, ICommitmentsApiClient commitmentsApiClient, ILinkGenerator linkGenerator)
        {
            _modelMapper = modelMapper;
            _cookieStorage = cookieStorage;
            _commitmentsApiClient = commitmentsApiClient;
            _linkGenerator = linkGenerator;
        }

        [Route("", Name = RouteNames.ApprenticesIndex)]
        public async Task<IActionResult> Index(IndexRequest request)
        {
            IndexRequest savedRequest = null;

            if (request.FromSearch)
            {
                savedRequest = _cookieStorage.Get(CookieNames.ManageApprentices);

                if (savedRequest != null)
                {
                    request = savedRequest;
                }
            }

            if (savedRequest == null)
            {
                _cookieStorage.Update(CookieNames.ManageApprentices, request);
            }

            var viewModel = await _modelMapper.Map<IndexViewModel>(request);
            viewModel.SortedByHeader();

            return View(viewModel);
        }

        [Route("download", Name = RouteNames.ApprenticesDownload)]
        public async Task<IActionResult> Download(DownloadRequest request)
        {
            var downloadViewModel = await _modelMapper.Map<DownloadViewModel>(request);

            return File(downloadViewModel.Content, downloadViewModel.ContentType, downloadViewModel.Name);
        }

        [Route("{apprenticeshipHashedId}/details/editenddate", Name = RouteNames.ApprenticeEditEndDate)]
        [DasAuthorize(EmployerFeature.ManageApprenticesV2)]
        public async Task<IActionResult> EditEndDate(EditEndDateRequest request)
        {
            var viewModel = await _modelMapper.Map<EditEndDateViewModel>(request);
            return View(viewModel);
        }

        [Route("{apprenticeshipHashedId}/details/editenddate", Name = RouteNames.ApprenticeEditEndDate)]
        [DasAuthorize(EmployerFeature.ManageApprenticesV2)]
        [HttpPost]
        public async Task<IActionResult> EditEndDate(EditEndDateViewModel viewModel)
        {
            var request = await _modelMapper.Map<CommitmentsV2.Api.Types.Requests.EditEndDateRequest>(viewModel);
            await _commitmentsApiClient.UpdateEndDateOfCompletedRecord(request, CancellationToken.None);
            var url = _linkGenerator.ApprenticeDetails(viewModel.AccountHashedId, viewModel.ApprenticeshipHashedId);
            return Redirect(url);
        }


        [Route("{apprenticeshipHashedId}/change-provider/changing-training-provider", Name = RouteNames.ChangeProviderInform)]
        [DasAuthorize(EmployerFeature.ChangeOfProvider)]
        public async Task<IActionResult> ChangeProviderInform(ChangeProviderInformRequest request)
        {
            var viewModel = await _modelMapper.Map<ChangeProviderInformViewModel>(request);
            return View(viewModel);
        }

        [Route("{apprenticeshipHashedId}/details/changestatus")]
        [DasAuthorize(EmployerFeature.ManageApprenticesV2)]
        [HttpGet]
        public async Task<IActionResult> ChangeStatus(ChangeStatusRequest request)
        {
            var viewModel = await _modelMapper.Map<ChangeStatusRequestViewModel>(request);


            return View(viewModel);
        }


        // Placeholder for CON-2516 - url not specified yet
        [Route("{apprenticeshipHashedId}/change-provider/stopped-error", Name = RouteNames.ApprenticeNotStoppedError)]
        [DasAuthorize(EmployerFeature.ChangeOfProvider)]
        public IActionResult ApprenticeNotStoppedError()
        {
            return View();
        }


        [HttpGet]
        [Route("{apprenticeshipHashedId}/change-provider/enter-new-training-provider-name-or-reference-number", Name = RouteNames.EnterNewTrainingProvider)]
        [DasAuthorize(EmployerFeature.ChangeOfProvider)]
        public async Task<IActionResult> EnterNewTrainingProvider(EnterNewTrainingProviderRequest request)
        {
            var viewModel = await _modelMapper.Map<EnterNewTrainingProviderViewModel>(request);

            return View(viewModel);
        }

        [HttpPost]
        [Route("{apprenticeshipHashedId}/change-provider/enter-new-training-provider-name-or-reference-number", Name = RouteNames.EnterNewTrainingProvider)]
        [DasAuthorize(EmployerFeature.ChangeOfProvider)]
        public async Task<IActionResult> EnterNewTrainingProvider(EnterNewTrainingProviderViewModel viewModel)
        {
            var request = await _modelMapper.Map<SendNewTrainingProviderRequest>(viewModel);

            return RedirectToRoute(RouteNames.SendRequestNewTrainingProvider, new { request.AccountHashedId, request.ApprenticeshipHashedId, request.ProviderId });
        }

        [Route("{apprenticeshipHashedId}/change-provider/send-request-new-training-provider", Name = RouteNames.SendRequestNewTrainingProvider)]
        [DasAuthorize(EmployerFeature.ChangeOfProvider)]
        public async Task<IActionResult> SendRequestNewTrainingProvider(SendNewTrainingProviderRequest request)
        {
            var viewModel = await _modelMapper.Map<SendNewTrainingProviderViewModel>(request);
            return View(viewModel);
        }

        [Route("{apprenticeshipHashedId}/change-provider/send-request-new-training-provider", Name = RouteNames.SendRequestNewTrainingProvider)]
        [HttpPost]
        [DasAuthorize(EmployerFeature.ChangeOfProvider)]
        public IActionResult SendRequestNewTrainingProvider(SendNewTrainingProviderViewModel request)
        {
            if (request.Confirm.Value)
            {
                return RedirectToRoute(RouteNames.Sent);
            }

            return Redirect(_linkGenerator.ApprenticeDetails(request.AccountHashedId, request.ApprenticeshipHashedId));
        }

        [Route("{apprenticeshipHashedId}/change-provider/sent", Name = RouteNames.Sent)]
        public IActionResult Sent()
        {
            // Place holder to display information sent.
            return View();
        }

        [Route("{apprenticeshipHashedId}/details/changestatus")]
        [DasAuthorize(EmployerFeature.ManageApprenticesV2)]
        [HttpPost]
        public IActionResult ChangeStatus(ChangeStatusRequestViewModel viewModel)
        {
            switch (viewModel.SelectedStatusChange)
            {
                case ChangeStatusType.Pause:
                    return RedirectToAction(nameof(PauseApprenticeship), new { viewModel.AccountHashedId, viewModel.ApprenticeshipHashedId });
                case ChangeStatusType.Stop:
                    return Redirect(_linkGenerator.WhenToApplyStopApprentice(viewModel.AccountHashedId, viewModel.ApprenticeshipHashedId));
                case ChangeStatusType.Resume:
                    return RedirectToAction(nameof(ResumeApprenticeship), new { viewModel.AccountHashedId, viewModel.ApprenticeshipHashedId });
                default:
                    return Redirect(_linkGenerator.ApprenticeDetails(viewModel.AccountHashedId, viewModel.ApprenticeshipHashedId));
            }
        }

        [Route("{apprenticeshipHashedId}/details/pause")]
        [DasAuthorize(EmployerFeature.ManageApprenticesV2)]
        [HttpGet]
        public async Task<IActionResult> PauseApprenticeship(PauseRequest request)
        {
            var viewModel = await _modelMapper.Map<PauseRequestViewModel>(request);
            return View(viewModel);
        }

        [Route("{apprenticeshipHashedId}/details/pause")]
        [DasAuthorize(EmployerFeature.ManageApprenticesV2)]
        [HttpPost]
        public async Task<IActionResult> PauseApprenticeship(PauseRequestViewModel viewModel)
        {
            if (viewModel.PauseConfirmed.HasValue && viewModel.PauseConfirmed.Value)
            {
                var pauseRequest = new PauseApprenticeshipRequest { ApprenticeshipId = viewModel.ApprenticeshipId };

                await _commitmentsApiClient.PauseApprenticeship(pauseRequest, CancellationToken.None);
            }
            
            return Redirect(_linkGenerator.ApprenticeDetails(viewModel.AccountHashedId, viewModel.ApprenticeshipHashedId));
        }

        [DasAuthorize(EmployerFeature.ManageApprenticesV2)]
        [Route("{apprenticeshipHashedId}/details/resume")]
        [HttpGet]
        public async Task<IActionResult> ResumeApprenticeship(ResumeRequest request)
        {
            var viewModel = await _modelMapper.Map<ResumeRequestViewModel>(request);
            return View(viewModel);
        }

        [DasAuthorize(EmployerFeature.ManageApprenticesV2)]
        [Route("{apprenticeshipHashedId}/details/resume")]
        [HttpPost]
        public async Task<IActionResult> ResumeApprenticeship(ResumeRequestViewModel viewModel)
        {
            if (viewModel.ResumeConfirmed.HasValue && viewModel.ResumeConfirmed.Value)
            {
                var resumeRequest = new ResumeApprenticeshipRequest { ApprenticeshipId = viewModel.ApprenticeshipId };

                await _commitmentsApiClient.ResumeApprenticeship(resumeRequest, CancellationToken.None);
            }

            return Redirect(_linkGenerator.ApprenticeDetails(viewModel.AccountHashedId, viewModel.ApprenticeshipHashedId));
        }
    }
}