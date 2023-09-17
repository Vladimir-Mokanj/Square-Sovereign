using System;
using Godot;

namespace FT.Player;

public class PlayerCustomRaycast
{
    private readonly Camera3D _camera;
    private readonly float[] _yHeights;
    private readonly float _cellSize; 

    private readonly float _maxT;
    private readonly float _step;
    private Vector3 _direction;
    private Vector3 _scaledDirection;
    private Vector3 _rayPosition;
    

    public PlayerCustomRaycast(byte rows, byte cols, ushort cellSize, Camera3D camera/*, float[] yHeights*/)
    {
        _camera = camera;
        //_yHeights = yHeights;
        _cellSize = cellSize;
        
        _maxT = Math.Max(rows * cellSize, cols * cellSize);
        _step = cellSize / 2.0f;
    }

    public /*(float row, float col)*/ void GetRowCol(float mouseXPosition, float mouseYPosition)
    {
        _direction = _camera.Position - _camera.ProjectPosition(new Vector2(mouseXPosition, mouseYPosition), _camera.Far);
        float t = 0;
        while (t < _maxT)
        {
            _scaledDirection = _direction * t;
            _rayPosition = _camera.Position + _scaledDirection;

            int row = Mathf.FloorToInt(_rayPosition.X / _cellSize);
            int col = Mathf.FloorToInt(_rayPosition.Z / _cellSize);
            
            GD.Print($"Row: {row}, Col: {col}");
            
            t += _step;
        }
        
        //return (-1, -1);
    }
}