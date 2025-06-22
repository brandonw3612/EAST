using EAST.CPP.AST.Attributes;
using EAST.CPP.AST.Enums;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.AST;

[Expression("IntegerLiteral")]
public class IntegerLiteral : Expression
{
    public required Int128 Value { get; set; }
    
    public new static IntegerLiteral ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as IntegerLiteral)
                .Expect("Failed to cast existing AST node to IntegerLiteral.", j);
        }
        
        IntegerLiteral node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Value = Int128.Parse(
                j.Value<string>("value")
                    .Expect("Cannot find value of the integer literal.", j)
            ),
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
            Label = Value.ToString(),
            Shape = GraphvizVertexShape.Ellipse
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}