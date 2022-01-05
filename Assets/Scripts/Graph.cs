using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Vertex 
{
    public int index { get; set; }
    public Vector2 position { get; set; }
    public List<Edge> Edges { get; set; }        

    public Vertex(int index)
    {
        this.index = index;
        this.Edges = new List<Edge>();
        this.position = Vector2.zero;
    }

    public int getPosX()
    {
        return (int)position.x;
    }

    public int getPosY()
    {
        return (int)position.y;
    }

    public override string ToString() {
        string str = "";

        str += "[" + index.ToString("00") + "] ";
        str += "x:" + position.x.ToString() + " | y:" + position.y.ToString(); 
        str += System.Environment.NewLine;

        foreach (var edge in Edges) {
            str += "     " + edge.ToString();
        }

        return str;
    }
}

public class Edge
{
    public double distance { get; set; }
    //public double pheromoneValue { get; set; }
    public Vertex vertexA { get; set; }
    public Vertex vertexB { get; set; }

    public Edge(Vertex vertexA, Vertex vertexB)
    {
        this.vertexA = vertexA;
        this.vertexB = vertexB;
        CalculateDistance();
    }

    public void CalculateDistance()
    {
        int distance_x = Mathf.Abs(vertexB.getPosX() - vertexA.getPosX());
        int distance_y = Mathf.Abs(vertexB.getPosY() - vertexA.getPosY());
        this.distance = Mathf.Sqrt( Mathf.Pow( distance_x, 2) + Mathf.Pow( distance_y, 2));
    }

    public override string ToString() {
        string str = "";

        str += "-> [" + vertexB.index.ToString("00") + "] ";
        str += " d = " + distance.ToString("0.00"); 
        str += System.Environment.NewLine;
        return str;
    }
}

public class Graph
{
    public int vertexCount {
        get { return _vertexCount; }
    }

    public List<Vertex> Vertices { get; set; }
    public List<Edge> Edges {get; set;}
    
    private System.Random rand = new System.Random();

    protected int _vertexCount;
    protected Vector2 _minPos;
    protected Vector2 _maxPos;

    public Graph(int vertexCount, Vector2 minPos, Vector2 maxPos)
    {
        _vertexCount = vertexCount;
        _minPos = minPos;
        _maxPos = maxPos;

        this.Vertices = new List<Vertex>();
        this.Edges = new List<Edge>();

        CreateGraph();   
    }

    private Vector2 GetRandomPosition()
    {
        //System.Random rand = new System.Random();

        // ramdomized position of the vertex
        int x = rand.Next((int)_minPos.x, (int)_maxPos.x);
        int y = rand.Next((int)_minPos.y, (int)_maxPos.y);
        return new Vector2(x, y);
    }

    public void RandomizeVertices()
    {
        // Ramdomize Positions
        for (int i = 0; i < _vertexCount; i++)
            Vertices[i].position = GetRandomPosition();

        // Recalculate Distances
        foreach (var edge in Edges)
            edge.CalculateDistance();
    }

    private void CreateGraph()
    {
        // Generate _vertexCount vertices
        for (int i = 0; i < _vertexCount; i++)
        {
            Vertex vertex = new Vertex(i);
            vertex.position = GetRandomPosition();
            this.Vertices.Add(vertex);
        }
        
        // Connect all vertices and calculate distance
        for (int i = 0; i < _vertexCount; i++)
        {
            for (int j = i + 1; j < _vertexCount; j++)
            {
                Edge edgeTo = new Edge(Vertices[i], Vertices[j]);
                Vertices[i].Edges.Add(edgeTo);
                
                Edge edgeFrom = new Edge(Vertices[j], Vertices[i]);
                Vertices[j].Edges.Add(edgeFrom);
                
                Edges.Add(edgeTo);
                Edges.Add(edgeFrom);
            }
        }
    }

    public override string ToString() {
        string str = "";
        foreach (var vertex in Vertices) {
            str += vertex.ToString() + System.Environment.NewLine;
        }
        return str;
    }
}