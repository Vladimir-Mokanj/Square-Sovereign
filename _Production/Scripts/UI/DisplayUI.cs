using FT.Data;
using Godot;

namespace FT.UI;

public partial class DisplayUI : Button
{
    public int ID { get; private set; } = -1;
    
    public void InitializeValues(Item building)
    {
        Icon = building.Sprite;
        ID = building.Id;
    }
}