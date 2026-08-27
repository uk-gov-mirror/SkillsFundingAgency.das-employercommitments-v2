using FluentAssertions;
using SFA.DAS.EmployerCommitmentsV2.Services.Approvals.Responses;
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

    [Test]
    public async Task ThenDisplaysMessageIfStatusIsNotLivePauseOrWaitingToStart()
    {
        var result = await _fixture.SetChangeApprovalAllowed(false).GetAllChangeHistory();

        _fixture.VerifyChangeApprovalAllowedModelStateError(result as ViewResult);
    }

    [Test]
    public async Task ThenReturnsViewModelIfStatusIsLivePauseOrWaitingToStart()
    {
        var result = await _fixture.SetChangeApprovalAllowed(true).GetAllChangeHistory();

        _fixture.VerifyViewModel(result as ViewResult);
    }

    [Test]
    public async Task ThenDisplaysMessageIfChangeHasBeenCompleted()
    {
        var result = await _fixture.SetApprovalRequestStatus(CocApprovalResultStatus.Complete).GetAllChangeHistory();

        _fixture.VerifyApprovalRequestStatusModelStateError(result as ViewResult);
    }

    [Test]
    public async Task ThenReturnsViewModelIfChangeIsPending()
    {
        var result = await _fixture.SetApprovalRequestStatus(CocApprovalResultStatus.Pending).GetAllChangeHistory();

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

    public WhenCallingApprenticeshipApprovalRequestFixture SetApprovalRequestStatus(CocApprovalResultStatus status)
    {
        _viewModel.ApprovalRequestStatus = status;
        return this;
    }

    public WhenCallingApprenticeshipApprovalRequestFixture SetChangeApprovalAllowed(bool changeApprovalAllowed)
    {
        _viewModel.ChangeApprovalAllowed = changeApprovalAllowed;
        return this;
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

    public void VerifyApprovalRequestStatusModelStateError(ViewResult viewResult)
    {
        viewResult.Should().NotBeNull();
        viewResult.ViewData.ModelState.Should().ContainSingle(m => m.Key == "ApprovalRequestStatus" && m.Value.Errors.Any(e => e.ErrorMessage == "This change has already been approved."));
    }

    public void VerifyChangeApprovalAllowedModelStateError(ViewResult viewResult)
    {
        viewResult.Should().NotBeNull();
        viewResult.ViewData.ModelState.Should().ContainSingle(m => m.Key == "ChangeApprovalAllowed" && m.Value.Errors.Any(e => e.ErrorMessage == "This change no longer exists"));
    }
}