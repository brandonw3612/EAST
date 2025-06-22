using QuikGraph;

namespace EAST.CPP.Graph;

public interface IGraphNode
{
    GraphNode AddToGraph(AdjacencyGraph<GraphNode, GraphEdge> graph, Dictionary<string, GraphNode> astNodeDict);
}