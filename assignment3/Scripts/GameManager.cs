using Godot;

public partial class GameManager : Node2D
{
    public static GameManager Instance { get; private set; } = default!;
    public Player Player { get; private set; } = default!;

    public override void _Ready()
    {
        Instance = this;
        Player = GetNode<Player>("Player");
    }
}
