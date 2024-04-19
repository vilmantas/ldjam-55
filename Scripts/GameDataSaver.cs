using Godot;
using System;
using System.Linq;

public static class GameDataSaver
{
    public static void RemoveSavefile()
    {
        if (!FileAccess.FileExists("user://savegame.save")) return;

        DirAccess.RemoveAbsolute("user://savegame.save");
    }

    public static void SaveData()
    {
        using var saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);

        var gold = GameManager.Instance.Gold;
        var maxScore = GameManager.Instance.MaxScore;

        var data = new Godot.Collections.Dictionary<string, Variant>
        {
            { "Gold", gold },
            { "MaxScore", maxScore }
        };

        var gameJsonData = Json.Stringify(data);

        saveGame.StoreLine(gameJsonData);

        data = new Godot.Collections.Dictionary<string, Variant>();

        foreach (var recipe in RecipeManager.Instance.Recipes.Where(x => x.IsShopItem))
        {
            data.Add(recipe.ResultName, recipe.IsUnlocked);
        }

        gameJsonData = Json.Stringify(data);

        saveGame.StoreLine(gameJsonData);

        data = new Godot.Collections.Dictionary<string, Variant>();

        foreach (var recipe in UpgradesManager.Instance.Upgrades)
        {
            data.Add(recipe.Name, recipe.CurrentTier);
        }

        gameJsonData = Json.Stringify(data);

        saveGame.StoreLine(gameJsonData);
    }

    public static void LoadData(GameManager manager)
    {
        if (!FileAccess.FileExists("user://savegame.save")) return;

        using var saveGame = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);

        var jsonString = saveGame.GetLine();

        var json = new Json();

        var parseResult = json.Parse(jsonString);
        if (parseResult != Error.Ok)
        {
            GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
            return;
        }

        var gameData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);

        manager.Gold = (int)gameData["Gold"];
        manager.MaxScore = (int)gameData["MaxScore"];

        // RECIPES
        jsonString = saveGame.GetLine();

        parseResult = json.Parse(jsonString);

        if (parseResult != Error.Ok)
        {
            GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
            return;
        }

        var recipeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);

        foreach (var recipe in RecipeManager.Instance.Recipes.Where(x => x.IsShopItem))
        {
            recipe.IsUnlocked = true;
        }

        // UPGRADES
        jsonString = saveGame.GetLine();

        parseResult = json.Parse(jsonString);

        if (parseResult != Error.Ok)
        {
            GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
            return;
        }

        var upgradeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);

        foreach (var upgrade in UpgradesManager.Instance.Upgrades)
        {
            if (!upgradeData.ContainsKey(upgrade.Name)) continue;

            upgrade.CurrentTier = (int)upgradeData[upgrade.Name];
        }
    }
}
