using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXBindTemporaryExpr")]
public class BindTemporaryExpression : Expression
{
    public required DestructorDeclaration Destructor { get; set; }
    public required Expression Sub { get; set; }
    
    public new static BindTemporaryExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as BindTemporaryExpression)
                .Expect("Failed to cast existing AST node to BindTemporaryExpression.", j);
        }

        var children = j.GetNullableChildren();
        BindTemporaryExpression node = new()
        {
            Id = id,
            Destructor = DestructorDeclaration.ParseFromJ(
                j.Value<JObject>("dtor")
                    .Expect("Cannot find destructor declaration in the bind temporary expression.", j),
                astNodeDict
            ),
            Sub = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find sub-expression in BindTemporaryExpression.", j),
                astNodeDict
            ),
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory()
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
            Label = $"[BindTemporary]\n{Type}"
        };
        graph.AddVertex(node);

        var subExpressionNode = Sub.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, subExpressionNode)
        {
            Label = "Sub"
        });

        var destructorNode = Destructor.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, destructorNode)
        {
            Label = "Dtor"
        });

        astNodeDict[Id] = node;
        return node;
    }
}