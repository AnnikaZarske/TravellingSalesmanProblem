using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Path
{
    public List<Vertex> vertices { 
        get {return _path; } 
    }        

    protected List<Vertex> _path;
    protected List<Vertex> _previousPath;

    System.Random random = new System.Random();

    public Path()
    {
        _path = new List<Vertex>();
        _previousPath = new List<Vertex>();
    }

    public void GenerateInitialPath(List<Vertex> verts)
    {
        List<Vertex> cities = new List<Vertex>(verts);

        Vertex startCity = cities[0];
        cities.Remove(startCity);
        _path.Add(startCity);

        while (cities.Count > 0) 
        {
            int index = random.Next(0, cities.Count - 1);
            Vertex currentCity = cities[index];

            cities.Remove(currentCity);
            _path.Add(currentCity);
        }
    }

    public double GetDistance()
    {
        double distance = 0;

        for (int index = 0; index < _path.Count; index++) 
        {
            Vertex start = _path[index];
            Vertex dest;
            if (index + 1 < _path.Count) {
                dest = _path[index + 1];
            } else {
                dest = _path[0];
            }
            distance += start.GetDistance(dest);
        }
        return distance;
    }

    public void SwapCities() 
    {
        int indexA = random.Next(0, _path.Count - 1);
        int indexB = random.Next(0, _path.Count - 1);

        _previousPath = new List<Vertex>(_path);

        Vertex cityA = _path[indexA];
        Vertex cityB = _path[indexB];

        _path[indexA] = cityB;
        _path[indexB] = cityA;
    }

    public void revertSwap() 
    {
        _path = _previousPath;
    }
}


public class SimulatedAnnealing
{
    public Graph graph { 
        get {return _graph; } 
    }        

    public double minDistance {
        get { return _minDistance; }
    }

    public List<Vertex> ShortestPath {
        get { return _shortestPath; }
    }

    protected Graph _graph;
    protected double _minDistance;
    protected List<Vertex> _shortestPath; // holds final result.     

    System.Random random = new System.Random();

    public SimulatedAnnealing(Graph graph)
    {
        _graph = graph;
        _shortestPath = new List<Vertex>();
        _minDistance = 0;
    }

    public void CalcShortestPath(double startingTemperature = 25.0, int numberOfIterations = 200, double coolingRate = 0.995)
    {
        double t = startingTemperature;
        Path path = new Path();

        path.GenerateInitialPath(_graph.Vertices);
        double bestDistance = path.GetDistance();

        Path bestSolution = path;
        Path currentSolution = bestSolution;
        
        for (int i = 0; i < numberOfIterations; i++) 
        {
            if (t > 0.1) {
                currentSolution.SwapCities();
                double currentDistance = currentSolution.GetDistance();
            
                if (currentDistance < bestDistance) {
                    bestDistance = currentDistance;
                    bestSolution = currentSolution;
                } 
                else if (Math.Exp((bestDistance - currentDistance) / t) < random.NextDouble()) {
                    currentSolution.revertSwap();
                    bestDistance = currentSolution.GetDistance(); 
                    bestSolution = currentSolution;
                }
                
                t *= coolingRate;
            } else {
                break;
            }
            // if (i % 10 == 0) {
            //     Console.WriteLine("SA #" + i + ": " + path.ToString());
            // }
        }
        
        // adding first city to last element in list to close the route
        _shortestPath = bestSolution.vertices;
        Vertex firstCity = _shortestPath[0];
        Vertex lastCity = _shortestPath[_shortestPath.Count - 1];
        _shortestPath.Add(firstCity);  

        _minDistance = bestDistance;
    }
}
