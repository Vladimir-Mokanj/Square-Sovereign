using System.Collections.Generic;
using Godot;

namespace FT.Managers;

public enum TerrainType : byte {WATER, LAND, HILL}
public enum ResourceType : byte {NONE, WOOD, FISH}

public class CellManager
{
    private readonly byte[] _cells;
    private readonly byte _cols;
    
    public CellManager(byte rows, byte cols)
    {
        _cells = new byte[rows * cols];
        _cols = cols;
    }

    public void InitializeCell(byte row, byte col, IEnumerable<float> heights)
    {
        int index = row * _cols + col;
        TerrainType terrainType = SetTerrainType(heights);
        _cells[index] = PackData(terrainType, ResourceType.WOOD, terrainType != TerrainType.LAND);
    }

    private byte PackData(TerrainType terrainType, ResourceType resourceType, bool isOccupied) => 
        (byte)(((byte)terrainType << 5) | ((byte)resourceType << 1) | (isOccupied ? 1 : 0));

    private TerrainType SetTerrainType(IEnumerable<float> heights)
    {
        foreach (float height in heights)
        {
            if (height < 8)
                return TerrainType.WATER;
            if (height > 8)
                return TerrainType.HILL;
        }
        
        return TerrainType.LAND;
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