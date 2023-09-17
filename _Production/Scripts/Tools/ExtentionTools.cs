using Godot;

namespace FT.Tools;

public static class ExtensionTools
{
    public static T CreateNode<T>(string nodeName, string nodeGroup = "", Node parent = null) where T : Node, new()
    {
        T node = new();
        node.Name = nodeName;

        if (!string.IsNullOrEmpty(nodeGroup))
            node.AddToGroup(nodeGroup);

        parent?.CallDeferred("add_child", node);
        return node;
    }
}