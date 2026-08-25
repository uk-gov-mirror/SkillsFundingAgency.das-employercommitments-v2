using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.EmployerCommitmentsV2.Contracts;
using SFA.DAS.EmployerCommitmentsV2.Services.Approvals.Responses;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice;
using System.Globalization;
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Mappers.Apprentice;

public class ApprenticeshipApprovalRequestToViewModelMapper(IApprovalsApiClient approvalsApiClient) : IMapper<ApprenticeshipApprovalRequest, ApprenticeshipApprovalRequestViewModel>
{
    private const string TNP = "TNP";

    public async Task<ApprenticeshipApprovalRequestViewModel> Map(ApprenticeshipApprovalRequest source)
    {
        var approvalRequest = await approvalsApiClient.GetApprenticeshipApprovalRequest(source.AccountId, source.ApprenticeshipId, source.ApprovalRequestId);
        var apprenticeship = await approvalsApiClient.GetEditApprenticeship(source.AccountId, source.ApprenticeshipId);

        return new ApprenticeshipApprovalRequestViewModel
        {
            ApprenticeshipHashedId = source.ApprenticeshipHashedId,
            AccountHashedId = source.AccountHashedId,
            ApprovalRequestId = source.ApprovalRequestId,
            ApprovalRequestStatus = approvalRequest.ApprovalRequestStatus,
            PriceChangeApprovalAllowed = IsPriceChangeApprovalAllowed(approvalRequest.Items, apprenticeship.Status),

            Items = ConvertToDisplayItems(approvalRequest.Items),

            Name = approvalRequest.Name,
            ULN = approvalRequest.ULN,
            CourseName = approvalRequest.CourseName,
            ProviderName = approvalRequest.ProviderName,
        };
    }

    private ICollection<ApprenticeshipApprovalRequestViewModel.ChangeItem> ConvertToDisplayItems(ICollection<GetApprenticeshipApprovalResponse.ChangeItem> items)
    {
        var displayItems = new List<ApprenticeshipApprovalRequestViewModel.ChangeItem>();
        foreach (var item in items)
        {
            if(item.FieldName == "TNP1")
            {
                displayItems.Add(new ApprenticeshipApprovalRequestViewModel.ChangeItem
                {
                    FieldName = "Training price (TNP1)",
                    OldValue = ToCurrency(item.OldValue),
                    NewValue = ToCurrency(item.NewValue),
                    EffectiveFromDate = item.EffectiveFromDate
                });
            }
            else if (item.FieldName == "TNP2")
            {
                displayItems.Add(new ApprenticeshipApprovalRequestViewModel.ChangeItem
                {
                    FieldName = "Assessment price (TNP2)",
                    OldValue = ToCurrency(item.OldValue),
                    NewValue = ToCurrency(item.NewValue),
                    EffectiveFromDate = item.EffectiveFromDate
                });

            }
            else
            {
                displayItems.Add(new ApprenticeshipApprovalRequestViewModel.ChangeItem
                {
                    FieldName = item.FieldName,
                    OldValue = item.OldValue,
                    NewValue = item.NewValue,
                });
            }
        }

        return displayItems;
    }

    private bool IsPriceChangeApprovalAllowed(ICollection<GetApprenticeshipApprovalResponse.ChangeItem> items, ApprenticeshipStatus currentStatus)
    {
        var result = false;
        var hasPriceChangeItem = items.Any(i => i.FieldName.Contains(TNP));
        
        if (hasPriceChangeItem 
            && (currentStatus == ApprenticeshipStatus.Live 
                || currentStatus == ApprenticeshipStatus.Paused 
                || currentStatus == ApprenticeshipStatus.WaitingToStart))
        {
            result = true;
        }

        return result;
    }

    public static string ToCurrency(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "#error#";

        var culture = new CultureInfo("en-GB");

        if (decimal.TryParse(input, NumberStyles.Any, culture, out decimal value))
        {
            return value.ToString("C0", culture);
        }

        return "#error#";
    }
}