using Godot;
using System;

public partial class RecipeUIController : Node
{
    [Export] public Label Component1Label;

    [Export] public Label Component2Label;

    [Export] public Label ResultLabel;

    public void SetRecipe(Recipe recipe)
    {
        Component1Label.Text = recipe.Components[0];
        Component2Label.Text = recipe.Components[1];
        ResultLabel.Text = $"{recipe.ResultName} ({recipe.CompletionReward})";
    }
}
