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
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Controllers.ApprenticeControllerTests
{
    public class WhenRequestingHasTheApprenticeBeenMadeRedundantPage : ApprenticeControllerTestBase
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

        [Test, MoqAutoData]
        public async Task WhenRequesting_HasTheAppretniceBeenMadeRedundant_ThenMadeRedundantViewModelIsPassedToTheView(MadeRedundantViewModel expectedViewModel)
        {
            _mockModelMapper.Setup(m => m.Map<MadeRedundantViewModel>(It.IsAny<MadeRedundantRequest>()))
                .ReturnsAsync(expectedViewModel);

            var viewResult = await _controller.HasTheApprenticeBeenMadeRedundant(new MadeRedundantRequest()) as ViewResult;
            var viewModel = viewResult.Model;

            var actualViewModel = (MadeRedundantViewModel)viewModel;

            Assert.IsInstanceOf<MadeRedundantViewModel>(viewModel);
            Assert.AreEqual(expectedViewModel, actualViewModel);
        }
    }
}
