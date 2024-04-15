using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance;

	public int MaxScore = 0;

	public int Gold = 0;

	public int Volume = 50;

	public Action OnVolumeChanged;

	public override void _Ready()
	{
		Instance = this;

		GameDataSaver.LoadData(this);
	}

	public void HandleGameOver(Gameplay gameplay)
	{
		MaxScore = Mathf.Max(MaxScore, gameplay.CurrentScore);

		Gold += gameplay.CurrentScore / 10;

		GameDataSaver.SaveData();

		GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://Scenes/intro.tscn"));
	}

	public void StartGame()
	{
		GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://Scenes/level_1.tscn"));
	}

	public void ChangeVolume(int newVolume)
	{
		Volume = newVolume;

		OnVolumeChanged?.Invoke();

		if (newVolume == 0)
		{
			AudioServer.SetBusMute(0, true);
		}
		else
		{
			AudioServer.SetBusMute(0, false);
		}
	}
}
