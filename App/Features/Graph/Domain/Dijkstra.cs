namespace VerticalSlice.Features.Graph.Domain;

public class DijkstraAlgorithm
{
    private readonly Graph _graph;
    private HashSet<string> Unvisited;
    private Dictionary<string, decimal> DistanceMap;
    private Dictionary<string, string> Predecessors;
    private readonly Vertex _start;
    private Vertex Target;
    
    public DijkstraAlgorithm(Graph graph, string start)
    {
        _start = graph.GetVertex(start) ?? throw new KeyNotFoundException($"Vertex {start} was not found");
        _graph = graph;
    }

    // 3
    private string? CurrentNode => Unvisited.Any()
        ? DistanceMap
            .Where(x => Unvisited.Contains(x.Key))
            .MinBy(x => x.Value).Key
        : null;
   
    private bool EndReached() => !Unvisited.Any() || Unvisited.All(v => DistanceMap[v] == decimal.MaxValue);

    private Edge[] GetUnvisitedEdges(string from)
    {
        
        var edges = _graph.GetEdges(from);
        var unvisitedEdges = edges.Where(e => Unvisited.Contains(e.OtherEnd(from)));
        return unvisitedEdges.ToArray();
    }
    
    private void UpdateNeighborDistances(string current)
    {
        var unvisitedNeighbors = GetUnvisitedEdges(current);
        foreach (var edge in unvisitedNeighbors)
        {
            var a = current;
            var b = edge.OtherEnd(a);
            var distanceA = DistanceMap[a];
            var distanceToB = (decimal)edge.Cost;
            var newDistance = distanceA + distanceToB;
            var distanceB = DistanceMap[b];
            if (distanceB > newDistance) DistanceMap[b] = newDistance;
            Predecessors[b] = current;
        }
    }
    
    public Path CalculateShortestPath(string to)
    {
        Target = _graph.GetVertex(to) ?? throw new KeyNotFoundException();
        // 1
        Unvisited = new(_graph.Vertices.Select(x => x.Label));
        // 2
        DistanceMap = new(
            _graph.Vertices.Select(x => new KeyValuePair<string, decimal>(x.Label, decimal.MaxValue))
        );
        DistanceMap[_start.Label] = 0;
        Predecessors = new();
        
        // https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
        var iterations = 0;
        while (!EndReached())
        {
            var node = CurrentNode;
            if (node is null || node == Target.Label) break;
            // 4
            UpdateNeighborDistances(node);
            // 5
            Unvisited.Remove(node);
            iterations++;
        }

        string? step = Target.Label;
        var path = new Path(_graph);
        while (!string.IsNullOrEmpty(step))
        {
            path.AddVertex(_graph.GetVertex(step) ?? throw new KeyNotFoundException(step));
            Predecessors.TryGetValue(step, out step);
        }
        
        return path;
    }
}