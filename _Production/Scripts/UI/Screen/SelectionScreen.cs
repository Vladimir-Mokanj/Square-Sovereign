using System.Collections.Generic;
using FT.Data;
using FT.Data.Items;
using FT.TBS;
using Godot;

namespace FT.UI;

public partial class SelectionScreen : Control
{
	[Export] private PackedScene _displayUI_Prefab;
	[Export] private GridContainer _unitContainer;
	[Export] private GridContainer _upgradeContainer;
	[Export] private InfoScreen _infoScreenScreen;

	public override void _Ready() => 
		PlayerManager.Instance.OnStateInitialized.AddObserver(OnStateInitialized);

	private void OnStateInitialized(StateParameters State)
	{
		State.BuildingSelectedID.AddObserver(OnBuildingSelection);
		State.IsMouseRightDown.AddObserver(b => { if (b) Visible = false; });
	}
	
	private void OnBuildingSelection(int? value)
	{
		if (!value.HasValue)
			return;

		Visible = true;
		DisposeChildren();
		AddItemsToContainers((ItemDatabase.Get(value.Value) as Building)?.BuildingProperties);
	}
	
	private void AddItemsToContainers(IEnumerable<int> buildingProperties)
	{
		foreach (int buildingProperty in buildingProperties)
		{
			Item item = ItemDatabase.Get(buildingProperty);
			if (item is not Upgrade and not Unit)
				continue;

			if (_displayUI_Prefab.Instantiate() is not DisplayUI uiItem)
				continue;

			uiItem.InitializeValues(item);
			AddChildToContainerAndAttachEvents(uiItem, item);
		}
	}

	private void AddChildToContainerAndAttachEvents(DisplayUI uiItem, Item item)
	{
		GridContainer targetContainer = null;
		if (item is Unit unitItem)
		{
			targetContainer = _unitContainer;
			uiItem.MouseEntered += () => _infoScreenScreen?.ShowDisplayPanel(item.Sprite, item.DisplayName, unitItem.Description);
		}
		else if (item is Upgrade upgradeItem)
		{
			targetContainer = _upgradeContainer;
			uiItem.MouseEntered += () => _infoScreenScreen?.ShowDisplayPanel(item.Sprite, item.DisplayName, upgradeItem.Description);
		}

		if (targetContainer != null)
		{
			targetContainer.AddChild(uiItem);
			targetContainer.Columns = Mathf.CeilToInt(targetContainer.GetChildCount() / 2.0f);
		}

		uiItem.MouseExited += () => _infoScreenScreen?.HideInfoPanel();
	}

	private void DisposeChildren()
	{
		foreach (Node child in _unitContainer.GetChildren())
		{
			_unitContainer.RemoveChild(child);
			child.Dispose();
		}

		foreach (Node child in _upgradeContainer.GetChildren())
		{
			_upgradeContainer.RemoveChild(child);
			child.Dispose();
		}
	}
}