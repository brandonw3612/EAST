namespace EAST.CPP.AST.Enums;

public class BinaryOperation
{
    private static BinaryOperation? _unknown;
    public static BinaryOperation Unknown => _unknown ??= new("[UnknownBinaryOperation]");
    
    private static BinaryOperation? _add;
    public static BinaryOperation Add => _add ??= new("+");
    
    private static BinaryOperation? _sub;
    public static BinaryOperation Sub => _sub ??= new("-");
    
    private static BinaryOperation? _mul;
    public static BinaryOperation Mul => _mul ??= new("*");
    
    private static BinaryOperation? _div;
    public static BinaryOperation Div => _div ??= new("/");
    
    private static BinaryOperation? _rem;
    public static BinaryOperation Rem => _rem ??= new("%");
    
    private static BinaryOperation? _lt;
    public static BinaryOperation Lt => _lt ??= new("<");
    
    private static BinaryOperation? _gt;
    public static BinaryOperation Gt => _gt ??= new(">");
    
    private static BinaryOperation? _le;
    public static BinaryOperation Le => _le ??= new("<=");

    private static BinaryOperation? _ge;
    public static BinaryOperation Ge => _ge ??= new(">=");
    
    private static BinaryOperation? _eq;
    public static BinaryOperation Eq => _eq ??= new("==");
    
    private static BinaryOperation? _ne;
    public static BinaryOperation Ne => _ne ??= new("!=");
    
    private static BinaryOperation? _assign;
    public static BinaryOperation Assign => _assign ??= new("=");
    
    private static BinaryOperation? _mulAssign;
    public static BinaryOperation MulAssign => _mulAssign ??= new("*=");
    
    private static BinaryOperation? _divAssign;
    public static BinaryOperation DivAssign => _divAssign ??= new("/=");
    
    private static BinaryOperation? _remAssign;
    public static BinaryOperation RemAssign => _remAssign ??= new("%=");
    
    private static BinaryOperation? _addAssign;
    public static BinaryOperation AddAssign => _addAssign ??= new("+=");
    
    private static BinaryOperation? _subAssign;
    public static BinaryOperation SubAssign => _subAssign ??= new("-=");
    
    private static BinaryOperation? _shlAssign;
    public static BinaryOperation ShlAssign => _shlAssign ??= new("<<=");
    
    private static BinaryOperation? _shrAssign;
    public static BinaryOperation ShrAssign => _shrAssign ??= new(">>=");
    
    private static BinaryOperation? _andAssign;
    public static BinaryOperation AndAssign => _andAssign ??= new("&=");
    
    private static BinaryOperation? _xorAssign;
    public static BinaryOperation XorAssign => _xorAssign ??= new("^=");
    
    private static BinaryOperation? _orAssign;
    public static BinaryOperation OrAssign => _orAssign ??= new("|=");
    
    private readonly string _op;
    private BinaryOperation(string op)
    {
        _op = op;
    }

    public override string ToString() => _op;
    
    public static bool operator ==(BinaryOperation op1, BinaryOperation op2) => op1._op == op2._op;
    public static bool operator !=(BinaryOperation op1, BinaryOperation op2) => op1._op != op2._op;
    
    private bool Equals(BinaryOperation other)
    {
        return _op == other._op;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((BinaryOperation) obj);
    }
    
    public override int GetHashCode() => _op.GetHashCode();
    
    public static BinaryOperation FromString(string op) => op switch
    {
        "+" => Add,
        "-" => Sub,
        "*" => Mul,
        "/" => Div,
        "%" => Rem,
        "<" => Lt,
        ">" => Gt,
        "<=" => Le,
        ">=" => Ge,
        "==" => Eq,
        "!=" => Ne,
        "=" => Assign,
        "*=" => MulAssign,
        "/=" => DivAssign,
        "%=" => RemAssign,
        "+=" => AddAssign,
        "-=" => SubAssign,
        "<<=" => ShlAssign,
        ">>=" => ShrAssign,
        "&=" => AndAssign,
        "^=" => XorAssign,
        "|=" => OrAssign,
        _ => Unknown
    };
}