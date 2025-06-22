using EAST.CPP.AST.Attributes;
using EAST.CPP.AST.Enums;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("ImplicitCastExpr")]
public class ImplicitCastExpression : Expression
{
    public required CastOperation CastOperation { get; set; }
    
    public required Expression CastedValue { get; set; }
    
    public new static ImplicitCastExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ImplicitCastExpression)
                .Expect("Failed to cast existing AST node to ImplicitCastExpression.", j);
        }
        
        ImplicitCastExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            CastOperation = CastOperation.FromString(
                j.Value<string>("castKind")
                    .Expect("Cannot find the cast kind in the implicit cast expression", j)
            ),
            CastedValue = Expression.ParseFromJ(
                j.GetChildren()
                    .FirstOrDefault()
                    .Expect("Cannot find the casted value in the implicit cast expression", j),
                astNodeDict
            )
        };
        
        astNodeDict[id] = node;
        return node;
    }

    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph,
        Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existingNode))
        {
            return existingNode;
        }
        
        GraphNode node = new()
        {
            Id = Id,
            Label = $"[ImplicitCast]\n({Type})"
        };
        graph.AddVertex(node);
        
        var castedValueNode = CastedValue.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, castedValueNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}