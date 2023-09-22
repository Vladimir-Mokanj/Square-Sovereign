using Godot;

namespace FT.UI;

public partial class BuildingInfo : Panel
{
    [Export] private TextureRect _buildingTexture;
    [Export] private Label _buildingDisplayName;
    [Export] private Label _buildingDisplayDescription;

    public override void _Ready() => Visible = false;

    public void SetDisplayValues(Texture2D texture, string displayName, string displayDescription)
    {
        bool isVisible = texture != null || !string.IsNullOrEmpty(displayName) || !string.IsNullOrEmpty(displayDescription);
        Visible = isVisible;

        if (isVisible == false)
            return;

        _buildingTexture.Texture = texture;
        _buildingDisplayName.Text = displayName;
        _buildingDisplayDescription.Text = displayDescription;
    }
}