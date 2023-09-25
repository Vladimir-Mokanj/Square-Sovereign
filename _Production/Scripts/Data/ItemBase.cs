using Godot;

namespace FT.Data;

public partial class ItemBase : Resource
{
    [Export, ReadOnly] public string Name { get; protected set; }
    
    public int Id => GetHashCode();
    
    public override int GetHashCode() => NameToId(Name);
    
    public override bool Equals(object other) => 
        other is ItemBase identifier && identifier.Id == Id;
    
    public static int NameToId(string str)
    {
        unchecked
        {
            int hash1 = 5381;
            int hash2 = hash1;

            for(int i = 0; i < str.Length && str[i] != '\0'; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ str[i];
                if (i == str.Length - 1 || str[i+1] == '\0')
                    break;
                hash2 = ((hash2 << 5) + hash2) ^ str[i+1];
            }

            return hash1 + (hash2*1566083941);
        }
    }
}