using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class NearestNeighbour
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

        protected Vertex _startCity;
        protected Vertex _currentCity;
        protected Vertex _closestCity;
        protected Vertex _possibleCity;

        protected List<Vertex> _cities;

        public NearestNeighbour(Graph graph)
        {
            _graph = graph;
            _cities = new List<Vertex>(_graph.Vertices); // holds the working list of cities

            _shortestPath = new List<Vertex>();
            _minDistance = 0;
        }

        public void CalcShortestPath()
        {    
            // set currentCity to first city and remove it from cities list
            _startCity = _cities[0];
            _currentCity = _cities[0];
            _cities.Remove(_currentCity);

            // add first city to result list
            _shortestPath.Add(_currentCity);

            // set closestCity to new first city list item
            _closestCity = _cities[0]; 

            double distance;

            // outer loop for all cities
            while (_cities.Count > 0) {
                distance = Double.MaxValue; // reset distance to max value

                // inner loop checks distance between currentCity and possibleCity
                for (int count = 0; count < _cities.Count; count++) {
                    _possibleCity = _cities[count];
                
                    double possibleDistance = _currentCity.GetDistance(_possibleCity);
                    if (possibleDistance < distance) {
                        distance = possibleDistance;
                        _closestCity = _possibleCity;
                    }
                }

                // once inner loop finds closest city set currentCity to closestCity
                _minDistance += distance;
                _currentCity = _closestCity;
                _cities.Remove(_closestCity);
                _shortestPath.Add(_currentCity);
            }

            _minDistance += _currentCity.GetDistance(_startCity);
            _shortestPath.Add(_startCity);
        }
}
