using Godot;

namespace FT.UI;

public partial class InfoScreen : Panel
{
    [Export] private TextureRect _buildingTexture;
    [Export] private Label _buildingDisplayName;
    [Export] private Label _buildingDisplayDescription;

    public override void _Ready() => Visible = false;

    public void ShowDisplayPanel(Texture2D texture, string displayName, string displayDescription)
    {
        Visible = true;
        _buildingTexture.Texture = texture;
        _buildingDisplayName.Text = displayName;
        _buildingDisplayDescription.Text = displayDescription;
    }

    public void HideInfoPanel() => Visible = false;
}