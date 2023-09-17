using System;

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
    
    /// Generate all of the cell data
    /// <param name="rows">Terrain Rows</param>
    /// <param name="heights">Vertex Y Heights Array</param>
    /// <param name="cellSize">Size of the square</param>
    public void InitializeCells(byte rows, byte[] heights, float cellSize)
    {
        for (byte x = 0; x < rows; x++)
            for (byte z = 0; z < _cols; z++)
            {
                int index = x * _cols + z;
                
                (TerrainType, ResourceType) terrainData = SetTerrainData(heights[index], cellSize);
                _cells[index] = PackData(terrainData.Item1, terrainData.Item2, terrainData.Item1 != TerrainType.LAND);
            }
    }

    /// Get the cell data of the currently selected square (cell)
    /// <param name="row">Square Row</param>
    /// <param name="col">Square Col</param>
    /// <returns>Item1: TerrainType, Item2: ResourceType, Item3: IsOccupied</returns>
    public (TerrainType, ResourceType, bool) GetCellData(byte row, byte col)
    {
        byte packedData = _cells[row * _cols + col];
        TerrainType terrainType = (TerrainType)((packedData >> 5) & 0x7);
        ResourceType resourceType = (ResourceType)((packedData >> 1) & 0xF);
        bool isOccupied = (packedData & 0x1) != 0;
        
        return (terrainType, resourceType, isOccupied);
    }
    
    private static byte PackData(TerrainType terrainType, ResourceType resourceType, bool isOccupied) => 
        (byte)(((byte)terrainType << 5) | ((byte)resourceType << 1) | (isOccupied ? 1 : 0));

    private static (TerrainType, ResourceType) SetTerrainData(float height, float cellSize)
    {
        bool hasResource = Random.Shared.Next(0, 100) < 5;
        if (height < cellSize) return (TerrainType.WATER, hasResource ? ResourceType.FISH : ResourceType.NONE);
        if (height > cellSize) return (TerrainType.HILL, hasResource ? ResourceType.METAL : ResourceType.NONE);
        return (TerrainType.LAND, hasResource ? ResourceType.WOOD : ResourceType.NONE);
    }
}