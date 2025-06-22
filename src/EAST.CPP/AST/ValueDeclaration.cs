using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;

namespace EAST.CPP.AST;

public abstract class ValueDeclaration : Declaration, IGraphNode
{
    public new static ValueDeclaration? ParseNullableFromJ(JObject j, Dictionary<string, object> astNodeDict) =>
        j.Value<string>("kind") is null ? null : ParseFromJ(j, astNodeDict);
    
    public new static ValueDeclaration ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ValueDeclaration)
                .Expect("Failed to cast existing AST node to ValueDeclaration.", j);
        }
        
        var kind = j.Value<string>("kind")
            .Expect("Cannot find the kind of the declaration.", j);
        if (DeclaredTypes.ValueDeclarationTypes.TryGetValue(kind, out var type))
        {
            return (type.GetMethod(nameof(ParseFromJ))?.Invoke(null, [j, astNodeDict]) as ValueDeclaration)
                .Expect("Failed to parse ValueDeclaration from JObject.", j);
        }
        throw new NotSupportedException($"Unsupported ValueDeclaration kind: {kind}");
    }
}