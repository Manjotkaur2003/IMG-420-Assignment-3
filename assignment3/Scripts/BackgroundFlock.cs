using Godot;

public partial class BackgroundFlock : Node2D
{
    public override void _Ready()
    {
        var flock = new BoidFlock
        {
            Name = "AmbientFlock",
            InitialCount = 14,
            NeighborRadius = 70f,
            DesiredSeparation = 20f,
            SeparationWeight = 0.8f,
            AlignmentWeight = 1.2f,
            CohesionWeight = 1.0f,
            HuntWeight = 0f,
            PlayBounds = new Rect2(0,0,960,540),
            BoundsForce = 180f
        };
        AddChild(flock);

        var ps = new GpuParticles2D
        {
            Amount = 60,
            Emitting = true,
            Lifetime = 1.2f,
            OneShot = false,
            ProcessMaterial = new ParticleProcessMaterial
            {
                Gravity = Vector3.Zero,
                InitialVelocityMin = 5,
                InitialVelocityMax = 20,
                ScaleMin = 0.4f,
                ScaleMax = 0.8f,
                AngularVelocityMin = -2f,
                AngularVelocityMax = 2f
            }
        };
        AddChild(ps);
    }
}
