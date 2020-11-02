﻿using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerCommitmentsV2.Web.Models.Apprentice
{
    public class ViewChangesRequest
    {
        [FromRoute]
        public string AccountHashedId { get; set; }

        [FromRoute]
        public string ApprenticeshipHashedId { get; set; }
    }
}
