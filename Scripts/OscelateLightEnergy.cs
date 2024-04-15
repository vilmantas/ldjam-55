using Godot;
using System;

public partial class OscelateLightEnergy : OmniLight3D
{
	private float TimeElapsed = 0;

	private float Speed;

	public override void _Ready()
	{
		GD.Randomize();

		Speed = GD.Randf() + GD.Randf() + GD.Randf() + GD.Randf();
	}

	public override void _Process(double delta)
	{
		TimeElapsed += (float)delta;
		LightEnergy = 5f + Mathf.Sin(TimeElapsed * Speed);
	}
}
