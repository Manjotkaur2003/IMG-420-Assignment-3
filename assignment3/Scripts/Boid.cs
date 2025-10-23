using Godot;

public partial class Boid : CharacterBody2D
{
    [Export] public float MaxSpeed = 160f;
    [Export] public float MaxForce = 240f;

    public Vector2 Accel;
    public Vector2 Vel;

    public override void _Ready()
    {
        Vel = new Vector2(GD.Randf() * 2f - 1f, GD.Randf() * 2f - 1f).Normalized() * (MaxSpeed * 0.5f);
        CreateVisual();
    }

    private void CreateVisual()
    {
        var poly = new Polygon2D();
        poly.Polygon = new Vector2[] { new Vector2(12,0), new Vector2(-8,6), new Vector2(-8,-6) };
        poly.Color = new Color(0.2f, 0.85f, 0.75f, 1f);
        AddChild(poly);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vel += Accel * (float)delta;
        if (Vel.Length() > MaxSpeed) Vel = Vel.Normalized() * MaxSpeed;
        Velocity = Vel;
        MoveAndSlide();
        if (Vel.Length() > 0.01f) Rotation = Vel.Angle();
        Accel = Vector2.Zero;
    }

    public void ApplyForce(Vector2 f, float maxForce)
    {
        if (f.Length() > maxForce) f = f.Normalized() * maxForce;
        Accel += f;
    }
}
