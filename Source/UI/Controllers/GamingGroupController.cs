﻿using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UI.Filters;
using UI.Models.GamingGroup;
using UI.Transformations;

namespace UI.Controllers
{
    [Authorize]
    public partial class GamingGroupController : Controller
    {
        internal DataContext dataContext;
        internal GamingGroupToGamingGroupViewModelTransformation gamingGroupToGamingGroupViewModelTransformation;
        internal GamingGroupAccessGranter gamingGroupAccessGranter;
        internal GamingGroupCreator gamingGroupCreator;
        internal GamingGroupRetriever gamingGroupRetriever;

        public GamingGroupController(
            DataContext dataContext,
            GamingGroupToGamingGroupViewModelTransformation gamingGroupToGamingGroupViewModelTransformation,
            GamingGroupAccessGranter gamingGroupAccessGranter,
            GamingGroupCreator gamingGroupCreator,
            GamingGroupRetriever gamingGroupRetriever)
        {
            this.dataContext = dataContext;
            this.gamingGroupToGamingGroupViewModelTransformation = gamingGroupToGamingGroupViewModelTransformation;
            this.gamingGroupAccessGranter = gamingGroupAccessGranter;
            this.gamingGroupCreator = gamingGroupCreator;
            this.gamingGroupRetriever = gamingGroupRetriever;
        }

        //
        // GET: /GamingGroup
        [UserContextAttribute]
        public virtual ActionResult Index(ApplicationUser currentUser)
        {
            GamingGroup gamingGroup = gamingGroupRetriever.GetGamingGroupDetails(currentUser.CurrentGamingGroupId.Value, currentUser);
            GamingGroupViewModel viewModel = gamingGroupToGamingGroupViewModelTransformation.Build(gamingGroup);

            return View(MVC.GamingGroup.Views.Index, viewModel);
        }

        [HttpGet]
        [UserContextAttribute(RequiresGamingGroup = false)]
        public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [UserContextAttribute(RequiresGamingGroup = false)]
        public async virtual Task<ActionResult> Create(string gamingGroupName, ApplicationUser currentUser)
        {
            await gamingGroupCreator.CreateGamingGroupAsync(gamingGroupName, currentUser);
            return RedirectToAction(MVC.GamingGroup.ActionNames.Index);
        }

        //
        // POST: /GamingGroup/Delete/5
        [HttpPost]
        [UserContextAttribute]
        public virtual ActionResult GrantAccess(string inviteeEmail, ApplicationUser currentUser)
        {
            gamingGroupAccessGranter.CreateInvitation(inviteeEmail, currentUser);
            return RedirectToAction(MVC.GamingGroup.ActionNames.Index);
        }
    }
}
