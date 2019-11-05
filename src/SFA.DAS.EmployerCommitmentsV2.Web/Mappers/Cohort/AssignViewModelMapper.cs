﻿using System.Threading.Tasks;
using SFA.DAS.CommitmentsV2.Shared.Interfaces;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.Cohort;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Mappers.Cohort
{
    public class AssignViewModelMapper : IMapper<AssignRequest, AssignViewModel>
    {
        public Task<AssignViewModel> Map(AssignRequest request)
        {
            return Task.FromResult(new AssignViewModel
            {
                AccountHashedId = request.AccountHashedId,
                AccountLegalEntityHashedId = request.AccountLegalEntityHashedId,
                ReservationId = request.ReservationId,
                StartMonthYear = request.StartMonthYear,
                CourseCode = request.CourseCode,
                ProviderId = request.ProviderId
            });
        }
    }
}