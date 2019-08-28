﻿using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerCommitmentsV2.Web.Mappers;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Cohort;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Mappers
{
    public class WhenMappingConfirmProviderRequestToViewModel
    {
        [Test, MoqAutoData]
        public async Task Then_Maps_AccountHashedId(
            ConfirmProviderRequest request,
            [Frozen] Mock<ICommitmentsApiClient> commitmentsApiClient,
            ConfirmProviderViewModelMapper mapper)
        {
            var viewModel = await mapper.Map(request);

            viewModel.AccountHashedId.Should().Be(request.AccountHashedId);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_EmployerAccountLegalEntityPublicHashedId(
            ConfirmProviderRequest request,
            [Frozen] Mock<ICommitmentsApiClient> commitmentsApiClient,
            ConfirmProviderViewModelMapper mapper)
        {
            var viewModel = await mapper.Map(request);

            viewModel.AccountLegalEntityHashedId.Should().Be(request.AccountLegalEntityHashedId);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_ReservationId(
            ConfirmProviderRequest request,
            [Frozen] Mock<ICommitmentsApiClient> commitmentsApiClient,
            ConfirmProviderViewModelMapper mapper)
        {
            var viewModel = await mapper.Map(request);

            viewModel.ReservationId.Should().Be(request.ReservationId);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_StartMonthYear(
            ConfirmProviderRequest request,
            [Frozen] Mock<ICommitmentsApiClient> commitmentsApiClient,
            ConfirmProviderViewModelMapper mapper)
        {
            var viewModel = await mapper.Map(request);

            viewModel.StartMonthYear.Should().Be(request.StartMonthYear);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_CourseCode(
            ConfirmProviderRequest request,
            [Frozen] Mock<ICommitmentsApiClient> commitmentsApiClient,
            ConfirmProviderViewModelMapper mapper)
        {
            var viewModel = await mapper.Map(request);

            viewModel.CourseCode.Should().Be(request.CourseCode);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_ProviderId(
            ConfirmProviderRequest request,
            [Frozen] Mock<ICommitmentsApiClient> commitmentsApiClient,
            ConfirmProviderViewModelMapper mapper)
        {
            var viewModel = await mapper.Map(request);

            viewModel.ProviderId.Should().Be(request.ProviderId);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_ProviderName(
            ConfirmProviderRequest request,
            [Frozen] Mock<ICommitmentsApiClient> commitmentsApiClient,
            GetProviderResponse provider,
            ConfirmProviderViewModelMapper mapper)
        {
            commitmentsApiClient.Setup(x => x.GetProvider(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(provider);
            var viewModel = await mapper.Map(request);
            viewModel.ProviderName.Should().Be(provider.Name);
        }
    }
}