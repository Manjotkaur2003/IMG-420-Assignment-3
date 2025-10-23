using Godot;

public partial class BoidFlock : Node2D
{
    [Export] public PackedScene BoidScene = default!;
    [Export] public int InitialCount = 20;
    [Export] public float NeighborRadius = 60f;
    [Export] public float DesiredSeparation = 24f;

    [Export] public float SeparationWeight = 1.6f;
    [Export] public float AlignmentWeight = 1.0f;
    [Export] public float CohesionWeight = 0.9f;

    [Export] public float BoundsPadding = 24f;
    [Export] public Rect2 PlayBounds = new Rect2(0,0,960,540);
    [Export] public float BoundsForce = 220f;

    [Export] public NodePath HuntTargetPath = new NodePath("");
    [Export] public float HuntWeight = 0.9f;
    [Export] public bool Active = true;

    private Node2D? _huntTarget;

    public override void _Ready()
    {
        _huntTarget = GetNodeOrNull<Node2D>(HuntTargetPath);
        if (BoidScene == null)
            BoidScene = (PackedScene)ResourceLoader.Load("res://scenes/Boid.tscn");

        for (int i = 0; i < InitialCount; i++)
        {
            var b = BoidScene.Instantiate<Boid>();
            AddChild(b);
            b.Position = new Vector2((float)GD.RandRange(PlayBounds.Position.X + 80, PlayBounds.End.X - 80),
                                     (float)GD.RandRange(PlayBounds.Position.Y + 80, PlayBounds.End.Y - 80));
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Active) return;
        foreach (var node in GetChildren())
        {
            if (node is not Boid b) continue;
            Vector2 sep = Separation(b);
            Vector2 ali = Alignment(b);
            Vector2 coh = Cohesion(b);
            Vector2 bounds = KeepInBounds(b);
            Vector2 hunt = Vector2.Zero;
            if (_huntTarget != null)
            {
                var desired = (_huntTarget.GlobalPosition - b.GlobalPosition);
                var steer = desired.Normalized() * b.MaxSpeed - b.Vel;
                hunt = steer;
            }

            Vector2 acc = sep * SeparationWeight + ali * AlignmentWeight + coh * CohesionWeight + bounds + hunt * HuntWeight;
            b.ApplyForce(acc, b.MaxForce);
        }
    }

    private Vector2 Separation(Boid boid)
    {
        Vector2 steer = Vector2.Zero;
        int count = 0;
        foreach (var node in GetChildren())
        {
            if (node == boid || node is not Boid other) continue;
            float d = boid.GlobalPosition.DistanceTo(other.GlobalPosition);
            if (d > 0 && d < DesiredSeparation)
            {
                Vector2 diff = (boid.GlobalPosition - other.GlobalPosition).Normalized();
                diff /= Mathf.Max(d, 0.001f);
                steer += diff;
                count++;
            }
        }
        if (count > 0) steer /= count;
        if (steer == Vector2.Zero) return steer;
        steer = steer.Normalized() * boid.MaxSpeed - boid.Vel;
        return steer;
    }

    private Vector2 Alignment(Boid boid)
    {
        Vector2 sum = Vector2.Zero;
        int count = 0;
        foreach (var node in GetChildren())
        {
            if (node == boid || node is not Boid other) continue;
            float d = boid.GlobalPosition.DistanceTo(other.GlobalPosition);
            if (d < NeighborRadius)
            {
                sum += other.Vel;
                count++;
            }
        }
        if (count == 0) return Vector2.Zero;
        sum /= count;
        Vector2 desired = sum.Normalized() * boid.MaxSpeed;
        return desired - boid.Vel;
    }

    private Vector2 Cohesion(Boid boid)
    {
        Vector2 center = Vector2.Zero;
        int count = 0;
        foreach (var node in GetChildren())
        {
            if (node == boid || node is not Boid other) continue;
            float d = boid.GlobalPosition.DistanceTo(other.GlobalPosition);
            if (d < NeighborRadius)
            {
                center += other.GlobalPosition;
                count++;
            }
        }
        if (count == 0) return Vector2.Zero;
        center /= count;
        Vector2 desired = (center - boid.GlobalPosition).Normalized() * boid.MaxSpeed;
        return desired - boid.Vel;
    }

    private Vector2 KeepInBounds(Boid boid)
    {
        Vector2 steer = Vector2.Zero;
        Vector2 pos = boid.GlobalPosition;
        if (pos.X < PlayBounds.Position.X + BoundsPadding) steer.X = 1;
        else if (pos.X > PlayBounds.End.X - BoundsPadding) steer.X = -1;
        if (pos.Y < PlayBounds.Position.Y + BoundsPadding) steer.Y = 1;
        else if (pos.Y > PlayBounds.End.Y - BoundsPadding) steer.Y = -1;
        return steer * BoundsForce;
    }

    public void SetTarget(Node2D target) => _huntTarget = target;
    public void SetActive(bool active) => Active = active;

    public void SpawnWave(int count, Node2D target)
    {
        _huntTarget = target;
        for (int i = 0; i < count; i++)
        {
            var b = BoidScene.Instantiate<Boid>();
            AddChild(b);
            b.Position = new Vector2((float)GD.RandRange(PlayBounds.End.X - 120, PlayBounds.End.X - 40),
                                     (float)GD.RandRange(PlayBounds.Position.Y + 40, PlayBounds.End.Y - 40));
        }
        Visible = true;
        Active = true;
    }
}
