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
    [Export] private InputController _inputController;

    private InputDataParameters DataParameters { get; set; } = new();
    private StateParameters StateParameters { get; set; } = new();

    public override async void _Ready()
    {
        Instance = this;

        await Task.Delay(TimeSpan.FromSeconds(0.05f));
        _onStateInitialized?.Action.Invoke(StateParameters);
        _onDataInitialized?.Action.Invoke(DataParameters);
    }

    private void Initialize()
    {
        GenerateTerrain generateTerrain = new(_tgd);
        generateTerrain.GenerateMesh(ExtentionTools.CreateNode<Node3D>("Terrain", "Terrain", GetTree().GetFirstNodeInGroup("RootNode")));

        CellManager cellManager = new(_tgd.Rows, _tgd.Cols);
        cellManager.InitializeCells(_tgd.Rows, generateTerrain.GetCellMaxYVertexHeight);
        
        _inputController.Initialize(DataParameters);
    }
}