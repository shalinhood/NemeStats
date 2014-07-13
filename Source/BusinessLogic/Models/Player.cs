﻿using BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class Player : EntityWithTechnicalKey
    {
        public override int Id { get; set; }

        public int GamingGroupId { get; set; }

        [StringLength(255)]
        public string Name { get; set; }
        public bool Active { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual IList<PlayerGameResult> PlayerGameResults { get; set; }
    }
}
