using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NearestNeighbour
{
    public Graph graph { get; }

    public double minDistance
    {
        get { return _minDistance; }
    }

    public List<Vertex> ShortestPath
    {
        get { return _shortestPath; }
    }

    protected double _minDistance;
    protected List<Vertex> _shortestPath;

    protected Vertex _startPoint;
    protected Stack<Vertex> _pointStack;
    protected Stack<Vertex> _visitedPoints;

    public NearestNeighbour(Graph graph)
    {
        this.graph = graph;
        _visitedPoints = new Stack<Vertex>();
        _pointStack = new Stack<Vertex>();
        _shortestPath = new List<Vertex>();
        _minDistance = Double.MaxValue;
        ;
    }

    int counter = 0;
    double distance = 0;

    public void CalcShortestPath()
    {
        foreach (Vertex v in graph.Vertices)
        {
            _startPoint = v;
            NearestNeighbourRecurring(_startPoint);
            if (distance < _minDistance)
            {
                _minDistance = distance;
                _shortestPath.Clear();
                _shortestPath.AddRange(_pointStack);
            }
        }
    }

    private bool NearestNeighbourRecurring(Vertex vertex)
    {
        counter++;
        _visitedPoints.Push(vertex);
        _pointStack.Push(vertex);

        Edge nextEdge = null;

        foreach (Edge e in vertex.Edges)
        {
            if (e.vertexB == _startPoint)
            {
                if (counter == graph.Vertices.Count)
                {
                    _pointStack.Push(e.vertexB);
                    distance += e.distance;
                    return true;
                }
            }

            if (!_visitedPoints.Contains(e.vertexB))
            {
                if (nextEdge == null || nextEdge.distance > e.distance)
                {
                    nextEdge = e;
                }
            }
        }

        if (nextEdge != null)
        {
            if (NearestNeighbourRecurring(nextEdge.vertexB))
            {
                distance += nextEdge.distance;
                return true;
            }
        }

        _visitedPoints.Pop();
        counter--;
        return false;
    }
}
