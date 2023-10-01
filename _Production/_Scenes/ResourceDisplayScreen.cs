using System.Collections.Generic;
using System.Linq;
using FT.Data;
using Godot;
using FT.Managers;
using FT.TBS;

namespace FT.UI;

public partial class ResourceDisplayScreen : Control
{
	[Export] private PackedScene _resourcePackedScene;
	[Export] private CompressedTexture2D[] _resourceTexture;

	private HashSet<(byte row, byte col, ResourceType resourceType)> _cellData = new();
	private readonly List<TextureRect> _resourceTexturePool = new();

	private Camera3D _camera3D;
	private bool _isDragging;
	

	public override void _Ready()
	{
		_camera3D = GetViewport().GetCamera3D();
		_cellData = CellManager.GetResourceData(100, 100).ToHashSet();
		InitializeResources();
		PlayerManager.Instance.OnStateInitialized.AddObserver(OnStateInitialized);
	}

	private void OnStateInitialized(StateParameters State)
	{
		State.IsMouseDrag.AddObserver(b => _isDragging = b);
	}

	private void InitializeResources()
	{
		Vector2 textureSize = new(40, 40);
		
		foreach ((byte row, byte col, ResourceType resourceType) data in _cellData)
		{
			Vector3 worldPosition = new(data.row * 20 + 10, 0, data.col * 20 + 10);
			Vector2 screenPosition = _camera3D.UnprojectPosition(worldPosition);
			Rect2 textureRectGlobal = new(screenPosition, textureSize);

			if (!GetViewportRect().Intersects(textureRectGlobal))
				continue;
			
			TextureRect textureRect = _resourcePackedScene.Instantiate<TextureRect>();
			textureRect.GlobalPosition = screenPosition;
			textureRect.Texture = ItemDatabase.Get<FT.Data.Items.General.Resource>(data.resourceType.ToString())?.Sprite;

			AddChild(textureRect);
				
			_resourceTexturePool.Add(textureRect);
		}
	}
	
	public override void _Process(double delta)
	{
		if (!Visible || !_isDragging)
			return;

		UpdateResourceTextures();
	}

	private void UpdateResourceTextures()
	{
		Vector2 textureSize = new(40, 40);
		int index = 0;

		foreach ((byte row, byte col, ResourceType resourceType) data in _cellData)
		{
			Vector3 worldPosition = new(data.row * 20 + 10, 0, data.col * 20 + 10);
			Vector2 screenPosition = _camera3D.UnprojectPosition(worldPosition);
			Rect2 textureRectGlobal = new(screenPosition - textureSize / 2, textureSize);

			if (!GetViewportRect().Intersects(textureRectGlobal))
				continue;

			if (index < _resourceTexturePool.Count)
			{
				UpdateExistingTextureRect(index, screenPosition, data.resourceType);
				index++;
				continue;
			}
			
			CreateNewTextureRect(screenPosition, data.resourceType);
		}
		
		HideRemainingTextureRects(index);
	}
	
	private void UpdateExistingTextureRect(int index, Vector2 screenPosition, ResourceType resourceType)
	{
		TextureRect existingTextureRect = _resourceTexturePool[index];
		existingTextureRect.Visible = true;
		existingTextureRect.Texture = ItemDatabase.Get<FT.Data.Items.General.Resource>(resourceType.ToString())?.Sprite;
		existingTextureRect.GlobalPosition = screenPosition;
	}

	private void CreateNewTextureRect(Vector2 screenPosition, ResourceType resourceType)
	{
		TextureRect newTextureRect = _resourcePackedScene.Instantiate<TextureRect>();
		newTextureRect.Texture = ItemDatabase.Get<FT.Data.Items.General.Resource>(resourceType.ToString())?.Sprite;
		newTextureRect.GlobalPosition = screenPosition;
		_resourceTexturePool.Add(newTextureRect);
		AddChild(newTextureRect);
	}

	private void HideRemainingTextureRects(int startIndex)
	{
		for (int i = startIndex; i < _resourceTexturePool.Count; i++)
			_resourceTexturePool[i].Visible = false;
	}
}