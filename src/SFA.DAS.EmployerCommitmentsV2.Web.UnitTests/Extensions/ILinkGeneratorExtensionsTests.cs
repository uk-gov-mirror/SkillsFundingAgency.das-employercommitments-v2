﻿using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerCommitmentsV2.Web.Extensions;
using SFA.DAS.EmployerUrlHelper;

namespace SFA.DAS.EmployerCommitmentsV2.Web.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable]
    public class ILinkGeneratorExtensionsTests
    {
        private ILinkGeneratorExtensionsTestsFixture _fixture;
        [SetUp]
        public void Arrange()
        {
            _fixture = new ILinkGeneratorExtensionsTestsFixture();
        }

        [Test, AutoData]
        public void YourOrganisationsAndAgreements_BuildsPathCorrectly(string accountHashedId)
        {
            var url = _fixture.Sut.YourOrganisationsAndAgreements(accountHashedId);

            Assert.AreEqual($"{_fixture.AccountsLink}accounts/{accountHashedId}/agreements", url);
        }

        [Test, AutoData]
        public void PayeSchemes_BuildsPathCorrectly(string accountHashedId)
        {
            var url = _fixture.Sut.PayeSchemes(accountHashedId);

            Assert.AreEqual($"{_fixture.AccountsLink}accounts/{accountHashedId}/schemes", url);
        }

        [Test, AutoData]
        public void ViewApprentice_BuildsPathCorrectly(string accountHashedId, string cohortReference, string draftApprenticeshipHashedId)
        {
            var url = _fixture.Sut.ViewApprentice(accountHashedId, cohortReference, draftApprenticeshipHashedId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/{cohortReference}/apprenticeships/{draftApprenticeshipHashedId}/view", url);
        }

        [Test, AutoData]
        public void DeleteApprentice_BuildsPathCorrectly(string accountHashedId, string cohortReference, string draftApprenticeshipHashedId)
        {
            var url = _fixture.Sut.DeleteApprentice(accountHashedId, cohortReference, draftApprenticeshipHashedId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/{cohortReference}/apprenticeships/{draftApprenticeshipHashedId}/delete", url);
        }

        [Test, AutoData]
        public void EmployerHome_BuildsPathCorrectly(string accountHashedId)
        {
            var url = _fixture.Sut.EmployerHome(accountHashedId);
            Assert.AreEqual($"{_fixture.AccountsLink}accounts/{accountHashedId}/teams", url);
        }

        [Test, AutoData]
        public void ApprenticeDetails_BuildsPathCorrectly(
            string accountHashedId,
            string hashedApprenticeshipId)
        {
            var url = _fixture.Sut.ApprenticeDetails(accountHashedId, hashedApprenticeshipId);
            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/manage/{hashedApprenticeshipId}/details", url);
        }

        [Test, AutoData]
        public void ApprenticeReviewChanges_BuildsPathCorrectly(string accountHashedId, string hashedApprenticeshipId)
        {            
            var url = _fixture.Sut.ReviewChanges(accountHashedId, hashedApprenticeshipId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/manage/{hashedApprenticeshipId}/changes/review", url);
        }

        [Test, AutoData]
        public void ApprenticeEditStopDate_BuildsPathCorrectly(string accountHashedId, string hashedApprenticeshipId)
        {
            var url = _fixture.Sut.EditStopDate(accountHashedId, hashedApprenticeshipId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/manage/{hashedApprenticeshipId}/details/editstopdate", url);
        }

        [Test, AutoData]
        public void ApprenticeViewChanges_BuildsPathCorrectly(string accountHashedId, string hashedApprenticeshipId)
        {
            var url = _fixture.Sut.ViewChanges(accountHashedId, hashedApprenticeshipId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/manage/{hashedApprenticeshipId}/changes/view", url);
        }   

        [Test, AutoData]
        public void EditApprentice_BuildsPathCorrectly(string accountHashedId, string hashedApprenticeshipId)
        {
            var url = _fixture.Sut.EditApprenticeship(accountHashedId, hashedApprenticeshipId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/manage/{hashedApprenticeshipId}/edit", url);
        }     

        [Test, AutoData]
        public void DatalockRestart_BuildsPathCorrectly(string accountHashedId, string hashedApprenticeshipId)
        {
            var url = _fixture.Sut.DatalockRestart(accountHashedId, hashedApprenticeshipId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/manage/{hashedApprenticeshipId}/datalock/restart", url);
        }

        [Test, AutoData]
        public void DatalockChanges_BuildsPathCorrectly(string accountHashedId, string hashedApprenticeshipId)
        {
            var url = _fixture.Sut.DatalockChanges(accountHashedId, hashedApprenticeshipId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/manage/{hashedApprenticeshipId}/datalock/changes", url);
        }

        [Test, AutoData]
        public void SelectTransferConnection_BuildsPathCorrectly(string accountHashedId)
        {
            var url = _fixture.Sut.SelectTransferConnection(accountHashedId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/transferConnection/create", url);
        }

        [Test, AutoData]
        public void SelectLegalEntity_BuildsPathCorrectly(string accountHashedId)
        {
            var url = _fixture.Sut.SelectLegalEntity(accountHashedId);

            Assert.AreEqual($"{_fixture.CommitmentsLink}accounts/{accountHashedId}/apprentices/legalEntity/create", url);
        }
    }

    public class ILinkGeneratorExtensionsTestsFixture
    {
        public Mock<ILinkGenerator> MockILinkGenerator;
        public ILinkGenerator Sut => MockILinkGenerator.Object;
        public string AccountsLink => "https://accounts.com/";
        public string CommitmentsLink => "https://commitments.com/";
        public string UsersLink => "https://users.com/";

        public ILinkGeneratorExtensionsTestsFixture()
        {
            MockILinkGenerator = new Mock<ILinkGenerator>();
            MockILinkGenerator.Setup(x => x.AccountsLink(It.IsAny<string>())).Returns((string path) => AccountsLink + path);
            MockILinkGenerator.Setup(x => x.CommitmentsLink(It.IsAny<string>())).Returns((string path) => CommitmentsLink + path);
            MockILinkGenerator.Setup(x => x.UsersLink(It.IsAny<string>())).Returns((string path) => UsersLink + path);
        }
    }
}