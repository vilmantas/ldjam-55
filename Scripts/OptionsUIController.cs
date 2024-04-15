using Godot;
using System;

public partial class OptionsUIController : Control
{
	[Export] public HSlider VolumeSlider;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		VolumeSlider.Value = GameManager.Instance.Volume;
		VolumeSlider.ValueChanged += OnVolumeSliderChanged;
	}

	private void OnVolumeSliderChanged(double i)
	{
		GameManager.Instance.ChangeVolume((int)VolumeSlider.Value);
	}
}
