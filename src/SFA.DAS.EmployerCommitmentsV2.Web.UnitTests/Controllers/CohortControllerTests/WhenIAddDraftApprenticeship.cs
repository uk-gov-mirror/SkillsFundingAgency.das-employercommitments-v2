using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Authorization.Services;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerCommitmentsV2.Features;
using SFA.DAS.EmployerCommitmentsV2.Web.Controllers;
using SFA.DAS.EmployerCommitmentsV2.Web.Mappers;
using SFA.DAS.EmployerCommitmentsV2.Web.Mappers.Cohort;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Cohort;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.DraftApprenticeship;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Shared;
using SFA.DAS.EmployerUrlHelper;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Controllers.CohortControllerTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class CreateCohortWithDraftApprenticeshipControllerTests
    {
        [Test]
        public void Constructor_WithAllParams_ShouldNotThrowException()
        {
            var fixtures = new CreateCohortWithDraftApprenticeshipControllerTestFixtures();

            fixtures.CreateController();

            Assert.Pass("Created controller without exception");
        }

        [Test]
        public async Task GetAddDraftApprenticeship_ValidModel_ShouldReturnViewModel()
        {
            var fixtures = new CreateCohortWithDraftApprenticeshipControllerTestFixtures()
                .ForGetRequest()
                .WithTrainingProvider();

            var result = await fixtures.CheckGet();

            result
                .VerifyReturnsViewModel()
                .WithModel<ApprenticeViewModel>();
        }

        [Test]
        public async Task PostAddDraftApprenticeship_WithValidModel_WithEnhancedApproval_ShouldRedirectToCohortDetailsV2()
        {
            var fixtures = new CreateCohortWithDraftApprenticeshipControllerTestFixtures()
                .ForPostRequest()
                .WithCreatedCohort("ABC123", 123)
                .WithEnhancedApproval();

            var result = await fixtures.CheckPost();

            result.VerifyReturnsRedirectToActionResult().WithActionName("Details");
        }


        [Test]
        public async Task PostAddDraftApprenticeship_WithValidModel_WithoutEnhancedApproval_ShouldRedirectToCohortDetailsV1()
        {
            var redirectUrl = "V1CohortDetailsPage";

            var fixtures = new CreateCohortWithDraftApprenticeshipControllerTestFixtures()
                .ForPostRequest()
                .WithCreatedCohort("ABC123", 123)
                .WithReviewCohortLink(redirectUrl);

            var result = await fixtures.CheckPost();

            result.VerifyReturnsRedirect().WithUrl(redirectUrl);
        }


        [Test]
        public async Task PostAddDraftApprenticeship_WithValidModel_ShouldSaveCohort()
        {
            var fixtures = new CreateCohortWithDraftApprenticeshipControllerTestFixtures()
                .ForPostRequest()
                .WithCreatedCohort("ABC123", 123)
                .WithReviewCohortLink("someurl")
                .WithTrainingProvider();

            await fixtures.CheckPost();
            
            fixtures.CommitmentsApiClientMock.Verify(cs => cs.CreateCohort(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public class CreateCohortWithDraftApprenticeshipControllerTestFixtures
    {
        public CreateCohortWithDraftApprenticeshipControllerTestFixtures()
        {
            RequestMapper = new AddDraftApprenticeshipToCreateCohortRequestMapper();
            LinkGeneratorMock = new Mock<ILinkGenerator>();
            TrainingProgrammeApiClientMock = new Mock<ITrainingProgrammeApiClient>();
            CommitmentsApiClientMock = new Mock<ICommitmentsApiClient>();
            ModelMapperMock = new Mock<IModelMapper>();
            ModelMapperMock.Setup(x => x.Map<ApprenticeViewModel>(It.IsAny<ApprenticeRequest>()))
                .ReturnsAsync(new ApprenticeViewModel());
            AuthorizationServiceMock = new Mock<IAuthorizationService>();
            AuthorizationServiceMock.Setup(x => x.IsAuthorized(EmployerFeature.EnhancedApproval))
                .Returns(false);
        }

        public IMapper<AddDraftApprenticeshipViewModel, CreateCohortRequest> RequestMapper { get; }

        public Mock<ILinkGenerator> LinkGeneratorMock { get; }
        public ILinkGenerator LinkGenerator => LinkGeneratorMock.Object;

        public Mock<IModelMapper> ModelMapperMock { get; set; }
        public IModelMapper ModelMapper => ModelMapperMock.Object;

        public Mock<IAuthorizationService> AuthorizationServiceMock { get; set; }
        public IAuthorizationService AuthorizationService => AuthorizationServiceMock.Object;

        public Mock<ITrainingProgrammeApiClient> TrainingProgrammeApiClientMock { get; }

        public ApprenticeRequest GetRequest { get; private set; }
        public ApprenticeViewModel PostRequest { get; private set; }
        
        public Mock<ICommitmentsApiClient> CommitmentsApiClientMock { get; }
        public ICommitmentsApiClient CommitmentsApiClient => CommitmentsApiClientMock.Object;

        public CreateCohortWithDraftApprenticeshipControllerTestFixtures ForGetRequest()
        {
            GetRequest = new ApprenticeRequest {ProviderId = 1};
            return this;
        }

        public CreateCohortWithDraftApprenticeshipControllerTestFixtures ForPostRequest()
        {
            PostRequest = new ApprenticeViewModel {ProviderId = 1};
            return this;
        }

        public CreateCohortWithDraftApprenticeshipControllerTestFixtures WithReviewCohortLink(string url)
        {
            LinkGeneratorMock
                .Setup(lg => lg.CommitmentsLink(It.IsAny<string>()))
                .Returns(url);

            return this;
        }

        public CreateCohortWithDraftApprenticeshipControllerTestFixtures WithCreatedCohort(string cohortReference, long  cohortId)
        {
            CommitmentsApiClientMock
                .Setup(cs => cs.CreateCohort(It.IsAny<CreateCohortRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CreateCohortResponse {CohortReference = cohortReference, CohortId = cohortId});

            return this;
        }

        public CreateCohortWithDraftApprenticeshipControllerTestFixtures WithTrainingProvider()
        {
            var response = new GetProviderResponse {Name = "Name", ProviderId = 1};

            CommitmentsApiClientMock.Setup(p => p.GetProvider(1, CancellationToken.None))
                .ReturnsAsync(response);

            return this;
        }

        public CreateCohortWithDraftApprenticeshipControllerTestFixtures WithEnhancedApproval()
        {
            AuthorizationServiceMock
                .Setup(x => x.IsAuthorized(EmployerFeature.EnhancedApproval))
                .Returns(true);

            return this;
        }

        public CohortController CreateController()
        {
            var controller = new CohortController(
                CommitmentsApiClient,
                Mock.Of<ILogger<CohortController>>(),
                LinkGenerator,
                ModelMapper,
                AuthorizationService
            );
            return controller;
        }

        public Task<IActionResult> CheckGet()
        {
            var controller = CreateController();

            return controller.Apprentice(GetRequest);
        }

        public Task<IActionResult> CheckPost()
        {
            var controller = CreateController();

            return controller.Apprentice(PostRequest);
        }
    }
}