﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using BusinessLogic.Models.Champions;
using BusinessLogic.Models.Players;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public class UniversalTopChampionsRetreiver : Cacheable<int, List<ChampionData>>, IUniversalTopChampionsRetreiver
    {
        private readonly IDataContext _dataContext;

        public UniversalTopChampionsRetreiver(IDateUtilities dateUtilities, ICacheService cacheService, IDataContext dataContext) : base(dateUtilities, cacheService)
        {
            _dataContext = dataContext;
        }


        public override List<ChampionData> GetFromSource(int boardGameGeekGameDefinitionId)
        {
            var topFiveChampions = _dataContext.GetQueryable<Champion>()
                .Where(c => c.GameDefinition.BoardGameGeekGameDefinitionId == boardGameGeekGameDefinitionId && c.GameDefinition.ChampionId == c.Id)
                .Include(c=>c.Player)
                .Include(c=>c.Player.GamingGroup)
                .OrderByDescending(c => c.NumberOfWins)
                .ThenByDescending(c => c.WinPercentage)
                .Take(5)
                .ToList();

            return topFiveChampions.Select(c => new ChampionData()
            {
                NumberOfWins = c.NumberOfWins,
                GameDefinitionId = c.GameDefinitionId,
                NumberOfGames = c.NumberOfGames,
                PlayerId = c.PlayerId,
                WinPercentage = 100*c.NumberOfWins/c.NumberOfGames,
                PlayerName = c.Player.Name,
                PlayerGamingGroupId = c.Player.GamingGroupId,
                PlayerGamingGroupName = c.Player.GamingGroup.Name
            }).ToList();


        }
    }
}