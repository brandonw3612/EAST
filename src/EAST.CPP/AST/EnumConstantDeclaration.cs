using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.AST;

[ValueDeclaration("EnumConstantDecl")]
public class EnumConstantDeclaration : ValueDeclaration
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    
    public new static EnumConstantDeclaration ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as EnumConstantDeclaration)
                .Expect("Failed to cast existing AST node to EnumConstantDeclaration.", j);
        }
        
        EnumConstantDeclaration node = new()
        {
            Id = id,
            Name = j.Value<string>("name")
                .Expect("Cannot find name in the enum constant declaration.", j),
            Type = j.GetTypeName()
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
            Label = $"[EnumConstant]\n{Name}: {Type}",
            Shape = GraphvizVertexShape.Circle
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}