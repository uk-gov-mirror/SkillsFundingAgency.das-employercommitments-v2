﻿using System.Net;
using System.Net.Http;
using System.Threading;
using AutoFixture.NUnit3;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerCommitmentsV2.Web.Controllers;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.CreateCohort;
using SFA.DAS.Testing.AutoFixture;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http.Connections.Internal;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.Http;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Controllers.CreateCohortControllerTests
{
    [TestFixture]
    public class WhenPostingSelectProvider
    {
        [Test, MoqAutoData]
        public async Task AndViewModelIsInvalid_ThenReturnsView(
            SelectProviderViewModel viewModel,
            ValidationResult validationResult,
            ValidationFailure error,
            [Frozen] Mock<IValidator<SelectProviderViewModel>> mockValidator,
            CreateCohortController controller)
        {
            validationResult.Errors.Add(error);
            mockValidator
                .Setup(x => x.Validate(viewModel))
                .Returns(validationResult);

            var result = await controller.SelectProvider(viewModel) as ViewResult;
            Assert.Null(result.ViewName);
            Assert.AreSame(viewModel,result.ViewData.Model);

        }

        [Test, MoqAutoData]
        public async Task ThenCallsApi(
            SelectProviderViewModel viewModel,
            long providerId,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetProviderResponse apiResponse,
            CreateCohortController controller)
        {
            viewModel.ProviderId = providerId.ToString();
            
            await controller.SelectProvider(viewModel);

            mockApiClient.Verify(x => x.GetProvider(providerId, CancellationToken.None), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task AndApiThrowsNotFoundException_ThenReturnsViewWithFailedValidation(
            SelectProviderViewModel viewModel,
            long providerId,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetProviderResponse apiResponse,
            HttpResponseMessage error,
            CreateCohortController controller)
        {
            var viewName = "ConfirmProvider";
            error.StatusCode = HttpStatusCode.NotFound;
            viewModel.ProviderId = providerId.ToString();
            mockApiClient
                .Setup(x => x.GetProvider(providerId, CancellationToken.None))
                .ThrowsAsync(new RestHttpClientException(error,error.ReasonPhrase));

            var result = await controller.SelectProvider(viewModel) as ViewResult;

            Assert.Null(result.ViewName);
            Assert.AreSame(viewModel,result.ViewData.Model);
            Assert.False(controller.ModelState.IsValid);
        }

        [Test, MoqAutoData]
        public async Task AndUnexpectedExceptionThrown_ThenReturnsErrorView(
            SelectProviderViewModel viewModel,
            long providerId,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            GetProviderResponse apiResponse,
            HttpResponseMessage error,
            CreateCohortController controller)
        {
            var viewName = "~/Views/Error/Error.cshtml";
            error.StatusCode = HttpStatusCode.NetworkAuthenticationRequired;
            viewModel.ProviderId = providerId.ToString();
            mockApiClient
                .Setup(x => x.GetProvider(providerId, CancellationToken.None))
                .ThrowsAsync(new RestHttpClientException(error, error.ReasonPhrase));

            var result = await controller.SelectProvider(viewModel) as ViewResult;

            Assert.NotNull(result);
            Assert.AreEqual(viewName,result.ViewName);
        }


        [Test, MoqAutoData]
        public async Task ThenMapsConfirmProviderRequest(
            SelectProviderViewModel viewModel,
            long providerId,
            ValidationResult validationResult,
            [Frozen] Mock<IValidator<SelectProviderViewModel>> mockValidator,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            [Frozen] Mock<IMapper<SelectProviderViewModel, ConfirmProviderRequest>> mockConfirmProviderRequestMapper,
            GetProviderResponse apiResponse,
            CreateCohortController controller)
        {
            viewModel.ProviderId = providerId.ToString();
            mockValidator
                .Setup(x => x.Validate(viewModel))
                .Returns(validationResult);
            mockApiClient
                .Setup(x => x.GetProvider(long.Parse(viewModel.ProviderId), CancellationToken.None))
                .ReturnsAsync(apiResponse);

            await controller.SelectProvider(viewModel);

            mockConfirmProviderRequestMapper.Verify(x => x.Map(viewModel), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task ThenRedirectsToConfirmProvider(
            SelectProviderViewModel viewModel,
            long providerId,
            ValidationResult validationResult,
            [Frozen] Mock<IValidator<SelectProviderViewModel>> mockValidator,
            [Frozen] Mock<ICommitmentsApiClient> mockApiClient,
            [Frozen] Mock<IMapper<SelectProviderViewModel, ConfirmProviderRequest>> mockConfirmProviderRequestMapper,
            GetProviderResponse apiResponse,
            ConfirmProviderRequest confirmProviderRequest,
            CreateCohortController controller)
        {
            var actionName = "ConfirmProvider";
            viewModel.ProviderId = providerId.ToString();
            mockValidator
                .Setup(x => x.Validate(viewModel))
                .Returns(validationResult);
            mockApiClient
                .Setup(x => x.GetProvider(long.Parse(viewModel.ProviderId), CancellationToken.None))
                .ReturnsAsync(apiResponse);
            mockConfirmProviderRequestMapper
                .Setup(x => x.Map(viewModel))
                .Returns(confirmProviderRequest);

            var result = await controller.SelectProvider(viewModel) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.AreEqual(actionName,result.ActionName);
        }

    }
}
