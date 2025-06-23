using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Statement("DeclStmt")]
public class DeclarationStatement : Statement
{
    public required Declaration[] Declarations { get; set; }
    
    public new static DeclarationStatement ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as DeclarationStatement)
                .Expect("Failed to cast existing AST node to DeclarationStatement.", j);
        }
        
        var children = j.GetChildren();
        DeclarationStatement node = new()
        {
            Id = id,
            Declarations = children.Select(jj => Declaration.ParseFromJ(jj, astNodeDict))
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
            Label = "[Declaration]"
        };
        graph.AddVertex(node);
            
        foreach (var declaration in Declarations)
        {
            var childNode = declaration.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new(node, childNode));
        }
            
        astNodeDict[Id] = node;
        return node;
    }
}