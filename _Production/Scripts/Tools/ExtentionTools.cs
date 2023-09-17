using Godot;

namespace FT.Tools;

public static class ExtentionTools
{
    public static T CreateNode<T>(string nodeName, string nodeGroup = "", Node parent = null) where T : Node, new()
    {
        T node = new();
        node.Name = nodeName;

        if (!string.IsNullOrEmpty(nodeGroup))
            node.AddToGroup(nodeGroup);

        parent?.CallDeferred(method: "add_child", node);
        return node;
    }
    
    public static void CreateMeshNode(Vector3 pos, Node parent)
    {
        MeshInstance3D node = new();
        // Create a new SphereMesh and set its parameters
        SphereMesh sphere = new();
        sphere.Radius = 0.1f;
        sphere.Height = 0.1f;

        // Assign the SphereMesh to the MeshInstance3D
        node.Mesh = sphere;
        node.Position = pos;
        
        parent.AddChild(node);
    }
}