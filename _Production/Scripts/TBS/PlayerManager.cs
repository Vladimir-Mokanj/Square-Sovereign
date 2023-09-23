using System;
using System.Threading.Tasks;
using FT.Data;
using FT.Input;
using FT.Managers;
using FT.Terrain;
using FT.Tools;
using FT.Tools.Observers;
using Godot;

namespace FT.TBS;

public partial class PlayerManager : Node
{
    public static PlayerManager Instance { get; private set; }

    public IObservableAction<Action<StateParameters>> OnStateInitialized => _onStateInitialized;
    private readonly ObservableAction<Action<StateParameters>> _onStateInitialized = new();

    public IObservableAction<Action<InputDataParameters>> OnDataInitialized => _onDataInitialized;
    private readonly ObservableAction<Action<InputDataParameters>> _onDataInitialized = new();
    
    [Export] private TerrainGenerationData _tgd;
    
    private InputController _inputController;

    private InputDataParameters DataParameters { get; } = new();
    private StateParameters StateParameters { get; } = new();

    public override async void _Ready()
    {
        Instance = this;
        
        Initialize();
        
        await Task.Delay(TimeSpan.FromSeconds(0.05f));
        _onDataInitialized.Action?.Invoke(DataParameters);
        _onStateInitialized.Action?.Invoke(StateParameters);
    }

    private void Initialize()
    {
        GenerateTerrain generateTerrain = new(_tgd);
        generateTerrain.GenerateMesh(ExtentionTools.CreateNode<Node3D>("Terrain", "Terrain", GetTree().GetFirstNodeInGroup("RootNode")));

        CellManager cellManager = new(_tgd.Rows, _tgd.Cols);
        cellManager.InitializeCells(_tgd.Rows, generateTerrain.GetCellMaxYVertexHeight);
        
        (FindChild(nameof(InputController)) as InputController)?.Initialize(DataParameters);
        (FindChild(nameof(StateController)) as StateController)?.Initialize(StateParameters); 
        (FindChild(nameof(BuildingController)) as BuildingController)?.Initialize(cellManager); 
    }
}