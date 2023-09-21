using Godot;
using FT.Data.Items;

namespace FT.UI;

public partial class BuildingUI : Button
{
    private int _id = -1;

    public override void _Ready()
    {
        Connect(signal: "pressed", new Callable(this, nameof(Hello)));
    }

    private void Hello()
    {
        GD.Print("Hello");
    }

    public void InitializeValues(Building building)
    {
        Icon = building.Sprite;
        _id = building.Id;
    }
}
