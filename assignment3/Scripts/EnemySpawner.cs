using Godot;

public partial class EnemySpawner : Node
{
    [Export] public NodePath FlockPath = new NodePath("");
    [Export] public int WaveCount = 12;
    [Export] public float HuntSeconds = 10f;

    private BoidFlock? _flock;
    private float _timer = 0f;
    private bool _hunting = false;

    public override void _Ready()
    {
        _flock = GetNodeOrNull<BoidFlock>(FlockPath) ?? GetNode<BoidFlock>("../WaveFlock");
        _flock!.SetActive(false);
        _flock.Visible = false;
    }

    public void TriggerWave()
    {
        if (_hunting) return;
        _flock!.SpawnWave(WaveCount, GameManager.Instance.Player);
        _timer = HuntSeconds;
        _hunting = true;
    }

    public override void _Process(double delta)
    {
        if (_hunting)
        {
            _timer -= (float)delta;
            if (_timer <= 0f)
            {
                _flock!.SetActive(false);
                _flock.Visible = false;
                _hunting = false;
            }
        }
    }
}
