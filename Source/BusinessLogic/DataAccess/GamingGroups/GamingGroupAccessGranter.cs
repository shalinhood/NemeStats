﻿using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.GamingGroups
{
    public interface GamingGroupAccessGranter
    {
        GamingGroupInvitation CreateInvitation(string email, UserContext userContext);
        GamingGroupInvitation ConsumeInvitation(GamingGroupInvitation gamingGroupInvitation, UserContext userContext);
    }
}