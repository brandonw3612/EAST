using EAST.CPP.AST.Attributes;
using EAST.CPP.Extensions;
using EAST.CPP.External;
using EAST.CPP.Graph;
using Newtonsoft.Json.Linq;
using QuikGraph;

namespace EAST.CPP.AST;

[ValueDeclaration("FunctionDecl")]
public class FunctionDeclaration : ValueDeclaration
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required ParameterVariableDeclaration[] Parameters { get; set; }
    public required Statement Body { get; set; }

    public new static FunctionDeclaration ParseFromJ(JObject j, Dictionary<string, object> astNodeDict)
    {
        var id = j.GetId();
        if (astNodeDict.TryGetValue(id, out var existing))
        {
            return (existing as FunctionDeclaration)
                .Expect("Failed to cast existing AST node to FunctionDeclaration.", j);
        }

        try
        {
            var children = j.GetChildren();
            FunctionDeclaration node = new()
            {
                Id = id,
                Name = j.Value<string>("name")
                    .Expect("Cannot find the name of the function declaration", j),
                Type = j.GetTypeName(),
                Parameters = children.SkipLast(1)
                    .Select(jj => ParameterVariableDeclaration.ParseFromJ(jj, astNodeDict))
                    .ToArray(),
                Body = Statement.ParseFromJ(children.Last(), astNodeDict)
            };

            astNodeDict[id] = node;
            return node;
        }
        catch (NotSupportedException)
        {
            throw;
        }
        catch
        {
            return ExternalFunctionDeclaration.Create(
                j.Value<string>("name")
                    .Expect("Cannot find the name of the external function declaration", j),
                j.GetTypeName(),
                []
            );
        }
    }

    public override GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph,
        Dictionary<string, GraphNode> astNodeDict)
    {
        if (astNodeDict.TryGetValue(Id, out var existingNode))
        {
            return existingNode;
        }
        
        var label = $"[Function]\n{Name}(): {Type}";
        if (Parameters.Length > 0)
        {
            label += string.Concat(Parameters.Select(p => "\n" + p));
        }
        
        GraphNode node = new()
        {
            Id = Id,
            Label = label
        };
        graph.AddVertex(node);

        var bodyNode = Body.AddToGraph(graph, astNodeDict);
        graph.AddEdge(new(node, bodyNode));

        astNodeDict[Id] = node;
        return node;
    }
}