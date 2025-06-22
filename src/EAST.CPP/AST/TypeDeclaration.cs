using EAST.CPP.Extensions;
using Newtonsoft.Json.Linq;

namespace EAST.CPP.AST;

public class TypeDeclaration
{
    private readonly string _qualifiedType;
    private readonly string? _desugaredTypeName;

    public string TypeName => _desugaredTypeName ?? _qualifiedType;
    
    private TypeDeclaration(string qualifiedType, string? desugaredTypeName)
    {
        _qualifiedType = qualifiedType;
        _desugaredTypeName = desugaredTypeName;
    }
    
    public static TypeDeclaration ParseFromJ(JObject j)
    {
        var qualifiedType = j.Value<string>("qualType").Expect("Cannot find the qualified type.", j);
        var desugaredTypeName = j.Value<string>("desugaredQualType");

        return new(qualifiedType, desugaredTypeName);
    }
}