using Godot;
using System;

public partial class IntroUIController : Control
{
    [Export] public AudioStreamPlayer3D ClickStream;

    [Export] public AudioStreamPlayer3D HoverStream;

    [Export] public Button StartButton;

    [Export] public Button ShopButton;

    [Export] public Button ExitButton;

    [Export] public Button SettingsButton;

    [Export] public Label ScoreLabel;

    [Export] public Label GoldLabel;

    [Export] public ShopUIController ShopUI;

    [Export] public OptionsUIController OptionsUI;

    [Export] public Button ClearStats;
    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;

        ScoreLabel.Text = GameManager.Instance.MaxScore.ToString();

        GoldLabel.Text = GameManager.Instance.Gold.ToString();

        StartButton.Pressed += OnStartButtonPressed;

        ShopButton.Pressed += () =>
        {
            if (OptionsUI.Visible)
            {
                OptionsUI.Visible = false;
            }
            ClickStream.Play();
            ShopUI.Visible = !ShopUI.Visible;
        };

        ExitButton.Pressed += () =>
        {
            ClickStream.Play();
            GetTree().Quit();
        };

        SettingsButton.Pressed += () =>
        {
            if (ShopUI.Visible)
            {
                ShopUI.Visible = false;
            }

            ClickStream.Play();
            OptionsUI.Visible = !OptionsUI.Visible;
        };

        SettingsButton.MouseEntered += () =>
        {
            HoverStream.Play();
        };

        ExitButton.MouseEntered += () =>
        {
            HoverStream.Play();
        };

        ShopButton.MouseEntered += () =>
        {
            HoverStream.Play();
        };

        StartButton.MouseEntered += () =>
        {
            HoverStream.Play();
        };

        ClearStats.MouseEntered += () =>
        {
            HoverStream.Play();
        };

        ClearStats.Pressed += () =>
        {
            ClickStream.Play();
            GameDataSaver.RemoveSavefile();
            GetTree().Quit();
        };
    }

    public override void _Process(double delta)
    {
        GoldLabel.Text = GameManager.Instance.Gold.ToString();
    }

    public void OnStartButtonPressed()
    {
        ClickStream.Play();
        GameManager.Instance.StartGame();
    }
}
