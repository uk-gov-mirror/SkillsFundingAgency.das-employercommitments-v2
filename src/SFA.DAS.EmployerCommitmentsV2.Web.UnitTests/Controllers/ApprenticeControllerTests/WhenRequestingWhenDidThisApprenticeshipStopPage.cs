﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.Services;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.EmployerCommitmentsV2.Web.Controllers;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice;
using SFA.DAS.EmployerUrlHelper;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Controllers.ApprenticeControllerTests
{
    public class WhenRequestingWhenDidThisApprenticeshipStopPage : ApprenticeControllerTestBase
    {
        [SetUp]
        public void Arrange()
        {
            _mockModelMapper = new Mock<IModelMapper>();
            _mockCookieStorageService = new Mock<ICookieStorageService<IndexRequest>>();
            _mockCommitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _mockLinkGenerator = new Mock<ILinkGenerator>();

            _controller = new ApprenticeController(_mockModelMapper.Object, 
                _mockCookieStorageService.Object, 
                _mockCommitmentsApiClient.Object,
                _mockLinkGenerator.Object, 
                Mock.Of<ILogger<ApprenticeController>>(), 
                Mock.Of<IAuthorizationService>());
        }

        [Test]
        public async Task WhenRequesting_WhenDidThisApprenticeshipStop_ThenStopRequestViewModelIsPassedToTheView()
        {
            StopRequestViewModel _viewModel = new StopRequestViewModel { StopMonth = 6, StopYear = 2020, ApprenticeshipId = 1, AccountHashedId = "AAXX" };
            _mockModelMapper.Setup(m => m.Map<StopRequestViewModel>(It.IsAny<StopRequest>()))
                .ReturnsAsync(_viewModel);

            var viewResult = await _controller.StopApprenticeship(new StopRequest()) as ViewResult;
            var viewModel = viewResult.Model;

            var stopRequestViewModel = (StopRequestViewModel)viewModel;

            Assert.IsInstanceOf<StopRequestViewModel>(viewModel);
            Assert.AreEqual(_viewModel, stopRequestViewModel);
        }
    }
}
