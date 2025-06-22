namespace EAST.CPP.AST.Enums;

public class UnaryOperation
{
    private static UnaryOperation? _unknown;
    public static UnaryOperation Unknown => _unknown ??= new("[UnknownUnaryOperation]");
    
    private static UnaryOperation? _negate;
    public static UnaryOperation Negate => _negate ??= new("!");
    
    private static UnaryOperation? _addressOf;
    public static UnaryOperation AddressOf => _addressOf ??= new("&");
    
    private static UnaryOperation? _dereference;
    public static UnaryOperation Dereference => _dereference ??= new("*");
    
    private static UnaryOperation? _selfIncrement;
    public static UnaryOperation SelfIncrement => _selfIncrement ??= new("++");
    
    private static UnaryOperation? _selfDecrement;
    public static UnaryOperation SelfDecrement => _selfDecrement ??= new("--");
    
    private readonly string _op;
    private UnaryOperation(string op)
    {
        _op = op;
    }
    
    public static bool operator ==(UnaryOperation op1, UnaryOperation op2) => op1._op == op2._op;
    public static bool operator !=(UnaryOperation op1, UnaryOperation op2) => op1._op != op2._op;
    
    private bool Equals(UnaryOperation other)
    {
        return _op == other._op;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((UnaryOperation) obj);
    }
    
    public override int GetHashCode() => _op.GetHashCode();
    
    public static UnaryOperation FromString(string op) => op switch
    {
        "!" => Negate,
        "&" => AddressOf,
        "*" => Dereference,
        "++" => SelfIncrement,
        "--" => SelfDecrement,
        _ => Unknown
    };

    public override string ToString() => _op;
}