using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ComponentController : RigidBody3D
{
	public int Id;

	public static int NextId = 0;

	[Export] public string ComponentName;

	public Dictionary<string, List<ComponentController>> CollidedComponents = new();

	public Dictionary<string, int> CollidedComponentsCounts = new();

	[Export] public int Weight = 1;

	AudioStreamPlayer3D BumpSound;

	private float BumpSoundCooldown = 0.5f;

	private PackedScene Particles;

	private List<Recipe> UnlockedRecipes;

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
	}

	public override void _Process(double delta)
	{
		BumpSoundCooldown -= (float)delta;
	}

	private void OnBodyExited(Node body)
	{
		if (body is not ComponentController component) return;

		CollidedComponents[component.ComponentName].Remove(component);

		CollidedComponentsCounts[component.ComponentName]--;
	}

	public void OnBodyEntered(Node node)
	{
		if (node is GroundController)
		{
			Gameplay.Instance.GameOver(this);
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
