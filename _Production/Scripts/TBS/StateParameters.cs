using Godot;
using System;
using FT.Managers;
using FT.TBS.States;

namespace FT.TBS;

public class StateParameters
{
    public Action<byte, byte> RowCol { get; set; }
    public Action<GameState> GameState { get; set; }
    public Action<(TerrainType, ResourceType, bool)> RaycastData { get; set; }
}