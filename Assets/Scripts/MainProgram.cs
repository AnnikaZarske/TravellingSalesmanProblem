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
using Toggle = UnityEngine.UI.Toggle;

public class MainProgram : MonoBehaviour
{
    [Header("Display settings")]
    public Vector2 minPos;
    public Vector2 maxPos;

    [Header("Connected text fields")]
    public TMP_Text bruteForceWarning;
    public TMP_Text timeText;
    public TMP_Text distanceText;
    
    [Header("Connected Interactable")]
    public TMP_InputField cityAmount;
    private int cityAmountInt = 3;
    public TMP_InputField iterationsAmount;
    private int iterationsAmountInt = 1;
    public TMP_Dropdown algorithmChoice;
    public Toggle graphToggle;
    public Toggle fileOverrideToggle;
    
    [Header("Connected items")]
    public GameObject cityDotpf;
    public GameObject dotSpacePanel;
    public LineRenderer lineRenderer;

    private List<GameObject> cityDots;
    private Graph graph;
    private bool cityButtonNumbers;
    private bool iterationsButtonNumbers;
    private Stopwatch sw = new Stopwatch();
    void Start()
    {
        cityAmount.text = "3";
        iterationsAmount.text = "1";
        
        cityDots = new List<GameObject>();
    }

    private void Update()
    {
        
        if (cityAmount.text == "1" || cityAmount.text == "2")
        {
            cityAmount.text = "3";
            Debug.LogWarning("DO NOT SET NUMBER OF CITIES UNDER 3");
        }
        if (cityButtonNumbers)
        {
            cityAmount.text = cityAmountInt.ToString();
            cityButtonNumbers = false;
        }
        if (iterationsButtonNumbers)
        {
            iterationsAmount.text = iterationsAmountInt.ToString();
            iterationsButtonNumbers = false;
        }
        
        switch (algorithmChoice.value)
        {
            case 0:
                if (cityAmountInt > 9) {
                    bruteForceWarning.GetComponent<TMP_Text>().enabled = true;
                }
                else {
                    bruteForceWarning.GetComponent<TMP_Text>().enabled = false;
                }
                break;
            case 1:
                bruteForceWarning.GetComponent<TMP_Text>().enabled = false;
                break;
            case 2:
                bruteForceWarning.GetComponent<TMP_Text>().enabled = false;
                break;
        }
    }
    
    public void onCityPlusButton()
    {
        //cityAmountInt = int.Parse(cityAmount.text);
        cityAmountInt++;
        cityButtonNumbers = true;
    }
    public void onCityMinusButton()
    {
        //cityAmountInt = int.Parse(cityAmount.text);
        cityAmountInt--;
        cityButtonNumbers = true;
    }
    public void onIterationsPlusButton()
    {
        //cityAmountInt = int.Parse(cityAmount.text);
        iterationsAmountInt++;
        iterationsButtonNumbers = true;
    }
    public void oniterationsMinusButton()
    {
        //cityAmountInt = int.Parse(cityAmount.text);
        iterationsAmountInt--;
        iterationsButtonNumbers = true;
    }

    public void OnStartOnceClick()
    {
        cityAmountInt = int.Parse(cityAmount.text);
        if (graphToggle.isOn)
        {
            GenerateGraph();
        }
        
        switch (algorithmChoice.value)
        {
            case 0:
                Debug.Log("BRUTE FORCE");
                BruteForce();
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }
    
    public void OnStartIterationsClick()
    {
        cityAmountInt = int.Parse(cityAmount.text);
        GenerateGraph();
        
        switch (algorithmChoice.value)
        {
            case 0:
                Debug.Log("BRUTE FORCE");
                //Loop for iterations
                BruteForce();
                break;
            case 1:
                break;
            case 2:
                break;
        }
    }

    public void OnStopClick()
    {
        switch (algorithmChoice.value)
        {
            case 0:
                Debug.Log("STOP BRUTE FORCE");
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

        graph = new Graph(cityAmountInt, minPos, maxPos);
        //Debug.Log("City Amount: " + cityAmountInt);
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
        
        Debug.Log("Start Brute Force");
        sw.Reset();
        sw.Start();

        List<Vertex> path = bf.ShortestPath();

        sw.Stop();
        Debug.Log("Stop Brute Force");
        
        timeText.text = sw.Elapsed.ToString();

        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = new Vector3(path[i].position.x, path[i].position.y, -0.01f);
        }

        distanceText.text = bf.minDistance.ToString("0000.00");
        Drawlines(positions);
        
        AddRecord("Brute Force", iterationsAmountInt, cityAmountInt, sw.Elapsed.TotalSeconds , bf.minDistance, "BruteForceData.csv");
        
        //Debug.Log("[" + path[i].index.ToString("00") + "] ");
        //Debug.Log("Distance: " + bf.minDistance.ToString("0.00"));
        //Debug.Log("Elapsed Time: " + sw.Elapsed);
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
    
    private void AddRecord(string algoType, int iterations, int cityAmount, double time, double distance, string filepath)
    {
        if (fileOverrideToggle.isOn)
        {
            fileOverrideToggle.isOn = false;
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, false))
                {
                    file.WriteLine(algoType + "," + iterations + "," + cityAmount + "," + time + "," + distance);
                }
            }
            catch(Exception ex)
            {
                throw new AggregateException("This program messed up: ", ex);
            }
        }
        else
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, true))
                {
                    file.WriteLine(algoType + "," + iterations + "," + cityAmount + "," + time + "," + distance);
                }
            }
            catch(Exception ex)
            {
                throw new AggregateException("This program messed up: ", ex);
            }
        }
    }
}
