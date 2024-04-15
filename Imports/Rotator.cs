using Godot;
using System;

public partial class Rotator : Node3D
{
	[Export] public bool LoweringEnabled = false;

	[Export] public float LoweringDistance = 1.0f;

	[Export] public float LoweringDuration = 3f;

	public float LoweringElapsed = 0f;
	// Called every frame. 'delta' is the elapsed time since the previous frame.

	Vector3 _initialPosition;

	public override void _Ready()
	{
		_initialPosition = GlobalPosition;
	}

	public override void _Process(double delta)
	{
		RotateY(Mathf.Pi * (float)delta);

		if (!LoweringEnabled) return;

		LoweringElapsed += (float)delta;

		var toLower = Mathf.Lerp(0 , LoweringDistance, Mathf.Min(LoweringElapsed / LoweringDuration, 1));

		GlobalPosition = _initialPosition + new Vector3(0, -toLower, 0);
	}
}
