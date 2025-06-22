namespace EAST.CPP.AST.Attributes;

public static class DeclaredTypes
{
    private static Dictionary<string, Type>? _statementTypes;
    public static Dictionary<string, Type> StatementTypes
    {
        get
        {
            if (_statementTypes != null) return _statementTypes;
            _statementTypes = new Dictionary<string, Type>();
            foreach (var type in typeof(DeclaredTypes).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Statement)))
                {
                    if (type.GetCustomAttributes(typeof(StatementAttribute), true)
                            .FirstOrDefault() is StatementAttribute attribute)
                    {
                        _statementTypes[attribute.Key] = type;
                    }
                }
            }
            return _statementTypes;
        }
    }
    
    private static Dictionary<string, Type>? _expressionTypes;
    public static Dictionary<string, Type> ExpressionTypes
    {
        get
        {
            if (_expressionTypes != null) return _expressionTypes;
            _expressionTypes = new Dictionary<string, Type>();
            foreach (var type in typeof(DeclaredTypes).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Expression)))
                {
                    if (type.GetCustomAttributes(typeof(ExpressionAttribute), false)
                            .FirstOrDefault() is ExpressionAttribute attribute)
                    {
                        _expressionTypes[attribute.Key] = type;
                    }
                }
            }
            return _expressionTypes;
        }
    }
    
    private static Dictionary<string, Type>? _declarationTypes;
    public static Dictionary<string, Type> DeclarationTypes
    {
        get
        {
            if (_declarationTypes != null) return _declarationTypes;
            _declarationTypes = new Dictionary<string, Type>();
            foreach (var type in typeof(DeclaredTypes).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(Declaration)))
                {
                    if (type.GetCustomAttributes(typeof(DeclarationAttribute), false)
                            .FirstOrDefault() is DeclarationAttribute attribute)
                    {
                        _declarationTypes[attribute.Key] = type;
                    }
                }
            }
            return _declarationTypes;
        }
    }
    
    private static Dictionary<string, Type>? _valueDeclarationTypes;
    public static Dictionary<string, Type> ValueDeclarationTypes
    {
        get
        {
            if (_valueDeclarationTypes != null) return _valueDeclarationTypes;
            _valueDeclarationTypes = new Dictionary<string, Type>();
            foreach (var type in typeof(DeclaredTypes).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(ValueDeclaration)))
                {
                    if (type.GetCustomAttributes(typeof(ValueDeclarationAttribute), false)
                            .FirstOrDefault() is ValueDeclarationAttribute attribute)
                    {
                        _valueDeclarationTypes[attribute.Key] = type;
                    }
                }
            }
            return _valueDeclarationTypes;
        }
    }
}