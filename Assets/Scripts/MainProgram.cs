using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainProgram : MonoBehaviour
{
    public Vector2 minPos;
    public Vector2 maxPos;
    public int cityAmount;
    public GameObject cityDotpf;

    private List<GameObject> cityDots;
    private Graph graph;
    void Start()
    {
        cityDots = new List<GameObject>();
        graph = new Graph(cityAmount, minPos, maxPos);
    }

    private void GraphInDebug()
    {
        Debug.Log("*********************************");
        Debug.Log("***    All Points in Graph    ***");
        Debug.Log("*********************************");
        Debug.Log("");

        foreach (var point in graph.Vertices) {
            Debug.Log("[" + point.index.ToString("00") + "] ");
            Debug.Log("x:" + point.position.x.ToString() + " | y:" + point.position.y.ToString());
        }
    }

    private void ClearCities()
    {
        foreach (GameObject citydot in cityDots)
        {
            Destroy(citydot);
        }
        cityDots.Clear();
    }
    public void OnStartClick()
    {
        ClearCities();
        Destroy(graph);
        
        graph = new Graph(cityAmount, minPos, maxPos);
        GraphInDebug();
        
        foreach (var point in graph.Vertices)
        {
            GameObject cityDot;
            cityDot = Instantiate(cityDotpf, new Vector3(point.position.x, point.position.y, 0), Quaternion.identity);
            cityDot.name = ("[" + point.index.ToString("00") + "] ");
            cityDots.Add(cityDot);
        }
    }
    
}
