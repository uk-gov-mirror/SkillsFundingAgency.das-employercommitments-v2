﻿using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Shared.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerCommitmentsV2.Web.Exceptions;
using SFA.DAS.EmployerCommitmentsV2.Web.Mappers.DraftApprenticeship;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.DraftApprenticeship;
using SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Shared;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Mappers.DraftApprenticeship
{
    [TestFixture]
    public class AddDraftApprenticeshipViewModelMapperTests
    {
        private AddDraftApprenticeshipViewModelMapper _mapper;
        private AddDraftApprenticeshipRequest _source;
        private AddDraftApprenticeshipViewModel _result;

        private Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private GetCohortResponse _cohort;

        private TrainingProgrammeApiClientMock _trainingProgrammeApiClient;

        [SetUp]
        public async Task Arrange()
        {
            var autoFixture = new Fixture();

            _cohort = autoFixture.Create<GetCohortResponse>();
            _cohort.WithParty = Party.Employer;
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cohort);

            _trainingProgrammeApiClient = new TrainingProgrammeApiClientMock();

            _mapper = new AddDraftApprenticeshipViewModelMapper(_commitmentsApiClient.Object, _trainingProgrammeApiClient.Object);

            _source = autoFixture.Create<AddDraftApprenticeshipRequest>();
            _source.StartMonthYear = "092020";

            _result = await _mapper.Map(TestHelper.Clone(_source));
        }

        [Test]
        public void CourseCodeIsMappedCorrectly()
        {
            Assert.AreEqual(_source.CourseCode, _result.CourseCode);
        }

        [Test]
        public void StartDateIsMappedCorrectly()
        {
            Assert.AreEqual(new MonthYearModel(_source.StartMonthYear).Date, _result.StartDate.Date);
        }

        [Test]
        public void AccountHashedIdIsMappedCorrectly()
        {
            Assert.AreEqual(_source.AccountHashedId, _result.AccountHashedId);
        }

        [Test]
        public void AccountLegalEntityHashedIdIsMappedCorrectly()
        {
            Assert.AreEqual(_source.AccountLegalEntityHashedId, _result.AccountLegalEntityHashedId);
        }

        [Test]
        public void AccountLegalEntityIdIsMappedCorrectly()
        {
            Assert.AreEqual(_source.AccountLegalEntityId, _result.AccountLegalEntityId);
        }

        [Test]
        public void CohortIdIsMappedCorrectly()
        {
            Assert.AreEqual(_source.CohortId, _result.CohortId);
        }

        [Test]
        public void CohortReferenceIsMappedCorrectly()
        {
            Assert.AreEqual(_source.CohortReference, _result.CohortReference);
        }

        [Test]
        public void ReservationIdIsMappedCorrectly()
        {
            Assert.AreEqual(_source.ReservationId, _result.ReservationId);
        }

        [Test]
        public void ProviderNameIsMappedCorrectly()
        {
            Assert.AreEqual(_cohort.ProviderName, _result.ProviderName);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task CoursesAreMappedCorrectly(bool fundedByTransfer)
        {
            _cohort.IsFundedByTransfer = fundedByTransfer;

            _result = await _mapper.Map(_source);

            Assert.AreEqual(fundedByTransfer
                    ?  _trainingProgrammeApiClient.Standards
                    : _trainingProgrammeApiClient.All,
                _result.Courses);
        }

        [Test]
        public void ThrowsWhenCohortNotWithEditingParty()
        {
            _cohort.WithParty = Party.Provider;
            Assert.ThrowsAsync<CohortEmployerUpdateDeniedException>(() => _mapper.Map(_source));
        }
    }
}
