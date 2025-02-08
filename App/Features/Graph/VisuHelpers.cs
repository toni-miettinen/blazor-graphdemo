using Blazor.Diagrams.Core;
using Blazor.Diagrams.Core.Models;
using VerticalSlice.Features.Graph.Domain;

namespace VerticalSlice.Features.Graph;

public static class VisuHelpers
{
    public static string GetGradientHex(int level)
    {
        if (level < 1 || level > 10)
            throw new ArgumentOutOfRangeException(nameof(level), "Level must be between 1 and 10");

        int red = (level - 1) * 28;   // Gradually increase red from 0 to 255
        int green = 255 - ((level - 1) * 28); // Gradually decrease green from 255 to 0

        return $"#{red:X2}{green:X2}00"; // Keep blue at 00
    }

    public static void ArrangeNodesInCircle(List<NodeModel> nodes, double centerX, double centerY, double radius)
    {
        int n = nodes.Count;
        for (int i = 0; i < n; i++)
        {
            double angle = 2 * Math.PI * i / n; // Evenly spaced angles
            double x = centerX + radius * Math.Cos(angle);
            double y = centerY + radius * Math.Sin(angle);

            nodes[i].SetPosition(x, y);
        }
    }

    public static void ForceDirectLayout(Diagram diagram, List<(string from, string to)> edges)
    {
        var nodes = diagram.Nodes.ToList();
        double AttractionFactor = 0.1;
        int Iterations = 100;
        double RepulsionFactor = 1000;
        Random _rand = new Random();
        Dictionary<string, (double x, double y)> positions = new Dictionary<string, (double x, double y)>();

        // Initialize positions randomly
        foreach (var node in nodes)
        {
            positions[node.Title!] = (_rand.NextDouble() * 500, _rand.NextDouble() * 500);
        }

        for (int step = 0; step < Iterations; step++)
        {
            Dictionary<string, (double x, double y)> forces = new Dictionary<string, (double x, double y)>();

            // Apply repulsion (nodes push each other away)
            foreach (var a in nodes)
            {
                forces[a.Title!] = (0, 0);
                foreach (var b in nodes)
                {
                    if (a.Id == b.Id) continue;

                    (double ax, double ay) = positions[a.Title!];
                    (double bx, double by) = positions[b.Title!];
                    double dx = ax - bx;
                    double dy = ay - by;
                    double distance = Math.Max(Math.Sqrt(dx * dx + dy * dy), 1);

                    double force = RepulsionFactor / (distance * distance);
                    forces[a.Title!] = (forces[a.Title!].x + force * dx, forces[a.Title!].y + force * dy);
                }
            }

            // Apply attraction (edges pull nodes closer)
            foreach (var edge in edges)
            {
                (double ax, double ay) = positions[edge.from];
                (double bx, double by) = positions[edge.to];
                double dx = bx - ax;
                double dy = by - ay;
                double distance = Math.Max(Math.Sqrt(dx * dx + dy * dy), 1);

                double force = AttractionFactor * (distance * distance);
                forces[edge.from] = (forces[edge.from].x + force * dx, forces[edge.from].y + force * dy);
                forces[edge.to] = (forces[edge.to].x - force * dx, forces[edge.to].y - force * dy);
            }

            // Update positions
            /*
            foreach (var node in nodes)
            {
                positions[node.Title!] = (
                    positions[node.Title!].x + forces[node.Title!].x * 0.1,
                    positions[node.Title!].y + forces[node.Title!].y * 0.1
                );
            }
            */
        }

        // Apply final positions
        foreach (var node in nodes)
        {
            var (x, y) = positions[node.Title];
            node.SetPosition(x, y);
        }
    }

    public static void CalculatePath(Vertex from, Vertex to, Domain.Graph graph, Diagram diagram)
    {
        var path = new DijkstraAlgorithm(graph, from.Label).CalculateShortestPath(to.Label);
        foreach (var l in diagram.Links)
        {
            if (l is LinkModel lm)
            {
                lm.Width = 2;
                lm.Refresh();
            }
        } 
        foreach (var e in path.Edges)
        {
            var link = diagram.Links
                .First(x => x.Labels.Exists(l => l.Content == e.Label));
            if (link is LinkModel lm)
            {
                lm.Width = 10.0;
            }
            link.Refresh();
        }
    }
}