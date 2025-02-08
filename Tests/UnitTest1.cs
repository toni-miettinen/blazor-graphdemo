using Shouldly;
using VerticalSlice.Features.Graph.Domain;
using Path = VerticalSlice.Features.Graph.Domain.Path;

namespace Tests;

public class UnitTest
{
    private Graph SimpleGraph()
    {
           
        /*  0              1
         *   v1----a1---v2
         *              |
         *     8 .v3.   b2
         *      X    \  |
         *    d4     5e |
         * 6 v4---c3---v5 3
         *   |
         *   f3
         *   |
         *   v6
         */         
        var graph = new Graph();
        graph.AddVertex("v1");
        graph.AddVertex("v2");
        graph.AddVertex("v3"); 
        graph.AddVertex("v4");
        graph.AddVertex("v5");
        graph.AddVertex("v6");

        graph.AddEdge("v1", "v2", "a", 1);
        graph.AddEdge("v2", "v5", "b", 2);
        graph.AddEdge("v5", "v4", "c", 3);
        //graph.AddEdge("v4", "v3", "d", 4);
        graph.AddEdge("v3", "v5", "e", 5);
        graph.AddEdge("v4", "v6", "f", 3);

        return graph;
    }

    
    /* square mesh
     * 0.0 -- 1.0 -- 2.0 -- 3.0 -- ...
     *  \      \      \      \
     *    \      \      \      \
     *      \      \      \      \
     * 1.0 -- 1.1 -- 2.1 -- 3.1 --
     *  \      \      \      \ 
     *  ...
     * 
     */
    private Graph BigGraph(int width, int height)
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

    private void ApplyFilter(Graph graph, string[] filter)
    {
        foreach (var v in filter)
        {
            if (graph.Vertices.Exists(x => x.Label == v))
                graph.IsolateVertex(v);
        }
    }

    private string[] BoxFilter(int x1, int y1, int x2, int y2)
    {
        List<string> filter = new();
        for (var x = x1; x <= x2; x++)
        {
            for (var y = y1; y <= y2; y++)
            {
                filter.Add($"{x}.{y}");
            }
        }

        return filter.ToArray();
    }
    
    private void OutputPath(Graph graph, Path path, int width, int height)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var v = graph.GetVertex($"{x}.{y}");
                var edgeCount = graph.GetEdges(v.Label).Count();
                if (edgeCount > 0)
                {
                    if (path.Vertices.Contains(v)) Console.Write($"*");
                    else Console.Write(edgeCount);
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();
        }
    }
    
    [Fact]
    public void DijkstraAlgorithm_Vertices_Valid() 
    {
        var graph = BigGraph(50, 50);
        ApplyFilter(graph, BoxFilter(10,10,25,25));
        ApplyFilter(graph, BoxFilter(30,14,55,45));
        var algo = new DijkstraAlgorithm(graph, "4.4");
        var path = algo.CalculateShortestPath("45.48");
        OutputPath(graph, path, 50, 50);
        path.Vertices.Last().Label.ShouldBe("45.48");
    }

    [Fact]
    public void DijkstraAlgorigthm_Edges_Valid()
    {
        var graph = GraphGenerators.ChatGPT(1024, 25);
        var first = graph.Vertices.First();
        var last = graph.Vertices.Last();
        var algo = new DijkstraAlgorithm(graph, first.Label);
        var path = algo.CalculateShortestPath(last.Label);
        path.ShouldNotBeNull();
        var edges = path.Edges.ToList();
        edges.ShouldNotBeEmpty();
        edges.First().ContainsEnd(first.Label).ShouldBeTrue();
        edges.Last().ContainsEnd(last.Label).ShouldBeTrue();
        path.Vertices.Select(x => x.Label).ShouldBeUnique();
        edges.Select(x => x.Label).ShouldBeUnique();
    }
}