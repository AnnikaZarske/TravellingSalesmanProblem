using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class SingleAnt
{
    public Vertex startVertex { get; set; }
    public Stack<Vertex> visitedVertices { get; set; }
    public List<Edge> visitedEdges { get; set; }
    public double travelledDistance { get; set; }

    public SingleAnt()
    {
        visitedVertices = new Stack<Vertex>();
        visitedEdges = new List<Edge>();
        travelledDistance = 0.0f;
    }

}

public class AntColony
{
   public Graph graph { get; }        
        
        public double minDistance {
            get { return _minDistance; }
        }

        public List<Vertex> ShortestPath {
            get { return _shortestPath; }
        }

        public double ro { get; set; }
        public int alpha { get; set; }
        public int beta { get; set; }

        protected double _minDistance;
        protected List<Vertex> _shortestPath;        

        protected List<SingleAnt> _ants; 

        System.Random random;
        int counter;

        public AntColony(Graph graph, double ro, int alpha, int beta)
        {
            this.graph = graph;
            this.ro = ro;
            this.alpha = alpha;
            this.beta = beta;
            random = new System.Random();
            _ants = new List<SingleAnt>();
            _shortestPath = new List<Vertex>();
            _minDistance = 0;
        }

        private void AntGenerator(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                _ants.Add(
                    new SingleAnt()
                    {
                        //startVertex = graph.vertices[random.Next(0, graph.vertices.Count - 1)]
                        startVertex = graph.Vertices[0]
                    }
                );
            }
        }

        public void CalcShortestPath(int iterations, int antAmount)
        {
            AntGenerator(antAmount);
            
            for (int i = 0; i < iterations; i++)
            {
                AntReset();

                foreach (SingleAnt ant in _ants)
                {
                    AntPath(ant);
                    Pheromone(ant);

                    if (_minDistance == 0 || ant.travelledDistance < _minDistance)
                    {
                        _minDistance = ant.travelledDistance;
                        _shortestPath.Clear();
                        _shortestPath.AddRange(ant.visitedVertices);
                    }
                }
            }
        }

        private void AntReset()
        {
            foreach (SingleAnt ant in _ants)
            {
                ant.travelledDistance = 0;
                ant.visitedEdges.Clear();
                ant.visitedVertices.Clear();
            }
        }

        private void AntPath(SingleAnt ant)
        {
            counter = 0;
            AntPathRecurring(ant, ant.startVertex);
        }

        private void AntPathRecurring(SingleAnt ant, Vertex vertex)
        {
            counter++;
            ant.visitedVertices.Push(vertex);           
            Edge nextEdge = null;

            if (counter == graph.Vertices.Count)
            {
                foreach (Edge edge in vertex.Edges)
                {
                    if (edge.vertexB == ant.startVertex)
                    {
                        ant.travelledDistance += edge.distance;
                        AntPathRecurring(ant, edge.vertexB);
                    }
                }
            }
            else
            {
                nextEdge = NextEdge(ant);
                if (nextEdge != null)
                {
                    ant.visitedEdges.Add(nextEdge);
                    ant.travelledDistance += nextEdge.distance;
                    AntPathRecurring(ant, nextEdge.vertexB);
                }
            }
        }

        private Edge NextEdge(SingleAnt ant)
        {
            double c = 0;
            double r = random.NextDouble();
            
            Vertex vertex = ant.visitedVertices.Peek();

            foreach (Edge edge in vertex.Edges)
            {
                if (!ant.visitedVertices.Contains(edge.vertexB))
                {
                    c += Probability(ant, edge);
                    if (c >= r)
                    {
                        return edge;
                    }
                }
            }
            return null;
        }


        private void Pheromone(SingleAnt ant)
        {
            foreach (Vertex point in graph.Vertices)
            {
                foreach (Edge edge in point.Edges)
                {
                    if (ant.visitedEdges.Contains(edge))
                    {
                        edge.pheromone = (1 - ro) * edge.pheromone + (1 / (double)ant.travelledDistance);
                    }
                    else
                    {
                        edge.pheromone = (1 - ro) * edge.pheromone;
                    }
                }
            }
        }

        private double Probability(SingleAnt ant, Edge edge)
        {
            double nominator = 0;
            double denominator = 0;
            double probability = 0;

            Vertex vertex = ant.visitedVertices.Peek();

            nominator = Math.Pow(edge.pheromone, alpha) * Math.Pow(1 / (double)edge.distance, beta);

            foreach (Edge e in vertex.Edges)
            {
                if (!ant.visitedVertices.Contains(e.vertexB))
                {
                    denominator += Math.Pow(e.pheromone, alpha) * Math.Pow(1 / (double)e.distance, beta);
                }
            }

            if (denominator != 0)
                probability = nominator / denominator;

            return probability;
        } 
}
