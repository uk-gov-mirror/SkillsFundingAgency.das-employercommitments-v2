﻿using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Controllers.ApprenticeControllerTests
{
    public class WhenPostingEditApprenticeshipDetails
    {
        private Fixture _autoFixture;
        private WhenPostingEditApprenticeshipDetailsFixture _fixture;
        private EditApprenticeshipRequestViewModel _viewModel;

        [SetUp]
        public void Arrange()
        {
            _autoFixture = new Fixture();
            _fixture = new WhenPostingEditApprenticeshipDetailsFixture();
            _viewModel = new EditApprenticeshipRequestViewModel();
        }   

        [Test]
        public async Task VerifyValidationApiIsCalled()
        {
            var result = await _fixture.EditApprenticeship(_viewModel);
            _fixture.VerifyValidationApiIsCalled();
        }

        [Test]
        public async Task VerifyMapperIsCalled()
        {
            await _fixture.EditApprenticeship(_viewModel);
            _fixture.VerifyMapperIsCalled();
        }
    }

    public class WhenPostingEditApprenticeshipDetailsFixture : ApprenticeControllerTestFixtureBase
    {
        public WhenPostingEditApprenticeshipDetailsFixture() : base () { }

        public async Task<IActionResult> EditApprenticeship(EditApprenticeshipRequestViewModel viewModel)
        {
            return await _controller.EditApprenticeship(viewModel);
        }     

        public void VerifyValidationApiIsCalled()
        {
            _mockCommitmentsApiClient.Verify(x => x.ValidateApprenticeshipForEdit(It.IsAny<ValidateApprenticeshipForEditRequest>(), CancellationToken.None), Times.Once());
        }

        public void VerifyMapperIsCalled()
        {
            _mockMapper.Verify(x => x.Map<ValidateApprenticeshipForEditRequest>(It.IsAny<EditApprenticeshipRequestViewModel>()), Times.Once());
        }
    }
}
