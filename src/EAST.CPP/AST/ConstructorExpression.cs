using EAST.CPP.AST.Attributes;
using EAST.CPP.AST.Enums;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXConstructExpr")]
public class ConstructorExpression : Expression
{
    public required string ConstructorType { get; set; }
    public required bool HadMultipleCandidates { get; set; }
    public required ConstructionKind ConstructionKind { get; set; }
    public required Expression[] Arguments { get; set; }
    
    public new static ConstructorExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ConstructorExpression)
                .Expect("Failed to cast existing AST node to ConstructorExpression.", j);
        }
        
        ConstructorExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            ConstructorType = TypeDeclaration.ParseFromJ(
                j.Value<JObject>("ctorType")
                    .Expect("Cannot find constructor type in the constructor expression.", j)
            ).TypeName,
            HadMultipleCandidates = j.Value<bool>("hadMultipleCandidates")
                .Expect("Cannot find 'hadMultipleCandidates' in the constructor expression.", j),
            ConstructionKind = ConstructionKind.FromString(
                j.Value<string>("constructionKind")
                    .Expect("Cannot find 'constructionKind' in the constructor expression.", j)
            ),
            Arguments = j.GetNullableChildren()
                .Select(jj => Expression.ParseFromJ(jj, astNodeDict))
                .ToArray()
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
            Label = $"[.ctor]\n({Type})"
        };
        graph.AddVertex(node);

        foreach (var arg in Arguments)
        {
            var argNode = arg.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new GraphEdge(node, argNode));
        }
        
        astNodeDict[Id] = node;
        return node;
    }
}