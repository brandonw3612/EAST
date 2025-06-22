using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXTemporaryObjectExpr")]
public class TemporaryObjectExpression : Expression
{
    public required string ConstructorType { get; set; }
    public required Expression[] Arguments { get; set; }
    
    public new static TemporaryObjectExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as TemporaryObjectExpression)
                .Expect("Failed to cast existing AST node to TemporaryObjectExpression.", j);
        }

        var children = j.GetNullableChildren();
        TemporaryObjectExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            ConstructorType = TypeDeclaration.ParseFromJ(
                j.Value<JObject>("ctorType")
                    .Expect("Cannot find the constructor type of the temporary object expression.", j)
            ).TypeName,
            Arguments = children.Select(jj => Expression.ParseFromJ(jj, astNodeDict)).ToArray()
        };
        
        astNodeDict[id] = node;
        return node;
    }
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existing))
        {
            return existing;
        }

        GraphNode node = new()
        {
            Id = Id,
            Label = $"[TemporaryObject]\n({Type})\n.ctor(): {ConstructorType}",
        };
        graph.AddVertex(node);
        
        foreach (var arg in Arguments)
        {
            var argNode = arg.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new(node, argNode));
        }
        
        astNodeDict[Id] = node;
        return node;
    }
}