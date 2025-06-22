using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.AST;

[Expression("CXXNullPtrLiteralExpr")]
public class NullPointerLiteral : Expression
{
    public new static NullPointerLiteral ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as NullPointerLiteral)
                .Expect("Failed to cast existing AST node to NullPointerLiteral.", j);
        }
        
        NullPointerLiteral node = new()
        {
            Id = id,
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
            Label = "nullptr",
            Shape = GraphvizVertexShape.Ellipse
        };
        graph.AddVertex(node);
        
        astNodeDict[Id] = node;
        return node;
    }
}