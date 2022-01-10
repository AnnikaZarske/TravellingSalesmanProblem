using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoOpt
{
    public Graph graph { 
        get {return _graph; } 
    }        

    public double minDistance {
        get { return _minDistance; }
    }

    public List<Vertex> shortestPath {
        get { return _shortestPath; }
    }

    protected Graph _graph;
    protected double _minDistance;
    protected List<Vertex> _shortestPath; // holds final result.     

    protected List<Vertex> _cities;

    private List<Vertex> GenNNPath(List<Vertex> verts)
    {    
        List<Vertex> cities = new List<Vertex>(verts);

        Vertex startCity = cities[0];
        Vertex currentCity = cities[0];
        cities.Remove(currentCity);

        List<Vertex> path = new List<Vertex>();
        path.Add(currentCity);

        Vertex closestCity = cities[0]; 

        double distance;

        while (cities.Count > 0) 
        {
            distance = Double.MaxValue; 

            for (int count = 0; count < cities.Count; count++) 
            {
                Vertex possibleCity = cities[count];
            
                double possibleDistance = currentCity.GetDistance(possibleCity);
                if (possibleDistance < distance) 
                {
                    distance = possibleDistance;
                    closestCity = possibleCity;
                }
            }

            currentCity = closestCity;
            cities.Remove(closestCity);
            path.Add(currentCity);
        }

        // path.Add(startCity);

        return path;
    }

    private static List<Vertex> swap(List<Vertex> cities, int i, int j) 
    {
        // conducts a 2 opt swap by inverting the order of the points between i and j
        List<Vertex> newTour = new List<Vertex>();

        // take array up to first point i and add to newTour
        int size = cities.Count;
        for (int c = 0; c <= i - 1; c++) {
            newTour.Add(cities[c]);
        }

        // invert order between 2 passed points i and j and add to newTour
        int dec = 0;
        for (int c = i; c <= j; c++) {
            newTour.Add(cities[j - dec]);
            dec++;
        }

        // append array from point j to end to newTour
        for (int c = j + 1; c < size; c++) {
            newTour.Add(cities[c]);
        }

        return newTour;
    }

    public static double routeLength(List<Vertex> cities) 
    {
        // Calculate the length of a TSP route held in an List of Verticis
        double result = 0; 
        Vertex prev = cities[cities.Count - 1];

        // Set the previous city to the last city in the List as we need to measure the length of the entire loop
        foreach (Vertex city in cities) 
        {
            //Go through each city in turn
            result += city.GetDistance(prev);

            //get distance from the previous city
            prev = city;

            //current city will be the previous city next time
        }
        return result;
    }

    public TwoOpt(Graph graph)
    {
        _graph = graph;
        _cities = new List<Vertex>();
        _shortestPath = new List<Vertex>();
        _minDistance = 0;
    }

    public void CalcShortestPath()
    { 
        _cities = GenNNPath(_graph.Vertices);
        double bestDist = routeLength(_cities);

        List<Vertex> newTour;
        double newDist;
        int swaps = 1;
        int improve = 0;
        int iterations = 0;
        long comparisons = 0;

        while (swaps != 0) // loop until no improvements are made.
        { 
            swaps = 0;

            // initialise inner/outer loops avoiding adjacent calculations and making use of 
            // problem symmetry to half total comparisons.
            for (int i = 1; i < _cities.Count - 2; i++) 
            {
                for (int j = i + 1; j < _cities.Count - 1; j++) 
                {
                    comparisons++;
                    
                    //check distance of line A,B + line C,D against A,C + B,D if there is improvement, call swap method.
                    if ((_cities[i].GetDistance(_cities[i - 1]) + _cities[j + 1].GetDistance(_cities[j])) >=
                        (_cities[i].GetDistance(_cities[j + 1]) + _cities[i - 1].GetDistance(_cities[j]))) 
                    {
                        newTour = swap(_cities, i, j); // pass list and 2 points to be swapped.

                        newDist = routeLength(newTour);

                        // if the swap results in an improved distance, increment counters and update distance/tour
                        if (newDist < bestDist) 
                        { 
                            _cities = newTour;
                            bestDist = newDist;
                            swaps++;
                            improve++;
                        }
                    }
                }
            }
            iterations++;
        }

        _shortestPath = _cities;
        Vertex firstCity = _shortestPath[0];
        _shortestPath.Add(firstCity);  

        _minDistance = bestDist;
    }
}
