using FT.Data;
using FT.Data.Items;
using FT.TBS;
using Godot;

namespace FT.UI;

public partial class SelectionScreen : Control
{
	[Export] private PackedScene _displayUI_Prefab;
	[Export] private Control _selectionControlNode;
	[Export] private InfoScreen _infoScreenScreen;

	public override void _Ready() => 
		PlayerManager.Instance.OnStateInitialized.AddObserver(OnStateInitialized);

	private void OnStateInitialized(StateParameters State)
	{
		State.BuildingSelectedID.AddObserver(OnSelection);
		State.IsMouseRightDown.AddObserver(b => { if (b) Visible = false; });
	}

	private void OnSelection(int? value)
	{
		if (!value.HasValue) 
			return;

		Visible = true;
		foreach (Node child in _selectionControlNode.GetChildren())
			child.QueueFree();

		Building selection = ItemDatabase.Get<Building>(value.Value);
		foreach (int buildingProperty in selection.BuildingProperties)
		{
			Item item = ItemDatabase.Get(buildingProperty);
			if (item is not Upgrade and not Unit)
				continue;
			
			if (_displayUI_Prefab.Instantiate() is not BuildingUI uiItem)
				continue;
			
			uiItem.InitializeValues(item);
			_selectionControlNode.AddChild(uiItem);
			
			if (item is Unit unitItem) uiItem.MouseEntered += () => _infoScreenScreen?.ShowDisplayPanel(item.Sprite, item.DisplayName, unitItem.Description);
			else if (item is Upgrade upgradeItem) uiItem.MouseEntered += () => _infoScreenScreen?.ShowDisplayPanel(item.Sprite, item.DisplayName, upgradeItem.Description);
			
			uiItem.MouseExited += () => _infoScreenScreen?.HideInfoPanel();
		}
	}
}