using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[ValueDeclaration("BindingDecl")]
public class BindingDeclaration : ValueDeclaration
{
    public required string Name { get; set; }
    public required Expression BindingExpression { get; set; }
    
    public new static BindingDeclaration ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as BindingDeclaration)
                .Expect("Failed to cast existing AST node to BindingDeclaration.", j);
        }
        
        var children = j.GetChildren();
        BindingDeclaration node = new()
        {
            Id = id,
            Name = j.Value<string>("name")
                .Expect("Cannot find name in the binding declaration.", j),
            BindingExpression = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the holding variable in the binding declaration.", j)
                    .GetChildren()
                    .FirstOrDefault()
                    .Expect("Cannot find binding expression in the binding declaration.", j),
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
        
        var node = new GraphNode
        {
            Id = Id,
            Label = $"[Binding]\n{Name}: {BindingExpression.Type}"
        };
        graph.AddVertex(node);
        
        var childNode = BindingExpression.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, childNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}