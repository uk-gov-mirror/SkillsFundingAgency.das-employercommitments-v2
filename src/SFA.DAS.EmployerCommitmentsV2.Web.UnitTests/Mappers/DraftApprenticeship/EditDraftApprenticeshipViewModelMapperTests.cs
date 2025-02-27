﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerCommitmentsV2.Web.Mappers.DraftApprenticeship;
using SFA.DAS.EmployerCommitmentsV2.Web.Models.DraftApprenticeship;
using SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Extensions;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Mappers.DraftApprenticeship
{
    [TestFixture]
    public class EditDraftApprenticeshipViewModelMapperTests
    {
        private EditDraftApprenticeshipViewModelMapper _mapper;
        private EditDraftApprenticeshipRequest _source;
        private EditDraftApprenticeshipViewModel _result;

        private Mock<ICommitmentsApiClient> _commitmentsApiClient;
        private GetDraftApprenticeshipResponse _draftApprenticeshipResponse;
        private Mock<IEncodingService> _encodingService;
        private string _encodedApprenticeshipId;
        private string _cohortReference;
        private GetCohortResponse _cohort;
        private List<TrainingProgramme> _allTrainingProgrammes;
        private List<TrainingProgramme> _standardTrainingProgrammes;

        [SetUp]
        public async Task Arrange()
        {
            var autoFixture = new Fixture();

            _allTrainingProgrammes = autoFixture.CreateMany<TrainingProgramme>().ToList();
            _standardTrainingProgrammes = autoFixture.CreateMany<TrainingProgramme>().ToList();
            _encodedApprenticeshipId = autoFixture.Create<string>();
            _cohortReference = autoFixture.Create<string>();

            _encodingService = new Mock<IEncodingService>();
            _encodingService
                .Setup(x => x.Encode(It.IsAny<long>(), It.Is<EncodingType>(e => e == EncodingType.ApprenticeshipId)))
                .Returns(_encodedApprenticeshipId);
            _encodingService
                .Setup(x => x.Encode(It.IsAny<long>(), It.Is<EncodingType>(e => e == EncodingType.CohortReference)))
                .Returns(_cohortReference);

            _draftApprenticeshipResponse = autoFixture.Create<GetDraftApprenticeshipResponse>();
            _draftApprenticeshipResponse.IsContinuation = false;
            _commitmentsApiClient = new Mock<ICommitmentsApiClient>();
            _commitmentsApiClient.Setup(x =>
                    x.GetDraftApprenticeship(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_draftApprenticeshipResponse);
            _commitmentsApiClient
                .Setup(x => x.GetAllTrainingProgrammeStandards(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAllTrainingProgrammeStandardsResponse
                {
                    TrainingProgrammes = _standardTrainingProgrammes
                });
            
            _commitmentsApiClient
                .Setup(x => x.GetAllTrainingProgrammes(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAllTrainingProgrammesResponse
                {
                    TrainingProgrammes = _allTrainingProgrammes
                });

            _cohort = autoFixture.Create<GetCohortResponse>();
            _cohort.WithParty = Party.Employer;
            _cohort.ChangeOfPartyRequestId = null;
            _commitmentsApiClient.Setup(x => x.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_cohort);

            _source = autoFixture.Create<EditDraftApprenticeshipRequest>();
            _source.Cohort = _cohort;
            _mapper = new EditDraftApprenticeshipViewModelMapper(_commitmentsApiClient.Object, _encodingService.Object);

            _result = await _mapper.Map(TestHelper.Clone(_source)) as EditDraftApprenticeshipViewModel;
        }

        [Test]
        public void DraftApprenticeshipIdIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.Id, _result.DraftApprenticeshipId);
        }

        [Test]
        public void DraftApprenticeshipHashedIdIsMappedCorrectly()
        {
            Assert.AreEqual(_encodedApprenticeshipId, _result.DraftApprenticeshipHashedId);
        }

        [Test]
        public void CohortIdIsMappedCorrectly()
        {
            Assert.AreEqual(_source.Request.CohortId, _result.CohortId);
        }

        [Test]
        public void CohortReferenceIsMappedCorrectly()
        {
            Assert.AreEqual(_cohortReference, _result.CohortReference);
        }

        [Test]
        public void ReservationIdIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.ReservationId, _result.ReservationId);
        }

        [Test]
        public void FirstNameIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.FirstName, _result.FirstName);
        }

        [Test]
        public void LastNameIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.LastName, _result.LastName);
        }

        [Test]
        public void DateOfBirthIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.DateOfBirth, _result.DateOfBirth.Date);
        }

        [Test]
        public void UniqueLearnerNumberIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.Uln, _result.Uln);
        }

        [Test]
        public void CourseCodeIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.CourseCode, _result.CourseCode);
        }

        [Test]
        public void CostIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.Cost, _result.Cost);
        }

        [Test]
        public void StartDateIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.StartDate.ToMonthYearString(), _result.StartDate.Date.ToMonthYearString());
        }

        [Test]
        public void EndDateIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.EndDate.ToMonthYearString(), _result.EndDate.Date.ToMonthYearString());
        }

        [Test]
        public void OriginatorReferenceIsMappedCorrectly()
        {
            Assert.AreEqual(_draftApprenticeshipResponse.Reference, _result.Reference);
        }

        [Test]
        public void AccountHashedIdIsMappedCorrectly()
        {
            Assert.AreEqual(_source.Request.AccountHashedId, _result.AccountHashedId);
        }

        [Test]
        public void ProviderNameIsMappedCorrectly()
        {
            Assert.AreEqual(_cohort.ProviderName, _result.ProviderName);
        }

        [Test]
        public void LegalEntityNameIsMappedCorrectly()
        {
            Assert.AreEqual(_source.Cohort.LegalEntityName, _result.LegalEntityName);
        }

        [TestCase(123, true)]
        [TestCase(null, false)]
        public async Task CoursesAreMappedCorrectlyWhenAccountIsLevy(long? transferSenderId, bool fundedByTransfer)
        {
            _cohort.LevyStatus = ApprenticeshipEmployerType.Levy;
            _cohort.TransferSenderId = transferSenderId;

            _result = await _mapper.Map(_source) as EditDraftApprenticeshipViewModel;

            Assert.AreEqual(fundedByTransfer
                    ? _standardTrainingProgrammes
                    : _allTrainingProgrammes,
                _result.Courses);
        }

        [TestCase(123)]
        [TestCase(null)]
        public async Task CoursesAreMappedCorrectlyWhenAccountIsNonLevy(long? transferSenderId)
        {
            _cohort.LevyStatus = ApprenticeshipEmployerType.NonLevy;
            _cohort.TransferSenderId = transferSenderId;

            _result = await _mapper.Map(_source) as EditDraftApprenticeshipViewModel;

            Assert.AreEqual(_standardTrainingProgrammes, _result.Courses);
        }

        [Test]
        public async Task CoursesAreMappedCorrectlyWhenCohortIsChangeOfParty()
        {
            _cohort.LevyStatus = ApprenticeshipEmployerType.NonLevy;
            _draftApprenticeshipResponse.IsContinuation = true;

            _result = await _mapper.Map(_source) as EditDraftApprenticeshipViewModel;

            Assert.AreEqual(_allTrainingProgrammes, _result.Courses);
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task ThenIsContinuationIsMappedCorrectly(bool isContinuation)
        {
            _draftApprenticeshipResponse.IsContinuation = isContinuation;
            _result = await _mapper.Map(_source) as EditDraftApprenticeshipViewModel;
            Assert.AreEqual(_draftApprenticeshipResponse.IsContinuation, _result.IsContinuation);
        }
    }
}
