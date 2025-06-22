namespace EAST.CPP.AST.Enums;

public class CastOperation
{
    private static CastOperation? _unknown;
    public static CastOperation Unknown => _unknown ??= new(0);
    
    private static CastOperation? _functionToPointerDecay;
    public static CastOperation FunctionToPointerDecay => _functionToPointerDecay ??= new(1);
    
    private static CastOperation? _arrayToPointerDecay;
    public static CastOperation ArrayToPointerDecay => _arrayToPointerDecay ??= new(2);
    
    private readonly int _kind;
    private CastOperation(int kind)
    {
        _kind = kind;
    }
    
    public static bool operator ==(CastOperation ck1, CastOperation ck2) => ck1._kind == ck2._kind;
    public static bool operator !=(CastOperation ck1, CastOperation ck2) => ck1._kind != ck2._kind;

    private bool Equals(CastOperation other)
    {
        return _kind == other._kind;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((CastOperation) obj);
    }

    public override int GetHashCode() => _kind;

    public static CastOperation FromString(string k) => k switch
    {
        "FunctionToPointerDecay" => FunctionToPointerDecay,
        "ArrayToPointerDecay" => ArrayToPointerDecay,
        _ => Unknown
    };
}