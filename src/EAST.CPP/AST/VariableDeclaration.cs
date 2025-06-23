using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.AST;

[ValueDeclaration("VarDecl")]
public class VariableDeclaration : ValueDeclaration
{
    public required string? VariableName { get; set; }
    public required string Type { get; set; }
    public required Expression? Initializer { get; set; }
    
    public new static VariableDeclaration ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as VariableDeclaration)
                .Expect("Failed to cast existing AST node to VariableDeclaration.", j);
        }
        
        var children = j.GetNullableChildren();
        VariableDeclaration node = new()
        {
            Id = id,
            VariableName = j.Value<string>("name"),
            Type = j.GetTypeName(),
            Initializer = children.Length > 0 ?
                Expression.ParseFromJ(
                    children.FirstOrDefault()
                        .Expect("Cannot find initializer in VariableDeclaration.", j),
                    astNodeDict
                ) : null
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
            Label = $"[Variable]\n{VariableName}: {Type}"
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        
        if (Initializer == null) return node;
        
        var childNode = Initializer.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, childNode));
        
        return node;
    }
}