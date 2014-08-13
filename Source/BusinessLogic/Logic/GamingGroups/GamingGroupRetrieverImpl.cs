﻿using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupRetrieverImpl : GamingGroupRetriever
    {
        private DataContext dataContext;

        public GamingGroupRetrieverImpl(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public GamingGroup GetGamingGroupDetails(int gamingGroupId, ApplicationUser currentUser)
        {
            GamingGroup gamingGroup = dataContext.FindById<GamingGroup>(gamingGroupId, currentUser);

            gamingGroup.OwningUser = dataContext.GetQueryable<ApplicationUser>(currentUser)
                .Where(user => user.Id == gamingGroup.OwningUserId)
                .First();

            gamingGroup.GamingGroupInvitations = dataContext.GetQueryable<GamingGroupInvitation>(currentUser)
                .Where(invitation => invitation.GamingGroupId == gamingGroup.Id)
                .ToList();

            SetRegisteredUserInfo(currentUser, gamingGroup);

            return gamingGroup;
        }

        private void SetRegisteredUserInfo(ApplicationUser currentUser, GamingGroup gamingGroup)
        {
            List<string> registeredUserIds = (from gamingGroupInvitation in gamingGroup.GamingGroupInvitations
                                              select gamingGroupInvitation.RegisteredUserId).ToList();

            List<ApplicationUser> registeredUsers = dataContext.GetQueryable<ApplicationUser>(currentUser)
                .Where(user => registeredUserIds.Contains(user.Id))
                .ToList();

            foreach (GamingGroupInvitation gamingGroupInvitation in gamingGroup.GamingGroupInvitations)
            {
                gamingGroupInvitation.RegisteredUser = registeredUsers
                    .Where(user => user.Id == gamingGroupInvitation.RegisteredUserId)
                    .FirstOrDefault();
            }
        }
    }
}
