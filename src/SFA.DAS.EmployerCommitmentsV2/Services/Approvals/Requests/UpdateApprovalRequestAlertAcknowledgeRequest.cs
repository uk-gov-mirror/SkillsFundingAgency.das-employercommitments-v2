namespace SFA.DAS.EmployerCommitmentsV2.Services.Approvals.Requests;

public class UpdateApprovalRequestAlertAcknowledgeRequest : ApimSaveDataRequest
{
    public List<UpdateApprovalRequestAlertAcknowledge> ApprovalRequestAlerts { get; set; }
}

public class UpdateApprovalRequestAlertAcknowledge
{
    public Guid ApprovalRequestId { get; set; }
    public DateTime? EmployerAcknowledgedAt { get; set; }
    public string EmployerAcknowledgedBy { get; set; }
}