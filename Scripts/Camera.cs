using Godot;
using System;

public partial class Camera : Node3D
{
	[Export] public Node3D ComponentSpawn;

	[Export] public Node3D DisplaySpawn;

	[Export] public RigidBody3D CurrentDisplay;

	[Export] public Node3D NextDisplaySpawn;

	[Export] public RigidBody3D NextDisplay;

	public bool RotationEnabled = false;

	public int MouseSensitivity = 5;

	public Vector2 Offset;

	public override void _Ready()
	{
		Gameplay.Instance.OnGameOver += DisableRotation;
	}

	public override void _Process(double delta)
	{
		RotateMouse(delta);
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("rotate"))
		{
			EnableRotation();
		}

		if (Input.IsActionJustReleased("rotate"))
		{
			DisableRotation();
		}

		if (!RotationEnabled) return;

		if (@event is InputEventMouseMotion mouseMotion)
		{
			Offset = mouseMotion.Relative;
		}
		else
		{
			Offset = Vector2.Zero;
		}
	}

	private void DisableRotation()
	{
		RotationEnabled = false;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Offset = Vector2.Zero;
	}

	private void EnableRotation()
	{
		RotationEnabled = true;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	private void RotateMouse(double delta)
	{
		if (!RotationEnabled) return;

		var x = -Offset.X;
		var y = -Offset.Y;

		RotateY(Mathf.DegToRad(x * (float)delta * MouseSensitivity));

		Offset = Vector2.Zero;
	}
}
