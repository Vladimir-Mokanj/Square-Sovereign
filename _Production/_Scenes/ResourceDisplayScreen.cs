using FT.Data;
using Godot;
using FT.Managers;

namespace FT.UI;

public partial class ResourceDisplayScreen : Control
{
	[Export] private PackedScene _resourcePackedScene;
	[Export] private CompressedTexture2D[] _resourceTexture;
	
	private (TextureRect textureRect, Vector3 texturePosition)[] resourceData;
	public override void _Ready()
	{
		(byte row, byte col, ResourceType resourceType)[] cellData = CellManager.GetResourceData(100, 100);
		resourceData = new (TextureRect, Vector3)[cellData.Length];
		for (int i = 0; i < cellData.Length; i++)
		{
			TextureRect resourceTexture = _resourcePackedScene.Instantiate<TextureRect>();
			resourceTexture.Texture = ItemDatabase.Get<FT.Data.Items.Resource>(cellData[i].resourceType.ToString())?.Sprite;

			Vector3 resource3DPosition = new(cellData[i].row * 20 + 10, 0, cellData[i].col * 20 + 10);

			resourceData[i] = (resourceTexture, resource3DPosition);
			AddChild(resourceTexture);
		}
	}

	public override void _Process(double delta)
	{
		foreach ((TextureRect textureRect, Vector3 texturePosition) data in resourceData)
			data.textureRect.Position = GetViewport().GetCamera3D().UnprojectPosition(data.texturePosition) - new Vector2(25, 50);
	}
}