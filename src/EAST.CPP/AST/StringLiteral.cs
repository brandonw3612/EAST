using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.AST;

[Expression("StringLiteral")]
public class StringLiteral : Expression
{
    public required string Value { get; set; }
    
    public new static StringLiteral ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as StringLiteral)
                .Expect("Failed to cast existing AST node to StringLiteral.", j);
        }

        StringLiteral node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Value = j.Value<string>("value")
                .Expect("Cannot find value in string literal.", j)
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
            Label = Value,
            Shape = GraphvizVertexShape.Ellipse
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}