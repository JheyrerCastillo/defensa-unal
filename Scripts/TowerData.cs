using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TowerData : Resource
{
    [Export] public string Name;
    [Export] public Texture2D icon;
    [Export] public TowerType TowerType;
    [Export] public PackedScene BulletScene;
    
    [Export] public float FireRate = 1f;
    [Export] public int Cost = 50;
    
    [Export] public Array<TowerData> Evolutions = new();
}
