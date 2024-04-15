using Godot;
using System;
using System.Linq;

public partial class RecipeListUIController : Node
{
	[Export] public PackedScene RecipeUIPrefab;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var recipe in RecipeManager.Instance.Recipes.Where(x => x.IsUnlocked))
		{
			var recipeUI = RecipeUIPrefab.Instantiate<RecipeUIController>();
			recipeUI.SetRecipe(recipe);
			AddChild(recipeUI);
		}
	}
}
