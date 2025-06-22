namespace EAST.CPP.AST.Enums;

public class ValueCategory
{
    private static ValueCategory? _l, _pr, _x, _u;
    
    public static ValueCategory Unknown => _u ??= new(0b000);
    public static ValueCategory LeftValue => _l ??= new(0b001);
    public static ValueCategory PureRightValue => _pr ??= new(0b010);
    public static ValueCategory ExpiringValue => _x ??= new(0b100);
    
    private readonly int _category;
    
    private ValueCategory(int category) => _category = category;
    
    public bool IsLeftValue => _category is 0b001;
    public bool IsPureRightValue => _category is 0b010;
    public bool IsExpiringValue => _category is 0b100;
    
    public bool IsGeneralizedLeftValue => IsLeftValue || IsExpiringValue;
    public bool IsRightValue => IsPureRightValue || IsExpiringValue;

    public static ValueCategory FromString(string s) => s switch
    {
        "lvalue" => LeftValue,
        "prvalue" => PureRightValue,
        "xvalue" => ExpiringValue,
        _ => Unknown
    };
}