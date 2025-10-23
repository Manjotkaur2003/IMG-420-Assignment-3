using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] public float Speed = 220f;
    [Export] public float SprintMultiplier = 1.6f;
    [Export] public float SprintDuration = 1.5f;
    [Export] public float SprintCooldown = 2.0f;

    private float _sprintTimer = 0f;
    private float _cooldownTimer = 0f;

    public override void _Ready()
    {
        EnsureAction("ui_left", Key.A, Key.Left);
        EnsureAction("ui_right", Key.D, Key.Right);
        EnsureAction("ui_up", Key.W, Key.Up);
        EnsureAction("ui_down", Key.S, Key.Down);
    }

    private void EnsureAction(string action, Key primary, Key fallback)
    {
        if (!InputMap.HasAction(action)) InputMap.AddAction(action);
        bool hasPrimary = false, hasFallback = false;
        foreach (var ev in InputMap.ActionGetEvents(action))
        {
            if (ev is InputEventKey kev)
            {
                if (kev.Keycode == primary) hasPrimary = true;
                if (kev.Keycode == fallback) hasFallback = true;
            }
        }
        if (!hasPrimary) InputMap.ActionAddEvent(action, new InputEventKey { Keycode = primary });
        if (!hasFallback) InputMap.ActionAddEvent(action, new InputEventKey { Keycode = fallback });
    }

    public override void _PhysicsProcess(double delta)
    {
        var input = Vector2.Zero;
        input.X = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        input.Y = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
        if (input.LengthSquared() > 1f) input = input.Normalized();

        float speed = Speed;
        if (Input.IsKeyPressed(Key.Shift) && _cooldownTimer <= 0f)
        {
            if (_sprintTimer < SprintDuration)
            {
                _sprintTimer += (float)delta;
                speed *= SprintMultiplier;
            }
            else
            {
                _cooldownTimer = SprintCooldown;
            }
        }
        else
        {
            _sprintTimer = Math.Max(0f, _sprintTimer - (float)delta * 0.7f);
        }
        if (_cooldownTimer > 0f) _cooldownTimer -= (float)delta;

        Velocity = input * speed;
        MoveAndSlide();
    }
}
