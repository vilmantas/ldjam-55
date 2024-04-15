using Godot;
using System;

[GlobalClass]
public partial class Recipe : Resource
{
    [Export] public string[] Components;

    [Export] public PackedScene Result;

    [Export] public int CompletionReward;

    [Export] public bool IsShopItem;

    [Export] public bool IsUnlocked;

    [Export] public int UnlockCost;

    public string ResultName;

    public Texture2D Icon;
}
