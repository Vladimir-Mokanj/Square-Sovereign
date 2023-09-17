using System;
using System.Collections.Generic;
using Godot;

namespace FT.Managers;

public enum TerrainType : byte {WATER, LAND, HILL}
public enum ResourceType : byte {NONE, WOOD, FISH, METAL }

public class CellManager
{
    private readonly byte[] _cells;
    private readonly byte _cols;

    public CellManager(byte rows, byte cols)
    {
        _cells = new byte[rows * cols];
        _cols = cols;
    }

    public void InitializeCells(byte rows, float[] heights, float cellSize)
    {
        for (byte x = 0; x < rows; x++)
            for (byte z = 0; z < _cols; z++)
            {
                int tl = x * (_cols + 1) + z,
                    tr = tl + 1,
                    bl = tl + _cols + 1,
                    br = tl + _cols + 2;
    
                int index = x * _cols + z;
                
                (TerrainType, ResourceType) terrainData = SetTerrainData(new[]{heights[tl], heights[tr], heights[bl], heights[br]}, cellSize);
                _cells[index] = PackData(terrainData.Item1, terrainData.Item2, terrainData.Item1 != TerrainType.LAND);
                GD.Print(GetCellData(x, z));
            }
    }

    private byte PackData(TerrainType terrainType, ResourceType resourceType, bool isOccupied) => 
        (byte)(((byte)terrainType << 5) | ((byte)resourceType << 1) | (isOccupied ? 1 : 0));

    private (TerrainType, ResourceType) SetTerrainData(IEnumerable<float> heights, float cellSize)
    {
        bool hasResource = Random.Shared.Next(0, 100) < 5;
        foreach (float height in heights)
        {
            if (height < cellSize)
                return (TerrainType.WATER, hasResource ? ResourceType.FISH : ResourceType.NONE);
            if (height > cellSize)
                return (TerrainType.HILL, hasResource ? ResourceType.METAL : ResourceType.NONE);
        }
        
        return (TerrainType.LAND, hasResource ? ResourceType.WOOD : ResourceType.NONE);
    }
    
    public (TerrainType, ResourceType, bool) GetCellData(int row, int col)
    {
        byte packedData = _cells[row * _cols + col];
        TerrainType terrainType = (TerrainType)((packedData >> 5) & 0x7);
        ResourceType resourceType = (ResourceType)((packedData >> 1) & 0xF);
        bool isOccupied = (packedData & 0x1) != 0;
        
        return (terrainType, resourceType, isOccupied);
    }
}