using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Slider = UnityEngine.UI.Slider;

public class MainProgram : MonoBehaviour
{
    public Vector2 minPos;
    public Vector2 maxPos;
    //public int cityAmount;
    
    public Slider cityAmount;
    public TMP_Dropdown algorithmChoice;
    
    public GameObject cityDotpf;
    public GameObject dotSpacePanel;
    public TMP_Text cityAmountText;
    public LineRenderer lineRenderer;

    private List<GameObject> cityDots;
    private Graph graph;
    void Start()
    {
        //lineRenderer = GetComponent<LineRenderer>();
        cityDots = new List<GameObject>();
        //graph = new Graph((int)cityAmount.value, minPos, maxPos);
    }

    private void Update()
    {
        cityAmountText.text = cityAmount.value.ToString();
    }

    public void OnStartClick()
    {
        GenerateGraph();
        
        switch (algorithmChoice.value)
        {
            case 0:
                Debug.Log("BRUTE FORCE");
                if ((int)cityAmount.value > 9)
                {
                    Debug.LogError("SETTING CITY AMOUNT OVER 9 TAKES A LONG TIME");
                    cityAmount.value = 9;
                }
                else
                {
                    BruteForce();
                }
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }
    
    
    private void GenerateGraph()
    {
        ClearCities();

        graph = new Graph((int)cityAmount.value, minPos, maxPos);
        //GraphInDebug();
        
        foreach (var point in graph.Vertices) {
            GameObject cityDot;
            cityDot = Instantiate(cityDotpf, new Vector3(0, 0, 0), Quaternion.identity);
            cityDot.transform.SetParent(dotSpacePanel.transform);
            cityDot.transform.localPosition = new Vector3(point.position.x, point.position.y, -0.01f);
            cityDot.name = ("City: [" + point.index.ToString("00") + "]");
            cityDots.Add(cityDot);
        }
    }

    void Drawlines(Vector3[] vertexPositions)
    {
        lineRenderer.positionCount = vertexPositions.Length;
        lineRenderer.SetPositions(vertexPositions);
    }
    
    private void BruteForce()
    {
        BruteForce bf = new BruteForce(graph);
        
        Stopwatch sw = new Stopwatch();
        Debug.Log("Start Brute Force");
        sw.Start();
        List<Vertex> path = bf.ShortestPath();
        sw.Stop();
        Debug.Log("Stop Brute Force");

        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = new Vector3(path[i].position.x, path[i].position.y, -0.01f);
        }
        
        Drawlines(positions);
            
        //Debug.Log("[" + path[i].index.ToString("00") + "] ");
        Debug.Log("Distance: " + bf.minDistance.ToString("0.00"));
        Debug.Log("Elapsed Time: " + sw.Elapsed);
    }

    private void ClearCities()
    {
        foreach (GameObject citydot in cityDots) {
            Destroy(citydot);
        }
        cityDots.Clear();
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
}
