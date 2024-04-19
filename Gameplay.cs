using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Gameplay : Node3D
{
	[Export] public AudioStreamPlayer3D HurtAudio;

	[Export] public AudioStreamPlayer3D Audio;

	[Export] public Control PauseUI;

	[Export] public Node3D Bowl;

	[Export] public Rotator Kebab;

	public int CurrentLives = 3;

	public static Gameplay Instance;

	public int CurrentScore = 0;

	[Export] public PackedScene[] UsableComponents;

	public int CurrentIndex = 0;

	public int NextIndex = 0;

	[Export] public Camera Camera;

	private float Timer = 0f;

	private bool IsLaunching = false;

	public float Power = 0f;

	public Dictionary<string, int> UsableComponentWeights = new();

	public bool IsGameOver = false;

	public Action OnGameOver;

	public PackedScene Particles;

	[Export] public Control VictoryUI;

	public override void _Ready()
	{
		Instance = this;

		Particles = ResourceLoader.Load<PackedScene>("res://Prefabs/smoke.tscn");

		UpgradesManager.Instance.ApplyUpgrades(this);

		foreach (var component in UsableComponents)
		{
			var x = component.Instantiate<ComponentController>();

			UsableComponentWeights.Add(component.ResourcePath, x.Weight);

			x.QueueFree();
		}

		UsableComponentWeights = UsableComponentWeights.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

		GD.Randomize();

		var randomTicket = GD.RandRange(0, UsableComponentWeights.Sum(x => x.Value));

		var sum = 0;

		foreach (var weight in UsableComponentWeights)
		{
			if (randomTicket < sum + weight.Value)
			{
				NextIndex = UsableComponents.ToList().FindIndex(x => x.ResourcePath == weight.Key);
				break;
			}

			sum += weight.Value;
		}

		GD.Randomize();

		randomTicket = GD.RandRange(0, UsableComponentWeights.Sum(x => x.Value));

		sum = 0;

		foreach (var weight in UsableComponentWeights)
		{
			if (randomTicket < sum + weight.Value)
			{
				CurrentIndex = UsableComponents.ToList().FindIndex(x => x.ResourcePath == weight.Key);
				break;
			}

			sum += weight.Value;
		}

		SpawnDisplay(UsableComponents[CurrentIndex], UsableComponents[NextIndex]);
	}

	public bool GameOverHandled = false;

	public override void _Process(double delta)
	{
		if (IsGameOver && !GameOverHandled)
		{
			GameOverHandled = true;

			OnGameOver?.Invoke();

			HurtAudio.Play();

			GetTree().CreateTimer(0.5f).Timeout += () => GameManager.Instance.HandleGameOver(this);

			return;
		}

		if (!IsLaunching) return;

		Timer += (float)delta;

		Power = Mathf.Sin(4.683209f +  (Timer * 3)) + 1;
	}

	public void HandleRecipeCompletion(Recipe recipe, Vector3 midpoint)
	{
		var instance = recipe.Result.Instantiate<ComponentController>();

		if (!VictoryScreenSeen)
		{
			if (instance.ComponentName == "Watermelon")
			{
				VictoryScreenSeen = true;
				VictoryUI.Visible = true;
				Kebab.LoweringEnabled = true;

				GetTree().CreateTimer(6f).Timeout += () => Kebab.Visible = false;
			}
		}

		CurrentScore += recipe.CompletionReward;

		AddChild(instance);

		midpoint.Y += 0.1f;

		instance.GlobalPosition = midpoint;

		GD.Randomize();

		var particles = Particles.Instantiate<GpuParticles3D>();

		AddChild(particles);

		particles.GlobalPosition = midpoint;

		particles.Amount = Mathf.Min(recipe.CompletionReward, 500);

		particles.Emitting = true;

		Audio.PitchScale = 0.5f + Mathf.Lerp(1f, 0f, Mathf.Min(recipe.CompletionReward, 500) / 500f);
		Audio.Play();

		GetTree().CreateTimer(1.5f).Timeout += () =>
		{
			try { particles.QueueFree(); }
			catch
			{
				// ignored
			}
		};
	}

	public bool VictoryScreenSeen = false;

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("pause"))
		{
			GetTree().Paused = true;
			PauseUI.Visible = true;
		}

		if (Input.IsActionJustPressed("throw"))
		{
			IsLaunching = true;
		}

		if (Input.IsActionJustReleased("throw"))
		{
			Camera.CurrentDisplay.QueueFree();
			Camera.NextDisplay.QueueFree();

			LaunchComponent(UsableComponents[CurrentIndex]);

			CurrentIndex = NextIndex;

			GD.Randomize();

			var randomTicket = GD.RandRange(0, UsableComponentWeights.Sum(x => x.Value));

			var sum = 0;

			foreach (var weight in UsableComponentWeights)
			{
				if (randomTicket < sum + weight.Value)
				{
					NextIndex = UsableComponents.ToList().FindIndex(x => x.ResourcePath == weight.Key);
					break;
				}

				sum += weight.Value;
			}

			SpawnDisplay(UsableComponents[CurrentIndex], UsableComponents[NextIndex]);

			IsLaunching = false;

			Timer = 0f;
		}
	}

	private void LaunchComponent(PackedScene component)
	{
		var instance = component.Instantiate<ComponentController>();

		instance.IsThrown = true;

		Camera.ComponentSpawn.AddChild(instance);

		var forward = -Camera.ComponentSpawn.GlobalTransform.Basis.Z * (2.5f + Power) ;

		forward.Y += 2.5f + Power;

		GD.Randomize();

		instance.RotateX((float)GD.RandRange(-Mathf.Pi, Mathf.Pi));
		instance.RotateY((float)GD.RandRange(-Mathf.Pi, Mathf.Pi));
		instance.RotateZ((float)GD.RandRange(-Mathf.Pi, Mathf.Pi));

		instance.ApplyCentralImpulse(forward);

		instance.Reparent(this);
	}

	private void SpawnDisplay(PackedScene current, PackedScene next)
	{
		var currentDisplayInstance = current.Instantiate<ComponentController>();

		currentDisplayInstance.IsThrown = true;

		Camera.DisplaySpawn.AddChild(currentDisplayInstance);

		Camera.CurrentDisplay = currentDisplayInstance;

		Camera.CurrentDisplay.Freeze = true;
		Camera.CurrentDisplay.FreezeMode = RigidBody3D.FreezeModeEnum.Static;

		var nextDisplayInstance = next.Instantiate<ComponentController>();

		nextDisplayInstance.IsThrown = true;

		Camera.NextDisplaySpawn.AddChild(nextDisplayInstance);

		Camera.NextDisplay = nextDisplayInstance;

		Camera.NextDisplay.Freeze = true;
		Camera.NextDisplay.FreezeMode = RigidBody3D.FreezeModeEnum.Static;
	}

	public void OnGroundHit(ComponentController component)
	{
		CurrentLives--;

		HurtAudio.Play();

		if (CurrentLives == 0)
		{
			IsGameOver = true;
		}
	}
}
