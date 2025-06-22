using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[ValueDeclaration("ParmVarDecl")]
public class ParameterVariableDeclaration : ValueDeclaration
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    
    public new static ParameterVariableDeclaration ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as ParameterVariableDeclaration)
                .Expect("Failed to cast existing AST node to ParameterVariableDeclaration.", j);
        }
        
        ParameterVariableDeclaration node = new()
        {
            Id = id,
            Name = j.Value<string>("name")
                .Expect("Cannot find the name of the parameter variable declaration", j),
            Type = j.GetTypeName()
        };
        
        astNodeDict[id] = node;
        return node;
    }

    public override string ToString()
    {
        return $"{Name}: {Type}";
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
            Label = $"[Parameter]\n{Name}: {Type}"
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}