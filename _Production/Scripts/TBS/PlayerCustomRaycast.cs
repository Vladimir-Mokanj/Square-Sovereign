using System;
using Godot;

namespace FT.Player;

public class PlayerCustomRaycast
{
    private static PlayerCustomRaycast _instance;
    
    private readonly Camera3D _camera;
    private readonly byte[] _yHeights;
    private readonly byte _cellSize;
    private readonly byte _rows;
    private readonly byte _cols;

    private readonly float _maxT;
    private readonly float _step;
    private readonly float _startStep;
    private Vector3 _direction;
    private Vector3 _scaledDirection;
    private Vector3 _rayPosition;
    private readonly Vector2 _originalSize;
    private Vector2 _currentSize;
    private float _aspectRationCorrection = 1.0f;
    
    
    public PlayerCustomRaycast(byte rows, byte cols, Camera3D camera, byte[] yHeights, Viewport viewport)
    {
        _instance = this;
        
        _camera = camera;
        _yHeights = yHeights;
        _cellSize = 20;
        _rows = rows;
        _cols = cols;
        _startStep = camera.Near;

        _maxT = Math.Max((rows + rows) * 20, (cols + cols) * 20);
        _step = 20.0f / 5.0f;
        
        _originalSize = viewport.GetWindow().Size;
        _currentSize = viewport.GetWindow().Size;
        viewport.SizeChanged += () =>
        {
            _currentSize = viewport.GetWindow().Size;
            _aspectRationCorrection = _instance._currentSize.Aspect() / _instance._originalSize.Aspect();
        };
    }

    /// Gets the row and column based on the projected ray towards the mouse position.
    /// <param name="mousePosition">Mouse screen position</param>
    /// <returns>Returns Row and Column if it hits something. Otherwise it returns null!</returns>
    public static (byte?, byte?) GetRowCol(Vector2 mousePosition)
    {
        float aspectRatioX = _instance._currentSize.X / _instance._originalSize.X;
        float aspectRatioY = _instance._currentSize.Y / _instance._originalSize.Y;
        
        Vector2 adjustedMousePos = new(
            mousePosition.X / aspectRatioX * _instance._aspectRationCorrection,
            mousePosition.Y / aspectRatioY
        );
        
        _instance._direction = _instance._camera.ProjectRayNormal(adjustedMousePos);

        float t = _instance._startStep;
        while (t < _instance._maxT)
        {
            _instance._scaledDirection = _instance._direction * t;
            _instance._rayPosition = _instance._camera.Position + _instance._scaledDirection;
            
            int row = Mathf.FloorToInt(_instance._rayPosition.X / _instance._cellSize);
            int col = Mathf.FloorToInt(_instance._rayPosition.Z / _instance._cellSize);
            
            if (row >= 0 && row < _instance._rows && col >= 0 && col < _instance._cols)
            {
                int index = row * _instance._cols + col;
                if (Math.Abs(_instance._rayPosition.Y - _instance._yHeights[index]) < _instance._step)
                    return ((byte)row, (byte)col);
            }
                    
            t += _instance._step;
        }
        
        return (null, null);
    }
}