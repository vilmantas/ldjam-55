using Godot;
using System;

public partial class AudioVolumeChanger : AudioStreamPlayer3D
{
    public override void _Ready()
    {
        // GameManager.Instance.OnVolumeChanged += OnVolumeChanged;
    }

    private void OnVolumeChanged()
    {
        VolumeDb = GameManager.Instance.Volume / 5 - 10;
        GD.Print(VolumeDb);

        if (GameManager.Instance.Volume == 0)
        {
            VolumeDb = -80;
        }
    }
}
