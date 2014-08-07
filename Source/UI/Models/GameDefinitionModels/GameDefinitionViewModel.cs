﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.PlayedGame;

namespace UI.Models.GameDefinitionModels
{
    public class GameDefinitionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<PlayedGameDetailsViewModel> PlayedGames { get; set; }
    }
}