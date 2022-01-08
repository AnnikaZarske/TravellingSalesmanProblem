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
    public float waitTime = 0.2f;

    [Header("Output settings")] 
    [Tooltip("File name must end in .csv")]
    public string fileToOutputTo = "TSPData.csv";

    [Header("Ant Colony Settings")] 
    public int antCount = 100;
    public int antIterations = 100;
    public double ro = 0.5f;
    public int alpha = 1;
    public int beta = 2;

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
    public Toggle displayWait;
    
    [Header("Connected items")]
    public GameObject cityDotpf;
    public GameObject dotSpacePanel;
    public LineRenderer lineRenderer;

    private List<GameObject> cityDots;
    private Graph graph;
    private bool cityButtonNumbers, iterationsButtonNumbers, runningiterations = false;
    private Stopwatch sw = new Stopwatch();
    private double currentExecutionTime, currentShortestPath, avarageExecutionTime, avarageShortestPath;
    void Start()
    {
        cityAmount.text = "3";
        iterationsAmount.text = "1";
        
        cityDots = new List<GameObject>();
    }

    private void Update()
    {
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

    public void onCityEdit()
    {
        cityAmountInt = int.Parse(cityAmount.text); 
    }

    public void onIterationsEdit()
    {
        iterationsAmountInt = int.Parse(iterationsAmount.text);
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

    public void OnStartIterationsClick()
    {
        bool error = false;
        
        if (cityAmountInt < 3)
        {
            cityAmount.text = "3";
            error = true;
            Debug.LogError("DO NOT SET NUMBER OF CITIES UNDER 3");
        }
        if (!error)
        {
            switch (algorithmChoice.value)
            {
                case 0:
                    Debug.Log("BRUTE FORCE");
                    runningiterations = true;
                    StartCoroutine(CallBruteForceIterations());
                    break;
                case 1:
                    Debug.Log("NEAREST NEIGHBOUR");
                    runningiterations = true;
                    StartCoroutine(CallNearestNeighbourIterations());
                    break;
                case 2:
                    Debug.Log("ANT COLONY");
                    runningiterations = true;
                    StartCoroutine(CallAntColonyIterations());
                    break;
            }
        }
    }

    public void OnStopClick()
    {
        switch (algorithmChoice.value)
        {
            case 0:
                Debug.Log("STOP BRUTE FORCE");
                runningiterations = false;
                StopCoroutine(CallBruteForceIterations());
                break;
            case 1:
                Debug.Log("STOP NEAREST NEIGHBOUR");
                runningiterations = false;
                StopCoroutine(CallNearestNeighbourIterations());
                break;
            case 2:
                Debug.Log("STOP ANT COLONY");
                runningiterations = false;
                StopCoroutine(CallAntColonyIterations());
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

    void DrawLines(Vector3[] vertexPositions)
    {
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = vertexPositions.Length;
        lineRenderer.SetPositions(vertexPositions);
    }

    private IEnumerator CallBruteForceIterations()
    {
        ResetAverage();
        for (int i = 0; i < iterationsAmountInt; i++)
        {
            if (graphToggle.isOn)
            {
                GenerateGraph();
            }
            currentShortestPath = 0;
            currentExecutionTime = 0;
            yield return StartCoroutine(BruteForceIterations());
            CalcAverage(i);
        }
        AddRecord("Brute Force", iterationsAmountInt, cityAmountInt, avarageExecutionTime, avarageShortestPath, fileToOutputTo);
    }

    private IEnumerator BruteForceIterations()
    {
        BruteForce bf = new BruteForce(graph);
        
        sw.Reset();
        sw.Start();

        List<Vertex> path = bf.ShortestPath();

        sw.Stop();
        
        currentExecutionTime = sw.Elapsed.TotalSeconds;
        currentShortestPath = bf.minDistance;

        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = new Vector3(path[i].position.x, path[i].position.y, -0.01f);
        }
        DrawLines(positions);

        if (displayWait.isOn)
        {
            yield return new WaitForSeconds(waitTime);
        }
        else
        {
            yield return null;
        }
    }

    private IEnumerator CallNearestNeighbourIterations()
    {
        ResetAverage();
        for (int i = 0; i < iterationsAmountInt; i++)
        {
            if (graphToggle.isOn)
            {
                GenerateGraph();
            }
            currentShortestPath = 0;
            currentExecutionTime = 0;
            yield return StartCoroutine(NearestNeighbourIterations());
            CalcAverage(i);
        }
        AddRecord("Nearest Neighbour", iterationsAmountInt, cityAmountInt, avarageExecutionTime, avarageShortestPath, fileToOutputTo);
    }

    private IEnumerator NearestNeighbourIterations()
    {
        NearestNeighbour nn = new NearestNeighbour(graph);
        
        sw.Reset();
        sw.Start();

        nn.CalcShortestPath();
        List<Vertex> path = nn.ShortestPath;
        sw.Stop();

        currentExecutionTime = sw.Elapsed.TotalSeconds;
        currentShortestPath = nn.minDistance;

        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = new Vector3(path[i].position.x, path[i].position.y, -0.01f);
        }
        DrawLines(positions);
        
        if (displayWait.isOn) {
            yield return new WaitForSeconds(waitTime);
        } else {
            yield return null;
        }
    }

    private IEnumerator CallAntColonyIterations()
    {
        ResetAverage();
        for (int i = 0; i < iterationsAmountInt; i++)
        {
            if (graphToggle.isOn)
            {
                GenerateGraph();
            }
            currentShortestPath = 0;
            currentExecutionTime = 0;
            yield return StartCoroutine(AntColonyIterations());
            CalcAverage(i);
        }
        AddRecord("Ant Colony", iterationsAmountInt, cityAmountInt, avarageExecutionTime, avarageShortestPath, fileToOutputTo);
    }
    
    private IEnumerator AntColonyIterations()
    {
        AntColony antColony = new AntColony(graph, ro, alpha, beta);
        
        sw.Reset();
        sw.Start();

        antColony.CalcShortestPath(antIterations, antCount);
        List<Vertex> path = antColony.ShortestPath;
        sw.Stop();
        
        currentExecutionTime = sw.Elapsed.TotalSeconds;
        currentShortestPath = antColony.minDistance;

        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = new Vector3(path[i].position.x, path[i].position.y, -0.01f);
        }
        DrawLines(positions);
        
        if (displayWait.isOn)
        {
            yield return new WaitForSeconds(waitTime);
        }
        else
        {
            yield return null;
        }
    }

    private void CalcAverage(int i)
    {
        if (i == 0) {
            avarageExecutionTime = currentExecutionTime;
            avarageShortestPath = currentShortestPath;
        } else {
            avarageExecutionTime = avarageExecutionTime + (currentExecutionTime - avarageExecutionTime) / (i + 1);
            avarageShortestPath = avarageShortestPath + (currentShortestPath - avarageShortestPath) / (i + 1);
        }
        timeText.text = avarageExecutionTime.ToString("0.00000000");
        distanceText.text = avarageShortestPath.ToString("0000.00");
    }

    private void ResetAverage()
    {
        avarageExecutionTime = 0;
        avarageShortestPath = 0;
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
