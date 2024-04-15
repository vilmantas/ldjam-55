using Godot;
using System;

public partial class PauseUIController : Control
{
	[Export] public Button ResumeButton;
	[Export] public Button RestartButton;
	[Export] public Button MenuButton;
	[Export] public Button CloseGameButton;

	[Export] public AudioStreamPlayer3D ClickStream;
	[Export] public AudioStreamPlayer3D HoverStream;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HoverStream.ProcessMode = ProcessModeEnum.Always;
		ClickStream.ProcessMode = ProcessModeEnum.Always;
		ProcessMode = ProcessModeEnum.Always;

		ResumeButton.MouseEntered += () =>
		{
			HoverStream.Play();
		};

		RestartButton.MouseEntered += () =>
		{
			HoverStream.Play();
		};

		CloseGameButton.MouseEntered += () =>
		{
			HoverStream.Play();
		};

		MenuButton.MouseEntered += () =>
		{
			HoverStream.Play();
		};

		CloseGameButton.Pressed += () =>
		{
			ClickStream.Play();
			GetTree().Paused = false;
			GameDataSaver.SaveData();
			GetTree().Quit();
		};


		MenuButton.Pressed += () =>
		{
			ClickStream.Play();
			GetTree().Paused = false;
			GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://Scenes/intro.tscn"));
		};

		RestartButton.Pressed += () =>
		{
			ClickStream.Play();
			GetTree().Paused = false;
			GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://Scenes/level_1.tscn"));
		};

		ResumeButton.Pressed += () =>
		{
			ClickStream.Play();
			GetTree().Paused = false;
			Visible = false;
		};
	}
}
