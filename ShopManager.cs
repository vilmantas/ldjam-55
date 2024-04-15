using Godot;
using System;

public partial class ShopManager : Node
{
    public static ShopManager Instance;

    public override void _Ready()
    {
        Instance = this;
    }

    public void TryPurchaseComponent(Recipe recipe)
    {
        if (recipe.UnlockCost > GameManager.Instance.Gold) return;

        GameManager.Instance.Gold -= recipe.UnlockCost;
        recipe.IsUnlocked = true;
        GameDataSaver.SaveData();
        GetTree().ReloadCurrentScene();
    }

    public void TryPurchaseUpgrade(GameUpgrade upgrade)
    {
        if (upgrade.TierCosts[upgrade.CurrentTier+1] > GameManager.Instance.Gold) return;

        GameManager.Instance.Gold -= upgrade.TierCosts[upgrade.CurrentTier+1];
        upgrade.CurrentTier++;
        GameDataSaver.SaveData();
        GetTree().ReloadCurrentScene();
    }
}
