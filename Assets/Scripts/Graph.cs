using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public int index { get; set; }
    public Vector2 position { get; set; }
    public List<Edge> edges { get; set; }

    public Vertex(int index = -1)
    {
        this.index = index;
        this.edges = new List<Edge>();
        this.position = Vector2.zero;
    }

    public int getPosX()
    {
        return (int) position.x;
    }
    
    public int getPosY()
    {
        return (int) position.y;
    }
    
    public override string ToString() {
        string str = "";

        str += "[" + index.ToString("00") + "] ";
        str += "x:" + position.x.ToString() + " | y:" + position.y.ToString(); 
        str += System.Environment.NewLine;

        foreach (var edge in edges) {
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

    public Edge()
    {
        this.vertexA = new Vertex();
        this.vertexB = new Vertex();
        this.distance = 0;
    }

    public override string ToString() {
        string str = "";

        str += "-> [" + vertexB.index.ToString("00") + "] ";
        str += " d = " + distance.ToString("0.00"); 
        str += System.Environment.NewLine;
        return str;
    }
}

public class Graph : MonoBehaviour
{
    public int vertexCount
    {
        get { return _vertexCount; }
    }
    
    public List<Vertex> Vertices { get; set; }
    public List<Edge> Edges { get; set; }

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

    private void CreateGraph()
    {
        System.Random rand = new System.Random();

        for (int i = 0; i < _vertexCount; i++)
        {
            Vertex vertex = new Vertex(i);

            int x = rand.Next((int) _minPos.x, (int) _maxPos.x);
            int y = rand.Next((int) _minPos.y, (int) _maxPos.y);
            vertex.position = new Vector2(x, y);

            this.Vertices.Add(vertex);
        }

        for (int i = 0; i < _vertexCount; i++)
        {
            for (int j = 0; j < _vertexCount; j++)
            {
                Edge edgeTo = new Edge();
                edgeTo.vertexA = Vertices[i];
                edgeTo.vertexB = Vertices[j];
                int distance_x = Mathf.Abs(edgeTo.vertexB.getPosX() - edgeTo.vertexA.getPosX());
                int distance_y = Mathf.Abs(edgeTo.vertexB.getPosY() - edgeTo.vertexA.getPosY());
                edgeTo.distance = Mathf.Sqrt(Mathf.Pow(distance_x, 2) + Mathf.Pow(distance_y, 2));

                Vertices[i].edges.Add(edgeTo);

                Edge edgeFrom = new Edge();
                edgeFrom.vertexB = Vertices[i];
                edgeFrom.vertexA = Vertices[j];
                edgeFrom.distance = edgeTo.distance;

                Vertices[j].edges.Add(edgeFrom);

                Edges.Add(edgeTo);
                Edges.Add(edgeFrom);
            }
        }
    }

    public override string ToString() 
        {
        string str = "";
        foreach (var vertex in Vertices) {
            str += vertex.ToString() + System.Environment.NewLine;
        }
        return str;
        }
}
