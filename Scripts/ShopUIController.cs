using Godot;
using System;
using System.Linq;

public partial class ShopUIController : Control
{
	[Export] public PackedScene ShopItemPrefab;

	[Export] public Control ShopItemsContainer;

	[Export] public PackedScene UpgradeItemPrefab;

	[Export] public Control UpgradeItemsContainer;

	public override void _Ready()
	{
		PrepItems();
	}

	private void PrepItems()
	{
		bool previousItemUnlocked = RecipeManager.Instance.Recipes.First().IsUnlocked;

		foreach (var recipe in RecipeManager.Instance.Recipes.Where(x => x.IsShopItem))
		{
			var shopItem = ShopItemPrefab.Instantiate<ShopItemController>();
			shopItem.SetRecipe(recipe, previousItemUnlocked);
			previousItemUnlocked = recipe.IsUnlocked;
			ShopItemsContainer.AddChild(shopItem);
		}

		foreach (var gameUpgrade in UpgradesManager.Instance.Upgrades)
		{
			var upgradeItem = ShopItemPrefab.Instantiate<ShopItemController>();

			var requiredRecipe = gameUpgrade.RequiredRecipe;

			var recipe = RecipeManager.Instance.Recipes.First(x => x.ResultName == requiredRecipe);

			if (recipe.IsUnlocked)
			{
				requiredRecipe = string.Empty;
			}

			upgradeItem.SetUpgrade(gameUpgrade, requiredRecipe);
			UpgradeItemsContainer.AddChild(upgradeItem);
		}
	}
}
