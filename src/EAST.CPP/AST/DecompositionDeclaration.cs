using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[ValueDeclaration("DecompositionDecl")]
public class DecompositionDeclaration : VariableDeclaration
{
    public required BindingDeclaration[] Bindings { get; set; }
    
    public new static DecompositionDeclaration ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as DecompositionDeclaration)
                .Expect("Failed to cast existing AST node to DecompositionDeclaration.", j);
        }
        
        var children = j.GetNullableChildren();
        DecompositionDeclaration node = new()
        {
            Id = id,
            VariableName = j.Value<string>("name"),
            Type = j.GetTypeName(),
            Initializer = children.Length > 0 ?
                Expression.ParseFromJ(
                    children.FirstOrDefault()
                        .Expect("Cannot find initializer in VariableDeclaration.", j),
                    astNodeDict
                ) : null,
            Bindings = children.Skip(1)
                .Select(jj => BindingDeclaration.ParseFromJ(jj, astNodeDict))
                .ToArray()
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
            Label = $"[Decomposition]\n{VariableName ?? "_"}: {Type}"
        };
        graph.AddVertex(node);
        
        if (Initializer != null)
        {
            var childNode = Initializer.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new GraphEdge(node, childNode)
            {
                Label = "Init"
            });
        }
        
        foreach (var binding in Bindings)
        {
            var childNode = binding.AddToGraph(graph, astNodeDict);
            graph.AddEdge(new GraphEdge(node, childNode)
            {
                Label = "Bind"
            });
        }
        
        astNodeDict[Id] = node;
        return node;
    }
}