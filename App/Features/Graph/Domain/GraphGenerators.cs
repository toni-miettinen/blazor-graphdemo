namespace VerticalSlice.Features.Graph.Domain;

public static class GraphGenerators
{
    public static Graph SquareMesh(int width, int height)
    {
        var graph = new Graph();
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var curr = $"{x}.{y}";
                var above = $"{x}.{y-1}";
                var previous = $"{x-1}.{y}";
                var diagonal = $"{x-1}.{y-1}";
                graph.AddVertex(curr, 1);
                if (y > 0)
                {
                    graph.AddEdge(curr, above, $"{curr}-{above}", 1);
                    if (x > 0) graph.AddEdge(curr, diagonal, $"{curr}-{previous}", 1);
                }
            }
        }

        return graph;
    }

    public static Graph Random(int numVertices)
    {
        var graph = new Graph();
        var vertices = Enumerable
            .Range(0, numVertices)
            .Select(n => new Vertex($"v{n}")).ToArray();
        foreach(var vertex in vertices) graph.AddVertex(vertex);
        var rnd = new Random(DateTime.Now.Millisecond);
        var edges = Enumerable.Range(0, numVertices).Select(i =>
        {
            var from = $"v{rnd.Next(numVertices)}";
            var to = $"v{rnd.Next(numVertices)}";
            return new Edge(from, to, $"{from}-{to}", rnd.Next(20));
        });
        foreach (var edge in edges) graph.AddEdge(edge);
        
        return graph;
    }
    
    public static Graph ChatGPT(int vertexCount, int extraEdges)
    {
        var cc = new CharGenerator();
        var _rand = new Random(DateTime.Now.Millisecond);
        var _graph = new Graph();
        List<string> vertexLabels = new List<string>();

        // Step 1: Add vertices
        for (int i = 0; i < vertexCount; i++)
        {
            string label = $"v{i}";
            _graph.AddVertex(label, _rand.Next(1, 10));  // Random cost between 1-10
            vertexLabels.Add(label);
        }

        // Step 2: Create a spanning tree (ensuring connectivity)
        HashSet<string> connected = new HashSet<string>();
        connected.Add(vertexLabels[0]);  // Start with first vertex

        for (int i = 1; i < vertexLabels.Count; i++)
        {
            string from = vertexLabels[_rand.Next(0, i)];  // Pick a connected vertex
            string to = vertexLabels[i];  // Connect next available vertex
            
            _graph.AddEdge(from, to, cc.Next(), _rand.Next(1, 10));  
            connected.Add(to);
        }

        // Step 3: Add extra random edges
        for (int i = 0; i < extraEdges; i++)
        {
            string from = vertexLabels[_rand.Next(vertexCount)];
            string to = vertexLabels[_rand.Next(vertexCount)];

            if (from != to) // Avoid self-loops
            {
                _graph.AddEdge(from, to, cc.Next(), _rand.Next(1, 10));
            }
        }
        return _graph;
    }
}

public class CharGenerator
{
    private IList<Char> _chars;
    private int _index = 0;
    private int _rollOverIndex = 0;
    
    public CharGenerator()
    {
        _chars = Enumerable.Range('a', 26)
            .Concat(Enumerable.Range('A', 26))
            .Select(c => (char)c).ToList();
    }

    public string Next()
    {
        if (_index == _chars.Count - 1)
        {
            _index = 0;
            _rollOverIndex++;
        }

        if (_rollOverIndex > 0) return $"{_chars[_rollOverIndex]}{_chars[_index++]}";
        return $"{_chars[_index++]}";
    }
}
