using Godot;
using System;
using System.Linq;
using Environment = System.Environment;

public partial class ShopItemController : Control
{
    [Export] public Label ResultLabel;

    [Export] public TextureRect ResultIcon;

    [Export] public Panel PurchaseOverlay;

    [Export] public Button PurchaseButton;

    [Export] public Control ComponentBoughtScreen;

    public Recipe Recipe;

    public GameUpgrade Upgrade;

    [Export] public Label PurchasedLabel;

    [Export] public Label BuyLabel;

    [Export] public Control BlockedScreen;

    [Export] public Label BlockedLabel;

    public override void _Ready()
    {
        PurchaseOverlay.MouseDefaultCursorShape = CursorShape.PointingHand;
    }

    public void TryPurchase()
    {
        if (Recipe != null)
        {
            ShopManager.Instance.TryPurchaseComponent(Recipe);
        }
        else
        {
            ShopManager.Instance.TryPurchaseUpgrade(Upgrade);
        }
    }

    public void SetRecipe(Recipe recipe, bool enabled)
    {
        Recipe = recipe;

        ResultLabel.Text = $"{recipe.ResultName} ({recipe.UnlockCost})";

        ResultIcon.Texture = recipe.Icon;

        if (!enabled)
        {
            BlockedScreen.Visible = true;
            return;
        }

        if (recipe.IsUnlocked)
        {
            ComponentBoughtScreen.Visible = true;
            return;
        }

        MouseEntered += () => PurchaseOverlay.Visible = true;

        PurchaseButton.MouseExited += () => PurchaseOverlay.Visible = false;

        PurchaseButton.Pressed += TryPurchase;
    }

    public void SetUpgrade(GameUpgrade upgrade, string requiredRecipe)
    {
        Upgrade = upgrade;

        if (upgrade.CurrentTier == upgrade.MaxTiers)
        {
            ResultLabel.Text = $"{upgrade.Name}";
        }
        else
        {
            ResultLabel.Text = $"{upgrade.Name}{Environment.NewLine}{upgrade.CurrentTier}/{upgrade.MaxTiers} ({upgrade.TierCosts[upgrade.CurrentTier+1]})";
        }

        ResultIcon.Texture = upgrade.Icon;

        if (!string.IsNullOrEmpty(requiredRecipe))
        {
            BlockedScreen.Visible = true;
            BlockedLabel.Text = $"MUST HAVE {requiredRecipe} RECIPE PURCHASED";
            BlockedLabel.AddThemeColorOverride("a", new Color(167, 49, 31));
            return;
        }

        if (upgrade.MaxTiers == upgrade.CurrentTier)
        {
            BuyLabel.Text = "UPGRADE";
            PurchasedLabel.Text = "MAXED OUT";
            ComponentBoughtScreen.Visible = true;
            return;
        }

        MouseEntered += () => PurchaseOverlay.Visible = true;

        PurchaseButton.MouseExited += () => PurchaseOverlay.Visible = false;

        PurchaseButton.Pressed += TryPurchase;
    }
}
