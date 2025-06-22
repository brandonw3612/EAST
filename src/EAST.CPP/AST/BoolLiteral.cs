using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.AST;

[Expression("CXXBoolLiteralExpr")]
public class BoolLiteral : Expression
{
    public required bool Value { get; set; }
    
    public new static BoolLiteral ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as BoolLiteral)
                .Expect("Failed to cast existing AST node to BoolLiteral.", j);
        }
        
        BoolLiteral node = new()
        {
            Id = id,
            Value = j.Value<bool>("value")
                .Expect("Cannot find the value in boolean literal.", j),
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory()
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