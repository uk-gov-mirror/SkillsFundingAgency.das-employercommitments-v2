using FluentAssertions;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Controllers.ApprenticeControllerTests;

public class WhenCallingApprenticeshipApprovalRequest
{
    private WhenCallingApprenticeshipApprovalRequestFixture _fixture;

    [SetUp]
    public void Arrange() => _fixture = new WhenCallingApprenticeshipApprovalRequestFixture();

    [Test]
    public async Task ThenVerifyMapperWasCalled()
    {
        await _fixture.GetAllChangeHistory();

        _fixture.VerifyMapperWasCalled();
    }

    [Test]
    public async Task ThenReturnsViewModel()
    {
        var result = await _fixture.GetAllChangeHistory();

        _fixture.VerifyViewModel(result as ViewResult);
    }
}

public class WhenCallingApprenticeshipApprovalRequestFixture : ApprenticeControllerTestFixtureBase
{
    private readonly ApprenticeshipApprovalRequest _request;
    private readonly ApprenticeshipApprovalRequestViewModel _viewModel;

    public WhenCallingApprenticeshipApprovalRequestFixture()
    {
        var fixture = new Fixture();

        _request = fixture.Create<ApprenticeshipApprovalRequest>();
        _viewModel = fixture.Create<ApprenticeshipApprovalRequestViewModel>();

        MockMapper.Setup(m => m.Map<ApprenticeshipApprovalRequestViewModel>(_request)).ReturnsAsync(_viewModel);
    }

    public async Task<IActionResult> GetAllChangeHistory()
    {
        var result = await Controller.GetApprenticeshipApprovalRequest(_request);

        return result as ViewResult;
    }

    public void VerifyMapperWasCalled()
    {
        MockMapper.Verify(m => m.Map<ApprenticeshipApprovalRequestViewModel>(_request));
    }

    public void VerifyViewModel(ViewResult viewResult)
    {
        var viewModel = viewResult.Model as ApprenticeshipApprovalRequestViewModel;

        viewModel.Should().Be(_viewModel);
    }
}