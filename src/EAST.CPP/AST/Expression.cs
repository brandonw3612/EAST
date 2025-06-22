using System.Diagnostics.CodeAnalysis;
using EAST.CPP.AST.Attributes;
using EAST.CPP.AST.Enums;
using EAST.CPP.Extensions;
using Newtonsoft.Json.Linq;

namespace EAST.CPP.AST;

public abstract class Expression : Statement
{
    public required string Type { get; set; }
    public required ValueCategory ValueCategory { get; set; }

    public new static Expression? ParseNullableFromJ(JObject j, Dictionary<string, object> astNodeDict) =>
        j.Value<string>("kind") is null ? null : ParseFromJ(j, astNodeDict);
    
    public new static Expression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as Expression)
                .Expect("Failed to cast existing AST node to Expression.", j);
        }
        
        var kind = j.Value<string>("kind")
            .Expect("Cannot find the kind of the expression.", j);
        if (DeclaredTypes.ExpressionTypes.TryGetValue(kind, out var type))
        {
            return (type.GetMethod("ParseFromJ")?.Invoke(null, [j, astNodeDict]) as Expression)
                .Expect("Internal error: Cannot parse expression from JSON.", j);
        }
        throw new NotSupportedException($"Unsupported expression type: {kind}.\nJSON content: {j}");
    }
}