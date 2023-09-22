using Godot;
using FT.Data.Items;

namespace FT.UI;

public partial class BuildingUI : Button
{
    public int ID { get; private set; } = -1;
    
    public void InitializeValues(Building building)
    {
        Icon = building.Sprite;
        ID = building.Id;
    }
}