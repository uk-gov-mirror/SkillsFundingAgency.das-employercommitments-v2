using SFA.DAS.EmployerCommitmentsV2.Contracts;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice;

public class ApprenticeshipApprovalRequestAlertsRequest : IAuthorizationContextModel
{
    [FromRoute]
    public string ApprenticeshipHashedId { get; set; }

    public long ApprenticeshipId { get; set; }

    [FromRoute]
    public string AccountHashedId { get; set; }
}