﻿using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.GamingGroupsTests.EntityFrameworkGamingGroupAccessGranterTests
{
    [TestFixture]
    public class EntityFrameworkGamingGroupAccessGranterTestBase
    {
        protected NemeStatsDbContext dbContextMock;
        protected GamingGroupInvitationRepository gamingGroupInvitationRepositoryMock;
        protected GamingGroupAccessGranter gamingGroupAccessGranter;
        protected UserContext userContext;

        [SetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            userContext = new UserContext()
            {
                ApplicationUserId = "user id",
                GamingGroupId = 777
            };

            gamingGroupInvitationRepositoryMock = MockRepository.GenerateMock<GamingGroupInvitationRepository>();
            gamingGroupAccessGranter = new EntityFrameworkGamingGroupAccessGranter(dbContextMock, gamingGroupInvitationRepositoryMock);
        }
    }
}
