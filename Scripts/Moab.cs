using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Moab : Enemy
{
    [Export] private EnemySpawner spawner;
    [Export] private Texture2D muelte;
    [Export] private PackedScene BUM;
    [Export] private Array<EnemySpawnData> Enemies;
    
    public override void _Ready()
    {
        Speed = 50f;
        GD.Print("hola mundo");
        spawner = GetParent<EnemySpawner>();
        base._Ready();
    }
    protected async override void Die()
    {
        if (isDead) return; //Solo hace la función UNA vez
        isDead = true;
        Speed = 0f;
        //Efecto de cambio de sprite
        Node2D pow = (Node2D)BUM.Instantiate();
        pow.GlobalPosition = GlobalPosition;
        GetParent().AddChild(pow);
        //Quita la colisión para no ser "vista" por las torres
        // DESACTIVAR COLISIONES: Evita que el código de daño se repita o falle
        if (HasNode("CollisionPolygon2D"))
        {
            GetNode<CollisionPolygon2D>("CollisionPolygon2D").SetDeferred("disabled", true);
        }
        sprite.Texture = muelte; // Cambia la imagen del sprite
        // Genera los enemigos que tenian en su lista
        spawner.SetIndex(index);
        spawner.SetPosition(worldPath[index]);
        // generación de enemigos
            //Spawnea la cantidad indicada de enemigos
            for (int i = 0; i < Enemies.Count; i++)
            {
                for (int j = 0; j <= Enemies[i].Count; j++)
                {
                    spawner.SpawnEnemy(Enemies[i].Type, true);
                    await ToSignal(GetTree().CreateTimer(1f), "timeout");
                }
            }
            
            
            /*foreach (EnemySpawnData AEnemys in Enemies)
            {
                foreach (EnemyType Enemy in AEnemys)
                {
                    for (int i = 0; i <= AEnemys.Count; i++)
                    {
                        //Spawnea al enemigo
                        spawner.SpawnEnemy(Enemy, true);
                        await ToSignal(GetTree().CreateTimer(1f), "timeout");
                    }
                }
            }*/

            Tween tween = CreateTween();
        tween.TweenProperty(sprite, "self_modulate:a", 0.0f, 5.0f);
        tween.Finished += base.Die;
    }
}
