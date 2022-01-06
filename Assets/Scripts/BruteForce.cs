using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BruteForce
    {
        public Graph graph { get; }        
        
        public double minDistance {
            get { return _minDistance; }
        }

        protected double _minDistance;

        protected Vertex _startPoint;
        protected List<Vertex> _shortestPath;        
        protected Stack<Vertex> _pointStack;
        protected Stack<Vertex> _visitedPoints;
        
        public BruteForce(Graph graph)
        {
            this.graph = graph;
            _pointStack = new Stack<Vertex>();
            _visitedPoints = new Stack<Vertex>();
            _shortestPath = new List<Vertex>();
            _minDistance = Double.MaxValue;
        }

        public List<Vertex> ShortestPath()
        {
            _startPoint = graph.Vertices[0];
            SearchShortestPath(_startPoint);
            return _shortestPath;
        }

        int counter = 0;
        double distance = 0;

        private void SearchShortestPath(Vertex vertex)
        {
            counter++;
            _visitedPoints.Push(vertex);            
            _pointStack.Push(vertex);

            foreach (Edge edge in vertex.Edges)
            {
                if (edge.vertexB == _startPoint)
                {
                    if (counter == graph.Vertices.Count)
                    {
                        _pointStack.Push(edge.vertexB);
                        distance += edge.distance;

                        if (distance < minDistance)
                        {
                            _shortestPath.Clear();
                            _shortestPath.AddRange(_pointStack);
                            _minDistance = distance;
                        }

                        distance -= edge.distance;
                        _pointStack.Pop();
                    }
                }

                if (!_visitedPoints.Contains(edge.vertexB))
                {
                    distance += edge.distance;
                    SearchShortestPath(edge.vertexB);
                    distance -= edge.distance;
                }
            }

            _pointStack.Pop();
            _visitedPoints.Pop();
            counter--;
        }
    }
