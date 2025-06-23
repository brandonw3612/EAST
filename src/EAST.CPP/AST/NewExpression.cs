using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[Expression("CXXNewExpr")]
public class NewExpression : Expression
{
    public required FunctionDeclaration NewDeclaration { get; set; }
    public required FunctionDeclaration DeleteDeclaration { get; set; }
    public required Expression Initializer { get; set; }
    
    public new static NewExpression ParseFromJ(Newtonsoft.Json.Linq.JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as NewExpression)
                .Expect("Failed to cast existing AST node to NewExpression.", j);
        }

        var children = j.GetChildren();
        NewExpression node = new()
        {
            Id = id,
            Type = j.GetTypeName(),
            ValueCategory = j.GetExpressionValueCategory(),
            NewDeclaration = FunctionDeclaration.ParseFromJ(
                j.Value<JObject>("operatorNewDecl")
                    .Expect("Cannot find the new declaration of the new expression.", j),
                astNodeDict
            ),
            DeleteDeclaration = FunctionDeclaration.ParseFromJ(
                j.Value<JObject>("operatorDeleteDecl")
                    .Expect("Cannot find the delete declaration of the new expression.", j),
                astNodeDict
            ),
            Initializer = Expression.ParseFromJ(
                children.FirstOrDefault()
                    .Expect("Cannot find the initializer of the new expression.", j),
                astNodeDict
            )
        };

        astNodeDict[id] = node;
        return node;
    }
    
    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existing))
        {
            return existing;
        }

        GraphNode node = new()
        {
            Id = Id,
            Label = $"[New]\n({Type})\nnew: {NewDeclaration.Name}\ndelete: {DeleteDeclaration.Name}",
        };
        graph.AddVertex(node);
        
        var initializerNode = Initializer.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, initializerNode));
        
        astNodeDict[Id] = node;
        return node;
    }
}