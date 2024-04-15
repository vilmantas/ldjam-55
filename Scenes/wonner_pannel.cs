using Godot;
using System;

public partial class wonner_pannel : Panel
{
    public override void _Input(InputEvent @event)
    {
        if (Input.IsMouseButtonPressed(MouseButton.Left) || Input.IsMouseButtonPressed(MouseButton.Right))
        {
            Visible = false;
        }
    }
}
