using Godot;
using System;

public partial class SummoningManager : Node
{
    public static SummoningManager Instance;

    public override void _Ready()
    {
        Instance = this;
    }

    public void TrySummonCreature(Gameplay gameplay)
    {
        // gameplay.Bowl.Scale = new Vector3(gameplay.Bowl.Scale.X, gameplay.Bowl.Scale.Y + 0.1f, gameplay.Bowl.Scale.Z);

        gameplay.Bowl.Scale = new Vector3(gameplay.Bowl.Scale.X + 0.1f, gameplay.Bowl.Scale.Y + 0.1f, gameplay.Bowl.Scale.Z + 0.1f);
    }
}
