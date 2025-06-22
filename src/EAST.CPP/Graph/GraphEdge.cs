using QuikGraph;
using QuikGraph.Graphviz.Dot;

namespace EAST.CPP.Graph;

public class GraphEdge : IEdge<GraphNode>
{
    public GraphEdge(GraphNode source, GraphNode target)
    {
        Source = source;
        Target = target;
    }

    public GraphNode Source { get; }
    public GraphNode Target { get; }
    
    public string Label { get; set; } = string.Empty;
    
    public GraphvizEdgeStyle Style { get; set; } = GraphvizEdgeStyle.Solid;
}