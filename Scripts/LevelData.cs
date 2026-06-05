using Godot;
using Godot.Collections;

[GlobalClass]
public partial class LevelData : Resource
{
    [Export] public Array<WaveData> Waves = new();
}