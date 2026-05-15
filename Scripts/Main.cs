using Godot;

public partial class Main : Node2D
{
    private Node nivelActual;

    private BuildManager buildManager; //Nodo que maneja la construcción de torres
    
    private UIManager uiManager;
    private EvolutionPanel evolutionPanel;
    
    public void CargarNivel(string rutaNivel)
    {
        //Eliminar nivel anterior
        if (nivelActual != null)
        {
            nivelActual.QueueFree();
        }
        
        //Cargar nivel seleccionado
        PackedScene escenaNivel = GD.Load<PackedScene>(rutaNivel);

        nivelActual = escenaNivel.Instantiate();

        GetNode("LevelContainer").AddChild(nivelActual);

        // Obtener referencias del nivel cargado
        BuildManager bm = nivelActual.GetNode<BuildManager>("BuildManager");
        MoneyManager mm = nivelActual.GetNode<MoneyManager>("MoneyManager");
        Game game = nivelActual as Game;

        // Guardar referencia
        buildManager = bm;

        // Pasarlas al UIManager
        uiManager.Inicializar(bm, mm, game);
        evolutionPanel.Inicializar(mm, bm);
        
        //Incializar UI en el game
        game.Inicializar(uiManager);

    }
    public override void _Ready()
    {
        uiManager = GetNode<UIManager>("UI/UIManager");
        evolutionPanel = GetNode<EvolutionPanel>("UI/EvolutionPanel");

        CargarNivel(GameData.NivelSeleccionado);
    }
    
    public override void _Input(InputEvent @event)
    {
        //Toma los clics y los transfiere para la construcción de torres
        buildManager.HandleInput(@event);
    }
    
}
