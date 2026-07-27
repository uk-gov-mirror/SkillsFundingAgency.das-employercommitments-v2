using SFA.DAS.EmployerCommitmentsV2.Contracts;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice;

public class ApprenticeshipApprovalRequest : IAuthorizationContextModel
{
    [FromRoute]
    public string ApprenticeshipHashedId { get; set; }
    public long ApprenticeshipId { get; set; }
    public string AccountHashedId { get; set; }
    public long AccountId { get; set; }
    public Guid ApprovalRequestId { get; set; }
}