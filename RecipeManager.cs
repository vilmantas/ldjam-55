using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class RecipeManager : Node
{
    public static RecipeManager Instance;

    public List<Recipe> Recipes = new();

    public List<string> UniqueComponents = new();

    public ConcurrentQueue<(ComponentController id1, ComponentController id2, Recipe result)> PendingCombinations = new();

    public Dictionary<int, int> ConsumedIds = new();

    public override void _Ready()
    {
        Instance = this;

        var basePath = "res://Recipes";

        var files = DirAccess.GetFilesAt(basePath);

        foreach (var file in files)
        {
            var recipe = ResourceLoader.Load<Recipe>(Path.Combine(basePath, file.Replace(".remap", "")));

            var tokens = file.Split(".")[0].Split("_").Reverse().ToList();

            if (tokens[0] != "raw")
            {
                recipe.ResultName = tokens[0].Capitalize() + " " + tokens[1].Capitalize();
            }
            else
            {
                recipe.ResultName = tokens[1].Capitalize();
            }

            recipe.Icon = ResourceLoader.Load<Texture2D>("res://ComponentImages/" + recipe.ResultName + ".png");

            Recipes.Add(recipe);
        }

        Recipes = Recipes.OrderBy(x => x.CompletionReward).ToList();

        var prefabs = DirAccess.GetFilesAt("res://Components");

        foreach (var file in prefabs)
        {
            var prefab = ResourceLoader.Load<PackedScene>(Path.Combine("res://Components", file.Replace(".remap", "")));

            var instance = prefab.Instantiate<ComponentController>();

            instance.ProcessMode = ProcessModeEnum.Disabled;

            UniqueComponents.Add(instance.ComponentName);

            instance.QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        if (!PendingCombinations.TryDequeue(out var combination)) return;

        if (ConsumedIds.ContainsKey(combination.id1.Id) ||
            ConsumedIds.ContainsKey(combination.id2.Id)) return;

        ConsumedIds.Add(combination.id1.Id, 0);
        ConsumedIds.Add(combination.id2.Id, 0);

        var midpoint = combination.id1.Transform.InterpolateWith(combination.id2.Transform, 0.5f);

        combination.id1.QueueFree();
        combination.id2.QueueFree();

        if (combination.id1.ComponentName != combination.id2.ComponentName)
        {
            GD.Print($"Completed recipe: {combination.result.ResultName}. Used: {combination.id1.ComponentName} and {combination.id2.ComponentName}");
        }


        Gameplay.Instance.HandleRecipeCompletion(combination.result, midpoint.Origin);
    }

    public void EnqueueCombination(ComponentController component1, ComponentController component2, Recipe result)
    {
        PendingCombinations.Enqueue((component1, component2, result));
    }
}
