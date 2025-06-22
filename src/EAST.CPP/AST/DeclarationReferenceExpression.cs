using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.AST;

[Expression("DeclRefExpr")]
public class DeclarationReferenceExpression : Expression
{
    public required ValueDeclaration Declaration { get; set; }

    public new static DeclarationReferenceExpression ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as DeclarationReferenceExpression)
                .Expect("Failed to cast existing AST node to DeclarationReferenceExpression.", j);
        }
        
        DeclarationReferenceExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            Declaration = ValueDeclaration.ParseFromJ(
                j.Value<JObject>("referencedDecl")
                    .Expect("Cannot find referenced declaration in DeclarationReferenceExpression.", j),
                astNodeDict
            )
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
            Label = "[Ref]"
        };
        graph.AddVertex(node);
        
        var decNode = Declaration.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new GraphEdge(node, decNode)
        {
            Style = GraphvizEdgeStyle.Dashed
        });
        
        astNodeDict[Id] = node;
        return node;
    }
}