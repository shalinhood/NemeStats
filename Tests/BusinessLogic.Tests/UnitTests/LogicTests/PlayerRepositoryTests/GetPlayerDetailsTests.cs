﻿using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerDetailsTests
    {
        private NemeStatsDbContext dbContextMock;
        private PlayerRepository playerRepositoryPartialMock;
        private Player player;
        private int numberOfRecentGames = 1;
        private Nemesis nemesis;
        private UserContext userContext;

        [SetUp]
        public void SetUp()
        {
            userContext = new UserContext()
            {
                ApplicationUserId = "123",
                GamingGroupId = 15151
            };
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playerRepositoryPartialMock = MockRepository.GeneratePartialMock<PlayerRepository>(dbContextMock);
            player = new Player()
            {
                Id = 1351,
                Name = "the player",
                PlayerGameResults = new List<PlayerGameResult>(),
                Active = true
            };

            playerRepositoryPartialMock.Expect(repo => repo.GetPlayer(player.Id, userContext))
                .Repeat.Once()
                .Return(player);

            PlayerStatistics playerStatistics = new PlayerStatistics();
            playerRepositoryPartialMock.Expect(repo => repo.GetPlayerStatistics(player.Id, userContext))
                .Repeat.Once()
                .Return(playerStatistics);
            
            nemesis = new Nemesis()
            {
                NemesisPlayerId = 151541
            };
            playerRepositoryPartialMock.Expect(mock => mock.GetNemesis(player.Id, userContext))
                .Repeat.Once()
                .Return(nemesis);

            playerRepositoryPartialMock.Expect(mock => mock.GetPlayerGameResultsWithPlayedGameAndGameDefinition(player.Id, numberOfRecentGames, userContext))
                            .Repeat.Once()
                            .Return(player.PlayerGameResults.ToList());
        }

        [Test]
        public void ItThrowsArgumentExceptionIfThePlayerDoesntExist()
        {
            int playerId = 1;
            playerRepositoryPartialMock.Expect(mock => mock.GetPlayer(playerId, userContext))
                .Repeat.Once()
                .Return(null);

            var exception = Assert.Throws<ArgumentException>(() =>
                    playerRepositoryPartialMock.GetPlayerDetails(playerId, numberOfRecentGames, userContext)
                );

            Assert.AreEqual(PlayerRepository.EXCEPTION_PLAYER_NOT_FOUND, exception.Message);
        }

        //TODO need tests for the transformation... which should probably be refactored into a different class

        [Test]
        public void ItGetsThePlayersNemesis()
        {
            PlayerDetails playerDetails = playerRepositoryPartialMock.GetPlayerDetails(player.Id, numberOfRecentGames, userContext);

            Assert.AreEqual(nemesis, playerDetails.Nemesis);
        }

        [Test]
        public void ItThrowsAnUnauthorizedExceptionIfTheUserDoesntHaveAccessToThePlayer()
        {
            playerRepositoryPartialMock = MockRepository.GeneratePartialMock<PlayerRepository>(dbContextMock);
            playerRepositoryPartialMock.Expect(partialMock => partialMock.GetPlayer(player.Id, userContext))
                .Throw(new UnauthorizedAccessException());

            Assert.Throws<UnauthorizedAccessException>(() => playerRepositoryPartialMock.GetPlayerDetails(player.Id, 0, userContext));
        }
    }
}
