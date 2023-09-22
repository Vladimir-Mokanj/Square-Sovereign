using System;
using FT.Data;
using Godot;

namespace FT.Player;

public class PlayerCustomRaycast
{
    private readonly Camera3D _camera;
    private readonly byte[] _yHeights;
    private readonly byte _cellSize;
    private readonly byte _rows;
    private readonly byte _cols;

    private readonly float _maxT;
    private readonly float _step;
    private Vector3 _direction;
    private Vector3 _scaledDirection;
    private Vector3 _rayPosition;
    
    public PlayerCustomRaycast(TerrainGenerationData tgd, Camera3D camera, byte[] yHeights)
    {
        _camera = camera;
        _yHeights = yHeights;
        _cellSize = tgd.CellSize;
        _rows = tgd.Rows;
        _cols = tgd.Cols;

        _maxT = Math.Max((tgd.Rows + tgd.Rows) * tgd.CellSize, (tgd.Cols + tgd.Cols) * tgd.CellSize);
        _step = tgd.CellSize / 5.0f;
    }
    
    /// Gets the row and column based on the projected ray towards the mouse position.
    /// <param name="mousePosition">Mouse screen position</param>
    /// <returns>Returns Row and Column if it hits something. Otherwise it returns null!</returns>
    public (byte?, byte?) GetRowCol(Vector2 mousePosition)
    {
        _direction = _camera.ProjectRayNormal(mousePosition);
        float t = 0;
        while (t < _maxT)
        {
            _scaledDirection = _direction * t;
            _rayPosition = _camera.Position + _scaledDirection;
            
            int row = Mathf.FloorToInt(_rayPosition.X / _cellSize);
            int col = Mathf.FloorToInt(_rayPosition.Z / _cellSize);
            
            if (row >= 0 && row < _rows && col >= 0 && col < _cols)
            {
                int index = row * _cols + col;
                if (Math.Abs(_rayPosition.Y - _yHeights[index]) < _step)
                    return ((byte)row, (byte)col);
            }
                    
            t += _step;
        }
        
        return (null, null);
    }
}