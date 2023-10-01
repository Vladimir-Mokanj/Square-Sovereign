using Godot;
using FT.TBS;

namespace FT.Camera;

public partial class CameraController : Camera3D
{
	private Vector2 _startDragPosition;
	private Vector3 _initialCameraPosition;
	private bool _isDragging;

	public override void _Ready() => 
		PlayerManager.Instance.OnStateInitialized.AddObserver(OnStateInitialized);

	private void OnStateInitialized(StateParameters State) => 
		State.IsMouseDrag.AddObserver(value => { _isDragging = value; if (value) OnDragStart(); });
	
	private void OnDragStart()
	{
		_startDragPosition = GetViewport().GetMousePosition();
		_initialCameraPosition = GlobalPosition;
	}

	public override void _Process(double delta)
	{
		if (!_isDragging)
			return;
		
		UpdateCameraPosition(GetClampedDragVector());
	}
	
	private Vector2 GetClampedDragVector()
	{
		Vector2I screenSize = GetWindow().Size;
		Vector2 currentMousePosition = GetViewport().GetMousePosition();

		float maxLeftDrag = _startDragPosition.X;
		float maxRightDrag = screenSize.X - _startDragPosition.X;
		float maxUpDrag = _startDragPosition.Y;
		float maxDownDrag = screenSize.Y - _startDragPosition.Y;

		Vector2 dragVector = currentMousePosition - _startDragPosition;
		dragVector.X = Mathf.Clamp(dragVector.X, -maxLeftDrag, maxRightDrag);
		dragVector.Y = Mathf.Clamp(dragVector.Y, -maxUpDrag, maxDownDrag);

		return dragVector;
	}
	
	private void UpdateCameraPosition(Vector2 dragVector)
	{
		Vector3 cameraMove = new Vector3(-dragVector.Y, 0, dragVector.X) * 0.2f;
		GlobalPosition = _initialCameraPosition - cameraMove;
	}
}