using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ComponentController : RigidBody3D
{
	public int Id;

	private bool IsConsumed = false;

	public static int NextId = 0;

	[Export] public string ComponentName;

	public Dictionary<string, List<ComponentController>> CollidedComponents = new();

	public Dictionary<string, int> CollidedComponentsCounts = new();

	[Export] public int Weight = 1;

	AudioStreamPlayer3D BumpSound;

	private float BumpSoundCooldown = 0.5f;

	private PackedScene Particles;

	private List<Recipe> UnlockedRecipes;

	private float FullSizeTimout =0.1f;

	private float FullSizeTimeoutCurrent = 0f;

	public bool IsThrown = false;

	public override void _Ready()
	{
		UnlockedRecipes = RecipeManager.Instance.Recipes
			.Where(x => x.IsUnlocked && x.Components.Contains(ComponentName)).ToList();
		BumpSound = new AudioStreamPlayer3D();
		BumpSound.Stream = ResourceLoader.Load<AudioStream>("res://Addons/Bump.wav");

		AddToGroup("components");

		AddChild(BumpSound);

		Id = NextId;
		NextId++;

		ContactMonitor = true;
		MaxContactsReported = 10;

		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;

		CollidedComponents = RecipeManager.Instance.UniqueComponents.ToDictionary(x => x, x => new List<ComponentController>());

		CollidedComponentsCounts = RecipeManager.Instance.UniqueComponents.ToDictionary(x => x, x => 0);

		if (IsThrown) return;

		Scale = Vector3.Zero * 0.1f;
	}

	public override void _Process(double delta)
	{
		BumpSoundCooldown -= (float)delta;

		if (IsThrown) return;

		FullSizeTimeoutCurrent += (float)delta;

		var currentScale = Mathf.Lerp(0, 1, Math.Min(1, FullSizeTimeoutCurrent / FullSizeTimout));

		if (currentScale == 1) return;

		Scale = new Vector3(currentScale, currentScale, currentScale);

		if (ComponentName == "Pineapple")
		{
			RotateY((float)GD.RandRange(-Mathf.Pi, Mathf.Pi));
		}
		else
		{
			GD.Randomize();

			RotateY((float)GD.RandRange(-Mathf.Pi, Mathf.Pi));

			GD.Randomize();

			RotateX((float)GD.RandRange(-Mathf.Pi, Mathf.Pi));

			GD.Randomize();

			RotateZ((float)GD.RandRange(-Mathf.Pi, Mathf.Pi));
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body is not ComponentController component) return;

		CollidedComponents[component.ComponentName].Remove(component);

		CollidedComponentsCounts[component.ComponentName]--;
	}

	public void OnBodyEntered(Node node)
	{
		if (IsConsumed) return;

		if (node is GroundController)
		{
			Gameplay.Instance.OnGroundHit(this);
			IsConsumed = true;
			return;
		}

		if (node is not ComponentController component)
		{
			return;
		}

		CollidedComponents[component.ComponentName].Add(component);

		CollidedComponentsCounts[component.ComponentName]++;

		if (Id == NextId - 2)
		{
			if (BumpSoundCooldown < 0)
			{
				BumpSound.PitchScale = GD.Randf() + 0.5f;
				BumpSound.Play();
				BumpSoundCooldown = 0.3f;
			}

		}

		if (component.Id > Id) return;

		CheckRecipes(component);
	}

	public void CheckRecipes(ComponentController collider)
	{
		var validRecipes = UnlockedRecipes.Where(x =>
				x.Components.Contains(collider.ComponentName) &&
				x.Components.Contains(ComponentName))
			.ToList();

		foreach (var recipe in validRecipes)
		{
			Dictionary<string, int> requiredComponents = recipe.Components.GroupBy(x => x)
				.ToDictionary(x => x.Key, x => x.Count());

			requiredComponents[ComponentName]--;

			bool canWork = true;

			foreach (var requiredComponent in requiredComponents)
			{
				if (CollidedComponentsCounts[requiredComponent.Key] < requiredComponent.Value)
				{
					canWork = false;
					break;
				}
			}

			if (!canWork) continue;

			if (collider.ComponentName != ComponentName)
			{
				GD.Print($"$!!!!!! {ComponentName} + {collider.ComponentName}");
			}

			// var usedComponents = new List<ComponentController>();
			//
			// foreach (var componentName in requiredComponents.Keys)
			// {
			// 	usedComponents.AddRange(CollidedComponents[componentName].Take(requiredComponents[componentName]));
			// }
			//
			// foreach (var component in usedComponents)
			// {
			// 	// component.QueueFree();
			// }
			//
			// // collider.QueueFree();

			RecipeManager.Instance.EnqueueCombination(this, collider, recipe);
		}
	}
}
