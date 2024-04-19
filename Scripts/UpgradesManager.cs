using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradesManager : Node
{
	public static UpgradesManager Instance;

	public List<GameUpgrade> Upgrades = new();

	public override void _Ready()
	{
		Instance = this;

		Upgrades.Add(new GameUpgrade()
		{
			Name = "Height +5%",
			Icon = ResourceLoader.Load<Texture2D>("res://UpgradeImages/BowlHeight.png"),
			MaxTiers = 10,
			CurrentTier = 0,
			TierCosts = new[] { 0, 100, 200, 300, 400, 500, 600, 800, 1000, 1200, 2000 },
			ApplyUpgrade = ApplyHeightUpgrade,
			RequiredRecipe = "Pineapple",
		});

		Upgrades.Add(new GameUpgrade()
		{
			Name = "Width +5%",
			Icon = ResourceLoader.Load<Texture2D>("res://UpgradeImages/BowlWidth.png"),
			MaxTiers = 5,
			CurrentTier = 0,
			TierCosts = new[] { 0, 100, 200, 400, 800, 1600 },
			ApplyUpgrade = ApplyWidthUpgrade,
			RequiredRecipe = "Pineapple"
		});

		Upgrades.Add(new GameUpgrade()
		{
			Name = "Start with Pineapple",
			Icon = ResourceLoader.Load<Texture2D>("res://UpgradeImages/Pineapple.png"),
			MaxTiers = 1,
			CurrentTier = 0,
			TierCosts = new[] { 0, 200 },
			ApplyUpgrade = ApplyPineappleUpgrade,
			RequiredRecipe = "Pineapple",
		});

		Upgrades.Add(new GameUpgrade()
		{
			Name = "Start with Coconut",
			Icon = ResourceLoader.Load<Texture2D>("res://UpgradeImages/Coconut.png"),
			MaxTiers = 1,
			CurrentTier = 0,
			TierCosts = new[] { 0, 250 },
			ApplyUpgrade = ApplyCoconutUpgrade,
			RequiredRecipe = "Coconut"
		});

		Upgrades.Add(new GameUpgrade()
		{
			Name = "Start with Cabbage",
			Icon = ResourceLoader.Load<Texture2D>("res://UpgradeImages/Cabbage.png"),
			MaxTiers = 1,
			CurrentTier = 0,
			TierCosts = new[] { 0, 400 },
			ApplyUpgrade = ApplyCabbageUpgrade,
			RequiredRecipe = "Cabbage",
		});

		Upgrades.Add(new GameUpgrade()
		{
			Name = "Start with Pumpkin",
			Icon = ResourceLoader.Load<Texture2D>("res://UpgradeImages/Pumpkin.png"),
			MaxTiers = 1,
			CurrentTier = 0,
			TierCosts = new[] { 0, 500 },
			ApplyUpgrade = ApplyPumpkinUpgrade,
			RequiredRecipe = "Pumpkin",
		});

		Upgrades.Add(new GameUpgrade()
		{
			Name = "Extra Lives",
			Icon = ResourceLoader.Load<Texture2D>("res://UpgradeImages/ExtraLife.png"),
			MaxTiers = 5,
			CurrentTier = 0,
			TierCosts = new[] { 0, 100, 500, 500, 500, 500 },
			ApplyUpgrade = ApplyHealthUpgrade,
			RequiredRecipe = "Pineapple",
		});

		Upgrades.Add(new GameUpgrade()
		{
			Name = "Remove Shrooms",
			Icon = ResourceLoader.Load<Texture2D>("res://UpgradeImages/Shroom.png"),
			MaxTiers = 1,
			CurrentTier = 0,
			TierCosts = new[] { 0, 1000 },
			ApplyUpgrade = ApplyShroomRemoverUpgrade,
			RequiredRecipe = "Pineapple",
		});
	}

	public void ApplyShroomRemoverUpgrade(GameUpgrade upgrade, Gameplay gameplay)
	{
		if (upgrade.CurrentTier == 0) return;

		gameplay.UsableComponents = gameplay.UsableComponents.Where(x => !x.ResourcePath.Contains("shroom")).ToArray();
	}

	public void ApplyHealthUpgrade(GameUpgrade upgrade, Gameplay gameplay)
	{
		for (int i = 1; i <= upgrade.CurrentTier; i++)
		{
			gameplay.CurrentLives += 1;
		}
	}

	public void ApplyUpgrades(Gameplay gameplay)
	{
		foreach (var upgrade in Upgrades.Where(x => x.CurrentTier != 0))
		{
			upgrade.ApplyUpgrade(upgrade, gameplay);
		}
	}

	public void ApplyHeightUpgrade(GameUpgrade upgrade, Gameplay gameplay)
	{
		for (int i = 1; i <= upgrade.CurrentTier; i++)
		{
			gameplay.Bowl.Scale = new Vector3(gameplay.Bowl.Scale.X, gameplay.Bowl.Scale.Y + 0.05f,
				gameplay.Bowl.Scale.Z);
		}
	}

	public void ApplyWidthUpgrade(GameUpgrade upgrade, Gameplay gameplay)
	{
		for (int i = 1; i <= upgrade.CurrentTier; i++)
		{
			gameplay.Bowl.Scale = new Vector3(gameplay.Bowl.Scale.X + 0.1f, gameplay.Bowl.Scale.Y,
				gameplay.Bowl.Scale.Z + 0.1f);
		}
	}

	public void ApplyPineappleUpgrade(GameUpgrade upgrade, Gameplay gameplay)
	{
		var recipe = RecipeManager.Instance.Recipes.First(x => x.ResultName == "Pineapple");

		var component = recipe.Result.Instantiate<ComponentController>();

		gameplay.AddChild(component);

		component.GlobalPosition = new Vector3(0, 1.5f, 0.5f);
	}

	public void ApplyCoconutUpgrade(GameUpgrade upgrade, Gameplay gameplay)
	{
		var recipe = RecipeManager.Instance.Recipes.First(x => x.ResultName == "Coconut");

		var component = recipe.Result.Instantiate<ComponentController>();

		gameplay.AddChild(component);

		component.GlobalPosition = new Vector3(0.5f, 1.5f, 0);
	}

	public void ApplyCabbageUpgrade(GameUpgrade upgrade, Gameplay gameplay)
	{
		var recipe = RecipeManager.Instance.Recipes.First(x => x.ResultName == "Cabbage");

		var component = recipe.Result.Instantiate<ComponentController>();

		gameplay.AddChild(component);

		component.GlobalPosition = new Vector3(1f, 1.5f, 0);
	}

	public void ApplyPumpkinUpgrade(GameUpgrade upgrade, Gameplay gameplay)
	{
		var recipe = RecipeManager.Instance.Recipes.First(x => x.ResultName == "Pumpkin");

		var component = recipe.Result.Instantiate<ComponentController>();

		gameplay.AddChild(component);

		component.GlobalPosition = new Vector3(0f, 1.5f, 0);
	}
}

public class GameUpgrade
{
	public string Name;
	public string RequiredRecipe;
	public Texture2D Icon;
	public int MaxTiers;
	public int CurrentTier;
	public int[] TierCosts;
	public Action<GameUpgrade, Gameplay> ApplyUpgrade;
}
