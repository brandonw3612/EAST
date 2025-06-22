using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.AST;

[Expression("CharacterLiteral")]
public class CharacterLiteral : Expression
{
    public required char Character { get; set; }
    
    public new static CharacterLiteral ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as CharacterLiteral)
                .Expect("Internal error: Cannot cast existing AST node to CharacterLiteral.", j);
        }
        
        CharacterLiteral node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Character = (char) j.Value<int>("value")
                .Expect("Cannot find character value in character literal.", j)
        };
        
        astNodeDict[id] = node;
        return node;
    }
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph,
        Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existing))
        {
            return existing;
        }
        
        GraphNode node = new()
        {
            Id = Id,
            Label = $"'{Character}'",
            Shape = GraphvizVertexShape.Ellipse
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}