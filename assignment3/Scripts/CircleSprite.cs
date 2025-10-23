using Godot;

public partial class CircleSprite : Node2D
{
    [Export] public float Radius = 8f;
    [Export] public Color Fill = new Color(0.95f, 0.6f, 0.2f, 1f);

    public override void _Draw()
    {
        DrawCircle(Vector2.Zero, Radius, Fill);
    }
    public override void _Process(double delta) => QueueRedraw();
}
