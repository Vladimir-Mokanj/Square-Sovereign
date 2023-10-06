using FT.Data;
using Godot;

namespace FT.UI;

public partial class DisplayUI : Button
{
    [Export] private Label _hotkeyLabel;
    public int Id { get; private set; } = -1;

    public void InitializeValues(Item building, string hotkey = "")
    {
        Icon = building.Sprite;
        Id = building.Id;

        _hotkeyLabel.Visible = hotkey == "" ? !Visible : Visible;
        _hotkeyLabel.Text = hotkey;
    }
}