﻿using System.Net.Http;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using StructureMap.AutoMocking;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.HtmlHelpers;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayersControllerTests
{
    [TestFixture]
    public class SaveNewPlayerTests : ApiControllerTestBase<PlayersController>
    {
        private Player _expectedPlayer;
        private readonly int _expectedGamingGroupId = 2;

        [SetUp]
        public void SetUp()
        {
            _expectedPlayer = new Player
            {
                Id = 1,
                GamingGroupId = _expectedGamingGroupId
            };
            _autoMocker.Get<IPlayerSaver>().Expect(mock => mock.CreatePlayer(
                Arg<CreatePlayerRequest>.Is.Anything, 
                Arg<ApplicationUser>.Is.Anything,
                Arg<bool>.Is.Anything)).IgnoreArguments().Return(_expectedPlayer);
        }

        [Test]
        public void ItSavesTheNewPlayer()
        {
            var newPlayerMessage = new NewPlayerMessage
            {
                PlayerName = "some player name",
                GamingGroupId = _expectedGamingGroupId,
                PlayerEmailAddress = "some email address"
            };

            _autoMocker.ClassUnderTest.SaveNewPlayer(newPlayerMessage, _expectedGamingGroupId);

            var args = _autoMocker.Get<IPlayerSaver>().GetArgumentsForCallsMadeOn(
                mock => mock.CreatePlayer(Arg<CreatePlayerRequest>.Is.Anything, Arg<ApplicationUser>.Is.Anything,
                    Arg<bool>.Is.Anything));
            var firstCall = args.AssertFirstCallIsType<CreatePlayerRequest>();
            firstCall.Name.ShouldBe(newPlayerMessage.PlayerName);
            firstCall.GamingGroupId.ShouldBe(newPlayerMessage.GamingGroupId);
            firstCall.PlayerEmailAddress.ShouldBe(newPlayerMessage.PlayerEmailAddress);
        }

        [Test]
        public void ItSavesTheNewPlayerUsingTheGamingGroupIdOnTheRequestInsteadOfFromTheUri ()
        {
            var newPlayerMessage = new NewPlayerMessage
            {
                PlayerName = "some player name",
                GamingGroupId = _expectedGamingGroupId
            };
            int someGamingGroupIdThatWontGetUsed = -100;

            _autoMocker.ClassUnderTest.SaveNewPlayer(newPlayerMessage, someGamingGroupIdThatWontGetUsed);

            _autoMocker.Get<IPlayerSaver>().AssertWasCalled(
                mock => mock.CreatePlayer(Arg<CreatePlayerRequest>.Matches(player => player.Name == newPlayerMessage.PlayerName
                && player.GamingGroupId == _expectedGamingGroupId),
                    Arg<ApplicationUser>.Is.Anything,
                    Arg<bool>.Is.Anything));
        }

        [Test]
        public void ItReturnsThePlayerIdAndGamingGroupOfTheNewlyCreatedPlayer()
        {
            var expectedNemeStatsUrl = AbsoluteUrlBuilder.GetPlayerDetailsUrl(_expectedPlayer.Id);

            var actualResponse = _autoMocker.ClassUnderTest.SaveNewPlayer(new NewPlayerMessage(), _expectedGamingGroupId);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewlyCreatedPlayerMessage>)));
            var content = actualResponse.Content as ObjectContent<NewlyCreatedPlayerMessage>;
            var newlyCreatedPlayerMessage = content.Value as NewlyCreatedPlayerMessage;
            Assert.That(newlyCreatedPlayerMessage.PlayerId, Is.EqualTo(_expectedPlayer.Id));
            Assert.That(newlyCreatedPlayerMessage.GamingGroupId, Is.EqualTo(_expectedGamingGroupId));
            Assert.That(newlyCreatedPlayerMessage.NemeStatsUrl, Is.EqualTo(expectedNemeStatsUrl));
        }

        [Test]
        public void It_Throws_Returns_A_SubErrorCode_Of_1_If_The_Player_Already_Exists()
        {
            //--arrange
            _autoMocker = new RhinoAutoMocker<PlayersController>();
            _autoMocker.Get<IPlayerSaver>().Expect(mock =>
                    mock.CreatePlayer(Arg<CreatePlayerRequest>.Is.Anything, Arg<ApplicationUser>.Is.Anything, Arg<bool>.Is.Anything))
                .Throw(new PlayerAlreadyExistsException("some name", 1));

            //--act
            var exception = Assert.Throws<PlayerAlreadyExistsException>(() => _autoMocker.ClassUnderTest.SaveNewPlayer(new NewPlayerMessage(), _expectedGamingGroupId));

            //--assert
            exception.ErrorSubCode.ShouldBe(1);
        }

        [Test]
        public void It_Throws_Returns_A_SubErrorCode_Of_2_If_Another_User_Already_Has_This_Email()
        {
            //--arrange
            _autoMocker = new RhinoAutoMocker<PlayersController>();
            _autoMocker.Get<IPlayerSaver>().Expect(mock =>
                    mock.CreatePlayer(Arg<CreatePlayerRequest>.Is.Anything, Arg<ApplicationUser>.Is.Anything, Arg<bool>.Is.Anything))
                .Throw(new PlayerWithThisEmailAlreadyExistsException("email", "some name", 1));

            //--act
            var exception = Assert.Throws<PlayerWithThisEmailAlreadyExistsException>(() => _autoMocker.ClassUnderTest.SaveNewPlayer(new NewPlayerMessage(), _expectedGamingGroupId));

            //--assert
            exception.ErrorSubCode.ShouldBe(2);
        }
    }
}
