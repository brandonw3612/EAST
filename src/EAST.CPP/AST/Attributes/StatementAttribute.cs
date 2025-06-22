namespace EAST.CPP.AST.Attributes;

public class StatementAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}