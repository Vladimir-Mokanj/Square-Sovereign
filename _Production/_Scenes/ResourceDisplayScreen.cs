using Godot;
using System;
using FT.Managers;

public partial class ResourceDisplayScreen : Control
{
	[Export] private TextureRect _resourceTexture;
	
	private (byte row, byte col, ResourceType resourceType)[] resourceData;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		resourceData = CellManager.GetResourceData(10, 10);
		foreach (var VARIABLE in resourceData)
		{
			GD.PrintErr($"row: {VARIABLE.row}, col: {VARIABLE.col}, resource: {VARIABLE.resourceType}");	
		}
	}

	public override void _Process(double delta)
	{
		//foreach ((byte row, byte col, ResourceType resourceType) data in resourceData)
		//{
			// Convert the 3D position of the resource to 2D screen coordinates
			Vector3 resourcePos3D = new(resourceData[0].row * 20 + 10, 0, resourceData[0].col * 20 + 10);
			Vector2 resourcePos2D = GetViewport().GetCamera3D().UnprojectPosition(resourcePos3D);

			// Update TextureRect position
			_resourceTexture.Position = resourcePos2D - new Vector2(25, 50); //- _resourceTexture.Position / 2;

			// Scale TextureRect based on camera zoom or any other metric
			//float scale = 1.0f / GetViewport().GetCamera3D().Fov; // Assuming uniform zoom
			//_resourceTexture.CustomMinimumSize = new Vector2(50, 50) * scale; // Adjust the size as needed
			//}
	}
}
