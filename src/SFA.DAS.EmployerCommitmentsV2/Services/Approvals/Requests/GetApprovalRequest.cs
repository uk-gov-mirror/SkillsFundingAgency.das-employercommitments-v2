namespace SFA.DAS.EmployerCommitmentsV2.Services.Approvals.Requests;

public class GetApprovalRequestAlertRequest(long apprenticeshipId)
{
    public long ApprenticeshipId { get; set; } = apprenticeshipId;

    public byte Status { get; set; } = 1;

    public string GetUrl => $"approvalrequest/apprenticeships/{ApprenticeshipId}?status={Status}";
}