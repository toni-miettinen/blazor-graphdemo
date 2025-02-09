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

        int red = (level - 1) * 28; // Gradually increase red from 0 to 255
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

    public class ForceDirectedLayout
    {
        private const double RepulsionStrength = 5000;
        private const double BaseAttractionStrength = 0.1; // Base before cost scaling
        private const double DampingFactor = 0.85;
        private const double MaxForce = 250;
        private const double MinDistance = 50;

        private Dictionary<string, (double x, double y)> positions = new();
        private Dictionary<string, (double dx, double dy)> velocities = new();

        public void Initialize(IEnumerable<string> vertices, double width, double height)
        {
            Random rand = new();
            foreach (var label in vertices)
            {
                positions[label] = (rand.NextDouble() * width, rand.NextDouble() * height);
                velocities[label] = (0, 0);
            }
        }

        public void ApplyForces(IEnumerable<string> vertices, IEnumerable<Edge> edges, int iterations)
        {
            for (int step = 0; step < iterations; step++)
            {
                Dictionary<string, (double fx, double fy)> forces = new();

                foreach (var label in vertices)
                    forces[label] = (0, 0);

                // Repulsion (same as before)
                foreach (var v1 in vertices)
                {
                    foreach (var v2 in vertices)
                    {
                        if (v1 == v2) continue;

                        var (x1, y1) = positions[v1];
                        var (x2, y2) = positions[v2];

                        double dx = x2 - x1;
                        double dy = y2 - y1;
                        double distanceSq = dx * dx + dy * dy + MinDistance;
                        double distance = Math.Sqrt(distanceSq);

                        double repulsion = RepulsionStrength / distanceSq;
                        double fx = -repulsion * (dx / distance);
                        double fy = -repulsion * (dy / distance);

                        forces[v1] = (forces[v1].fx + fx, forces[v1].fy + fy);
                    }
                }

                // Attraction (affected by cost)
                foreach (var edge in edges)
                {
                    var (x1, y1) = positions[edge.From];
                    var (x2, y2) = positions[edge.To];

                    double dx = x2 - x1;
                    double dy = y2 - y1;
                    double distance = Math.Sqrt(dx * dx + dy * dy + MinDistance);

                    // Attraction strength inversely proportional to cost
                    double attraction = (BaseAttractionStrength * distance * distance) / Math.Max(1, edge.Cost);
                    double fx = attraction * (dx / distance);
                    double fy = attraction * (dy / distance);

                    forces[edge.From] = (forces[edge.From].fx + fx, forces[edge.From].fy + fy);
                    forces[edge.To] = (forces[edge.To].fx - fx, forces[edge.To].fy - fy);
                }

                // Apply forces and update positions
                foreach (var label in vertices)
                {
                    var (fx, fy) = forces[label];
                    fx = Math.Clamp(fx, -MaxForce, MaxForce);
                    fy = Math.Clamp(fy, -MaxForce, MaxForce);

                    var (vx, vy) = velocities[label];
                    vx = (vx + fx) * DampingFactor;
                    vy = (vy + fy) * DampingFactor;
                    velocities[label] = (vx, vy);

                    var (x, y) = positions[label];
                    positions[label] = (x + vx, y + vy);
                }
            }
        }

        public Dictionary<string, (double x, double y)> GetPositions() => positions;
    }

    public static void ForceDirect(Domain.Graph graph, Diagram diagram)
    {
        //if (diagram.Container is null) throw new ArgumentNullException(nameof(diagram.Container));
        var width = 1024;
        var height = 1024;
        var layout = new ForceDirectedLayout();
        var verticeNames = graph.Vertices.Select(x => x.Label).ToArray();
        layout.Initialize(verticeNames, width, height);
        layout.ApplyForces(verticeNames, graph.Edges, 100);
        var positions = layout.GetPositions();
        foreach (var pos in positions)
        {
            var node = diagram.Nodes.First(x => x.Title == pos.Key);
            node.SetPosition(pos.Value.x, pos.Value.y);
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