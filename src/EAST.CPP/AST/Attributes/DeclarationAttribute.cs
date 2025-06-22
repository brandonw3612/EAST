namespace EAST.CPP.AST.Attributes;

public class DeclarationAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}