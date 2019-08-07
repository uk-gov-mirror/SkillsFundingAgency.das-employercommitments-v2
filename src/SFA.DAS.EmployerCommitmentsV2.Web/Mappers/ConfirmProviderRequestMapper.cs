﻿using SFA.DAS.Commitments.Shared.Interfaces;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.CreateCohort;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Mappers
{
    public class ConfirmProviderRequestMapper : IMapper<SelectProviderViewModel, ConfirmProviderRequest>
    {
        public ConfirmProviderRequest Map(SelectProviderViewModel source)
        {
            return new ConfirmProviderRequest
            {
                AccountHashedId = source.AccountHashedId,
                CourseCode = source.CourseCode,
                AccountLegalEntityHashedId = source.AccountLegalEntityHashedId,
                ProviderId = long.Parse(source.ProviderId),
                ReservationId = source.ReservationId,
                StartMonthYear = source.StartMonthYear
            };
        }
    }
}
