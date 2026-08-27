using FluentAssertions;
using SFA.DAS.EmployerCommitmentsV2.Contracts;
using SFA.DAS.EmployerCommitmentsV2.Web.Mappers.Apprentice;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Mappers.Apprentice;

public class ApprenticeshipApprovalRequestAlertsViewModelToUpdateApprovalRequestAlertsRequestMapperTests
{
    [Test, MoqAutoData]
    public async Task Then_RequestIsMapped
        (ApprenticeshipApprovalRequestAlertsViewModel viewModel)
    {
        var mockService = new Mock<IAuthenticationService>().Object;
        var _mapper = new ApprenticeshipApprovalRequestAlertsViewModelToUpdateApprovalRequestAlertsRequestMapper(mockService);

        var request = await _mapper.Map(viewModel);

        request.ApprovalRequestAlerts.Should().HaveCount(viewModel.ApprovalRequests.Count);

        foreach (var item in request.ApprovalRequestAlerts)
        {
            var actual = viewModel.ApprovalRequests.Single(t => t.Id == item.ApprovalRequestId);

            if (actual.Seen.HasValue && actual.Seen.Value)
            {
                item.EmployerAcknowledgedAt.Should().Be(DateTime.UtcNow.Date);
                item.EmployerAcknowledgedBy.Should().Be(mockService.UserName);
            }
            else
            {
                item.EmployerAcknowledgedAt.Should().BeNull();
                item.EmployerAcknowledgedBy.Should().BeNull();
            }
            item.ApprovalRequestId.Should().Be(actual.Id);
        }
    }
}