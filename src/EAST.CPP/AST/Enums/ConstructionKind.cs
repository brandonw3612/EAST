namespace EAST.CPP.AST.Enums;

public class ConstructionKind
{
    private static ConstructionKind? _complete;
    public static ConstructionKind Complete => _complete ??= new(0);
    
    private static ConstructionKind? _nonVirtualBase;
    public static ConstructionKind NonVirtualBase => _nonVirtualBase ??= new(1);
    
    private static ConstructionKind? _virtualBase;
    public static ConstructionKind VirtualBase => _virtualBase ??= new(2);
    
    private static ConstructionKind? _delegating;
    public static ConstructionKind Delegating => _delegating ??= new(3);
    
    private readonly int _kind;
    private ConstructionKind(int kind)
    {
        _kind = kind;
    }
    
    public static bool operator ==(ConstructionKind ck1, ConstructionKind ck2) => ck1._kind == ck2._kind;
    public static bool operator !=(ConstructionKind ck1, ConstructionKind ck2) => ck1._kind != ck2._kind;
    
    private bool Equals(ConstructionKind other)
    {
        return _kind == other._kind;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((ConstructionKind) obj);
    }
    
    public override int GetHashCode() => _kind;
    
    public static ConstructionKind FromString(string k) => k.ToLower() switch
    {
        "complete" => Complete,
        "nonvirtualbase" => NonVirtualBase,
        "virtualbase" => VirtualBase,
        "delegating" => Delegating,
        _ => throw new NotSupportedException("Unknown construction kind: " + k)
    };
}