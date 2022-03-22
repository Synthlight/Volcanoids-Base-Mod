using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

namespace Base_Mod {
    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    public class RecipeBuilder {
        private GUID             guid;
        private string           name;
        private InventoryItem[]  inputs;
        private InventoryItem    output;
        private ItemDefinition[] requiredUpgrades;
        private RecipeCategory[] categories;
        private Sprite           icon;
        private int              order;
        private float            prodTime;

        public static RecipeBuilder FromRecipe(Recipe recipe) {
            return new RecipeBuilder {
                guid             = recipe.AssetId,
                name             = recipe.name,
                inputs           = recipe.Inputs.ToArray(),
                output           = recipe.Output,
                requiredUpgrades = recipe.RequiredUpgrades.ToArray(),
                categories       = recipe.Categories.ToArray(),
                icon             = recipe.Icon,
                order            = recipe.Order,
                prodTime         = recipe.ProductionTime
            };
        }

        public RecipeBuilder SetGuid(string guid) {
            SetGuid(GUID.Parse(guid));
            return this;
        }

        public RecipeBuilder SetGuid(GUID guid) {
            this.guid = guid;
            return this;
        }

        public RecipeBuilder SetName(string name) {
            this.name = name;
            return this;
        }

        public RecipeBuilder UpdateInputItems(params ItemDefinition[] inputItems) {
            for (var i = inputItems.Length - 1; i >= 0; i--) {
                inputs[i].Item = inputItems[i];
            }
            return this;
        }

        public RecipeBuilder SetInputs(IEnumerable<InventoryItem> inputs) {
            this.inputs = inputs.ToArray();
            return this;
        }

        public RecipeBuilder SetOutput(InventoryItem output) {
            this.output = output;
            return this;
        }

        public RecipeBuilder SetRequiredUpgrades(IEnumerable<ItemDefinition> requiredUpgrades) {
            this.requiredUpgrades = requiredUpgrades.ToArray();
            return this;
        }

        public RecipeBuilder SetCategories(IEnumerable<RecipeCategory> categories) {
            this.categories = categories.ToArray();
            return this;
        }

        public RecipeBuilder SetIcon(Sprite icon) {
            this.icon = icon;
            return this;
        }

        public RecipeBuilder SetOrder(int order) {
            this.order = order;
            return this;
        }

        public RecipeBuilder SetProdTime(float prodTime) {
            this.prodTime = prodTime;
            return this;
        }

        public Recipe Build() {
            var recipe = Definition.Create<Recipe>(guid);
            recipe.name             = name;
            recipe.Inputs           = inputs.ToArray();
            recipe.Output           = output;
            recipe.RequiredUpgrades = requiredUpgrades.ToArray();
            recipe.Categories       = categories.ToArray();
            recipe.Icon             = icon;
            recipe.Order            = order;
            recipe.ProductionTime   = prodTime;

            return recipe;
        }
    }
}