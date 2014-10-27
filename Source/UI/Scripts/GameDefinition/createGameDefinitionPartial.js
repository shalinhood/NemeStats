﻿//Usings
Namespace("Views.GameDefinition");

//Initialization
Views.GameDefinition.CreateGameDefinitionPartial = function () {
    this.$container = null;
    this.$form = null;
    this.$createBtn = null;
    this.$gameNameInput = null;
    this.$gamesTable = null;
    this.onDefinitionCreated = null;
    this.formAction = null;
};

//Implementation
Views.GameDefinition.CreateGameDefinitionPartial.prototype = {
    init: function () {
        var owner = this;
        this.formAction = "/gamedefinition/save";
        this.$container = $(".createGameDefinitionPartial");
        this.$form = this.$container.find("form");
        this.$form.attr('action', this.formAction);
        this.$gameNameInput = this.$form.find("input[type='text']");
        this.$createBtn = this.$form.find("button");
        this.$createBtn.click(function (e) {
            e.preventDefault();
            owner.createGameDefinition();
        });
    },
    createGameDefinition: function () {
        var owner = this;
        if (this.$form.valid()) {
            $.ajax({
                type: "POST",
                url: this.formAction,
                data: this.$form.serialize(),
                success: function (game) {
                    owner.onDefinitionCreated(game);
                    owner.$gameNameInput.val('');
                },
                error: function (err) {
                    alert("Error " + err.status + ":\r\n" + err.statusText);
                },
                dataType: "json"
            });
        }
    }
}