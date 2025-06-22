using EAST.CPP.AST;
using EAST.CPP.Graph;
using QuikGraph;

namespace EAST.CPP.External;

public class EmptyStatement : Statement
{
    public static EmptyStatement Create() => new()
    {
        Id = Guid.NewGuid().ToString()
    };
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existingNode))
        {
            return existingNode;
        }
        
        var node = new GraphNode
        {
            Id = Id,
            Label = "[External]"
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}