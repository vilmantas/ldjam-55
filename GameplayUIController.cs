using Godot;
using System;

public partial class GameplayUIController : Control
{
    public Gameplay Gameplay;

    private ProgressBar PowerBar;

    private Label ScoreText;

    public override void _Ready()
    {
        Gameplay = Gameplay.Instance;

        PowerBar = GetNode<ProgressBar>("PowerBar");

        ScoreText = GetNode<Label>("ScoreText");
    }

    public override void _Process(double delta)
    {
        PowerBar.Value = Gameplay.Power;

        ScoreText.Text = Gameplay.CurrentScore.ToString();
    }
}
