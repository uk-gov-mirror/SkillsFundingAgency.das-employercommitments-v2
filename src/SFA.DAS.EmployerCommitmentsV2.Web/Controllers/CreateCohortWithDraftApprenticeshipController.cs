﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Commitments.Shared.Extensions;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.EmployerCommitmentsV2.Extensions;
using SFA.DAS.EmployerCommitmentsV2.Features;
using SFA.DAS.EmployerCommitmentsV2.Web.Models;
using SFA.DAS.EmployerCommitmentsV2.Web.Requests;
using SFA.DAS.EmployerUrlHelper;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Controllers
{
    [Route("{AccountHashedId}/unapproved/add")]
    [DasAuthorize(EmployerFeature.EmployerCommitmentsV2, EmployerUserRole.OwnerOrTransactor)]
    public class CreateCohortWithDraftApprenticeshipController : Controller
    {
        private readonly ICommitmentsService _employerCommitmentsService;
        private readonly IMapper<AddDraftApprenticeshipViewModel, CreateCohortRequest> _createCohortRequestMapper;
        private readonly ILinkGenerator _linkGenerator;
        private readonly ITrainingProgrammeApiClient _trainingProgrammeApiClient;
        private readonly ICommitmentsApiClient _commitmentsApiClient;

        public CreateCohortWithDraftApprenticeshipController(
            ICommitmentsService employerCommitmentsService,
            IMapper<AddDraftApprenticeshipViewModel, CreateCohortRequest> createCohortRequestMapper,
            ILinkGenerator linkGenerator,
            ITrainingProgrammeApiClient trainingProgrammeApiClient,
            ICommitmentsApiClient commitmentsApiClient)
        {
            _employerCommitmentsService = employerCommitmentsService;
            _createCohortRequestMapper = createCohortRequestMapper;
            _linkGenerator = linkGenerator;
            _trainingProgrammeApiClient = trainingProgrammeApiClient;
            _commitmentsApiClient = commitmentsApiClient;
        }

        [HttpGet]
        [Route("apprentice")]
        public async Task<IActionResult> AddDraftApprenticeship(CreateCohortWithDraftApprenticeshipRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new AddDraftApprenticeshipViewModel
            {
                AccountLegalEntityId = request.AccountLegalEntityId,
                AccountLegalEntityHashedId = request.AccountLegalEntityHashedId,
                StartDate = new MonthYearModel(request.StartMonthYear),
                ReservationId = request.ReservationId,
                CourseCode = request.CourseCode,
                ProviderId = (int)request.ProviderId
            };

            await AddCoursesAndProviderNameToModel(model);
            
            return View(model);
        }

        [HttpPost]
        [Route("apprentice")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDraftApprenticeship(AddDraftApprenticeshipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await AddCoursesAndProviderNameToModel(model);
                return View(model);
            }

            var request = _createCohortRequestMapper.Map(model);
            request.UserId = User.Upn();
            
            try
            {
                var newCohort = await _employerCommitmentsService.CreateCohort(request);
                var reviewYourCohort = _linkGenerator.CommitmentsLink($"accounts/{model.AccountHashedId}/apprentices/{newCohort.CohortReference}/details");
                return Redirect(reviewYourCohort);
            }
            catch (CommitmentsApiModelException ex)
            {
                ModelState.AddModelExceptionErrors(ex);
                await AddCoursesAndProviderNameToModel(model);
                return View(model);
            }
        }

        private async Task AddCoursesAndProviderNameToModel(DraftApprenticeshipViewModel model)
        {
            var courses = await GetCourses();
            model.Courses = courses;

            var providerName = await GetProviderName(model.ProviderId);
            model.ProviderName = providerName;
        }

        private Task<IReadOnlyList<ITrainingProgramme>> GetCourses()
        {
            return _trainingProgrammeApiClient.GetAllTrainingProgrammes();
        }

        private async Task<string> GetProviderName(long providerId)
        {
            var provider = await _commitmentsApiClient.GetProvider(providerId);
            return provider.Name;
        }
    }
}