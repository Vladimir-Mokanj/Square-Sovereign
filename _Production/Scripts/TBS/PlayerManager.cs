using System;
using System.Threading.Tasks;
using FT.Data;
using FT.Managers;
using FT.Player;
using FT.Terrain;
using FT.Tools;
using FT.Tools.Observers;
using Godot;

namespace FT.TBS;

public partial class PlayerManager : Node
{
    [Export] private TerrainGenerationData _tgd;
    
    public static PlayerManager Instance { get; private set; }

    public IObservableAction<Action<StateParameters>> OnStateInitialized => _onStateInitialized;
    private readonly ObservableAction<Action<StateParameters>> _onStateInitialized = new();

    private StateParameters StateParameters { get; } = new();

    public override async void _Ready()
    {
        Instance = this;
        
        Initialize();
        
        await Task.Delay(TimeSpan.FromSeconds(0.05f));
        _onStateInitialized.Action?.Invoke(StateParameters);
    }

    private void Initialize()
    {
        GenerateTerrain generateTerrain = new(_tgd);
        generateTerrain.GenerateMesh(ExtentionTools.CreateNode<Node3D>("Terrain", "Terrain", GetTree().GetFirstNodeInGroup("RootNode")));

        CellManager cellManager = new(_tgd.Rows, _tgd.Cols);
        cellManager.InitializeCells(_tgd.Rows, generateTerrain.GetCellMaxYVertexHeight);
        
        PlayerCustomRaycast raycast = new(_tgd.Rows, _tgd.Cols, GetTree().GetFirstNodeInGroup("MainCamera") as Camera3D, generateTerrain.GetCellMaxYVertexHeight);

        (FindChild(nameof(StateController)) as StateController)?.Initialize(StateParameters);
    }
}