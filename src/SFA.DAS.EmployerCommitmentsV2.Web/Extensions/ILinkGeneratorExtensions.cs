using SFA.DAS.EmployerUrlHelper;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Extensions
{
    public static class ILinkGeneratorExtensions
    {
       public static string YourOrganisationsAndAgreements(this ILinkGenerator linkGenerator, string accountHashedId)
        {
            return linkGenerator.AccountsLink($"accounts/{accountHashedId}/agreements");
        }

        public static string PayeSchemes(this ILinkGenerator linkGenerator, string accountHashedId)
        {
            return linkGenerator.AccountsLink($"accounts/{accountHashedId}/schemes");
        }

        public static string ApprenticeDetails(
            this ILinkGenerator linkGenerator,
            string accountHashedId,
            string hashedApprenticeshipId)
        {
            return linkGenerator.CommitmentsLink($"accounts/{accountHashedId}/apprentices/manage/{hashedApprenticeshipId}/details");
        }

        public static string WhenToApplyStopApprentice(
           this ILinkGenerator linkGenerator,
           string accountHashedId,
           string hashedApprenticeshipId)
        {
            return linkGenerator.CommitmentsLink($"accounts/{accountHashedId}/apprentices/manage/{hashedApprenticeshipId}/details/statuschange/stop/whentoapply");
        }

        public static string ViewApprentice(this ILinkGenerator linkGenerator, 
            string accountHashedId, 
            string cohortReference, 
            string draftApprenticeshipHashedId)
        {
            return linkGenerator.CommitmentsLink($"accounts/{accountHashedId}/apprentices/{cohortReference}/apprenticeships/{draftApprenticeshipHashedId}/view");
        }

        public static string DeleteApprentice(this ILinkGenerator linkGenerator,
            string accountHashedId,
            string cohortReference,
            string draftApprenticeshipHashedId)
        {
            return linkGenerator.CommitmentsLink($"accounts/{accountHashedId}/apprentices/{cohortReference}/apprenticeships/{draftApprenticeshipHashedId}/delete");
        }

        public static string EmployerHome(this ILinkGenerator linkGenerator, string accountHashedId)
        {
            return linkGenerator.AccountsLink($"accounts/{accountHashedId}/teams");
        }

        public static string EmployerAccountsHome(this ILinkGenerator linkGenerator)
        {
            return linkGenerator.AccountsLink();
        }

        public static string HasLearnerBeenMadeRedundant(this ILinkGenerator linkGenerator,
        string hashedApprenticeshipId,
        string changeType)
        {
            return linkGenerator.CommitmentsLink($"{hashedApprenticeshipId}/details/statuschange/{changeType}/maderedundant");
        }

    }
}