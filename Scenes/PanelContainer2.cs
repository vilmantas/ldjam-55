using Godot;
using System;

public partial class PanelContainer2 : PanelContainer
{
	[Export] public float TransitionDuration;

	public static bool OnlyOnce = false;

	public float Elapsed;

	public float Delay = 1f;

	private Gradient g2;

	private Color startColor;
	public override void _Ready()
	{
		g2 = ((GradientTexture2D)((StyleBoxTexture)GetThemeStylebox("panel")).Texture).Gradient;

		startColor = g2.GetColor(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (OnlyOnce)
		{
			Visible = false;
			return;
		}

		Elapsed += (float)delta;

		if (Elapsed < Delay) return;

		var newAlpha = Mathf.Lerp(255, 0, Mathf.Min((Elapsed - Delay) / (TransitionDuration), 1));

		g2.SetColor(0, Color.Color8(12, 179, 205, (byte)newAlpha));

		((GradientTexture2D)((StyleBoxTexture)GetThemeStylebox("panel")).Texture).Gradient = g2;

		if (Elapsed > TransitionDuration)
		{
			OnlyOnce = true;
			Visible = false;
		}
	}
}
