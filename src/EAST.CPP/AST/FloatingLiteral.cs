using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("FloatingLiteral")]
public class FloatingLiteral : Expression
{
    public required double Value { get; set; }
    
    public new static FloatingLiteral ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as FloatingLiteral)
                .Expect("Failed to cast existing AST node to FloatingLiteral.", j);
        }

        var node = new FloatingLiteral
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Value = double.Parse(
                j.Value<string>("value")
                    .Expect("Cannot find the value of the floating literal.", j)
            )
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
            Label = Value.ToString()
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}