using EAST.CPP.AST;
using EAST.CPP.Graph;
using QuikGraph;

namespace EAST.CPP.External;

public class ExternalFunctionDeclaration : FunctionDeclaration
{
    public static ExternalFunctionDeclaration Create(string name, string type,
        ParameterVariableDeclaration[] parameters) => new()
    {
        Id = "external_" + name,
        Name = name,
        Type = type,
        Parameters = parameters,
        Body = EmptyStatement.Create()
    };
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph,
        Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existingNode))
        {
            return existingNode;
        }
        
        var label = "[External] " + Name;
        if (Parameters.Length > 0)
        {
            label += string.Concat(Parameters.Select(p => "\n" + p));
        }
        
        GraphNode node = new()
        {
            Id = Id,
            Label = label
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}