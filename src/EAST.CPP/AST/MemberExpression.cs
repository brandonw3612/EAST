using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("MemberExpr")]
public class MemberExpression : Expression
{
    public required Expression Base { get; set; }
    public required string Name { get; set; }
    
    public new static MemberExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as MemberExpression)
                .Expect("Failed to cast existing AST node to MemberExpression.", j);
        }
        
        var children = j.GetNullableChildren();
        MemberExpression node = new()
        {
            Id = id,
            Name = j.Value<string>("name")
                .Expect("Cannot find the name of the member expression.", j),
            Base = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find base expression in MemberExpression.", j),
                astNodeDict
            ),
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory()
        };
        
        astNodeDict[id] = node;
        return node;
    }
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existingNode))
        {
            return existingNode;
        }
        
        GraphNode node = new()
        {
            Id = Id,
            Label = $"[Member]\n.{Name}: {Type}"
        };
        graph.AddVertex(node);
        
        var baseNode = Base.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(baseNode, node));
        
        astNodeDict[Id] = node;
        return node;
    }
}