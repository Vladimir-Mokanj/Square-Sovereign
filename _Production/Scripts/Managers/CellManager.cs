using System;

namespace FT.Managers;

public enum TerrainType : byte {WATER, LAND, HILL}
public enum ResourceType : byte {NONE, FISH, FOOD, METAL }

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
    public void InitializeCells(byte rows, byte[] heights)
    {
        for (byte x = 0; x < rows; x++)
            for (byte z = 0; z < _cols; z++)
            {
                int index = x * _cols + z;
                
                (TerrainType, ResourceType) terrainData = SetTerrainData(heights[index]);
                _cells[index] = PackData(terrainData.Item1, terrainData.Item2, terrainData.Item1 != TerrainType.LAND && terrainData.Item2 == ResourceType.NONE);
            }
    }

    /// Get the cell data of the currently selected square (cell)
    /// <param name="rowCol">Square Row amd Col</param>
    /// <returns>Item1: TerrainType, Item2: ResourceType, Item3: IsOccupied</returns>
    public (TerrainType, ResourceType, bool)? GetCellData((byte? row, byte? col) rowCol)
    {
        if (!rowCol.row.HasValue || !rowCol.col.HasValue)
            return null;
        
        byte packedData = _cells[rowCol.row.Value * _cols + rowCol.col.Value];
        TerrainType terrainType = (TerrainType)((packedData >> 5) & 0x7);
        ResourceType resourceType = (ResourceType)((packedData >> 1) & 0xF);
        bool isOccupied = (packedData & 0x1) != 0;
        
        return (terrainType, resourceType, isOccupied);
    }

    public void SetIsOccupied(byte row, byte col) => _cells[row * _cols + col] |= 0x1;

    private static byte PackData(TerrainType terrainType, ResourceType resourceType, bool isOccupied) => 
        (byte)(((byte)terrainType << 5) | ((byte)resourceType << 1) | (isOccupied ? 1 : 0));

    private static (TerrainType, ResourceType) SetTerrainData(float height)
    {
        bool hasResource = Random.Shared.Next(0, 100) < 5;
        return height switch
        {
            < 1 => (TerrainType.WATER, hasResource ? ResourceType.FISH : ResourceType.NONE),
            > 1 => (TerrainType.HILL, hasResource ? ResourceType.METAL : ResourceType.NONE),
            _ => (TerrainType.LAND, hasResource ? ResourceType.FOOD : ResourceType.NONE)
        };
    }
}