using System.Collections;

namespace VerticalSlice.Features.Graph.Domain;

public record Vertex(string Label, int Cost = 1)
{
    public override string ToString() => $"Vertex {Label}({Cost})";
}

public record Edge(string From, string To, string Label, int Cost = 1)
{
    public string OtherEnd(string me) => From == me ? To : From;

    public bool EndsAre(string v1, string v2)
    {
        if (From == v1 && To == v2) return true;
        if (To == v1 && From == v2) return true;
        return false;
    }
    
    public bool ContainsEnd(string v) => From == v || To == v;
    
    public override string ToString() => $"Edge {Label}({Cost}): {From} -> {To}";
}

public class Graph
{
    private readonly Dictionary<string, IList<Edge>> _edgesByVertex = new();
    private readonly Dictionary<string, Vertex> _verticesByLabel = new();
    
    public List<Vertex> Vertices => _verticesByLabel.Values.ToList();
    public List<Edge> Edges => _edgesByVertex.SelectMany(x => x.Value).ToList();
    
    public IEnumerable<Edge> GetEdges(string vertex) =>
        _edgesByVertex[vertex];
    
    public Vertex GetVertex(string label) => _verticesByLabel.ContainsKey(label) ? _verticesByLabel[label] : throw new KeyNotFoundException();
   
    public void AddVertex(Vertex vertex)
    {
        _verticesByLabel[vertex.Label] = vertex;
        _edgesByVertex[vertex.Label] = new List<Edge>();
    }
    
    public void AddVertex(string label, int cost = 1) => AddVertex(new Vertex(label, cost));

    private void UpdateEdgeMap(Edge e)
    {
        _edgesByVertex[e.From].Add(e);
        _edgesByVertex[e.To].Add(e);
    }

    public Edge AddEdge(string from, string to, string label, int cost = 1) =>
        AddEdge(new Edge(from, to, label, cost));

    public Edge AddEdge(Edge edge)
    {
        UpdateEdgeMap(edge);
        return edge;
    }

    public void IsolateVertex(string label)
    {
        _edgesByVertex[label] = new List<Edge>();
    }
}

public class Path(Graph graph)
{
    private readonly IList<Vertex> _vertices = new List<Vertex>();
    public IEnumerable<Vertex> Vertices => _vertices.Reverse();
    public IEnumerable<Edge> Edges {
        get
        {
            var edges = new List<Edge>();
            var path = Vertices.ToList();
            if (!path.Any()) return Enumerable.Empty<Edge>();
            var prev = path.First();
            foreach (var vertex in path.Skip(1))
            {
                var ve = graph.GetEdges(vertex.Label);
                var e = ve
                    .Where(x => x.EndsAre(vertex.Label, prev.Label))
                    .MinBy(x => x.Cost) ?? throw new KeyNotFoundException($"could not find edge from {prev.Label} to {vertex.Label}");
                edges.Add(e);
                prev = vertex;
            }
            return edges;
        }
    }
    
    public void AddVertex(Vertex vertex) => _vertices.Add(vertex);
}