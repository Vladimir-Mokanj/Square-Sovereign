using Godot;
using FT.Data;
using FT.Data.Items;

namespace FT.UI;

public partial class BuildingScreen : Control
{
	[Export] private PackedScene _buildingUI_Prefab;
	[Export] private Control _buildingControlNode;
	
	public override void _Ready()
	{
		InitializeBuildings();
	}

	private void InitializeBuildings()
	{
		Building[] buildings = ItemDatabase.Database.GetAllOfType<Building>();
		foreach (Building building in buildings)
		{
			BuildingUI uiItem = _buildingUI_Prefab.Instantiate() as BuildingUI;
			uiItem?.InitializeValues(building);
			_buildingControlNode.AddChild(uiItem);
		}
	}
}