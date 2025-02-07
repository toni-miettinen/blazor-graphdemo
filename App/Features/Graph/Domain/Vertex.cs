using System.Collections;

namespace VerticalSlice.Features.Graph.Domain;

public record Vertex(string Label, int Cost = 1)
{
    public override string ToString() => $"Vertex {Label}({Cost})";
}

public record Edge(string From, string To, string Label, int Cost = 1)
{
    public string OtherEnd(string me) => From == me ? To : From;
    
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

    public Edge AddEdge(string from, string to, string label, int cost = 1)
    {
        var e = new Edge(from, to, label, cost);
        UpdateEdgeMap(e);
        return e;
    }

    public void IsolateVertex(string label)
    {
        _edgesByVertex[label] = new List<Edge>();
    }
}

public class Path
{
    private readonly List<Edge> _edges = new();
    private readonly List<Vertex> _vertices = new();
    public IEnumerable<Edge> Edges => _edges;
    public int Length => _edges.Sum(x => x.Cost);
    public int EdgeCount => _edges.Count;
    public IEnumerable<Vertex> Vertices => _vertices;

    public void Reverse() => _vertices.Reverse();
   
    public void AddEdge(Edge edge) => _edges.Add(edge);
    public void AddVertex(Vertex vertex) => _vertices.Add(vertex);
}