using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Toggle = UnityEngine.UI.Toggle;

public class MainProgram : MonoBehaviour
{
    [Header("Display settings")]
    public Vector2 minPos;
    public Vector2 maxPos;

    [Header("Simulated Annealing Settings")] 
    public double startTemp = 25;
    public int simIterations = 200;
    public double coolingRate = 0.995f;

    [Header("Connected text fields")]
    public TMP_Text bruteForceWarning;
    public TMP_Text timeText;
    public TMP_Text distanceText;
    public TMP_Text iterationsText;
    
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
    private bool cityButtonNumbers, iterationsButtonNumbers, runningiterations = false;
    private Stopwatch sw = new Stopwatch();
    private double currentExecutionTime, currentShortestPath, avarageExecutionTime, avarageShortestPath;
    private string fileToOutputTo;
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
        cityAmountInt++;
        cityButtonNumbers = true;
    }
    public void onCityMinusButton()
    {
        cityAmountInt--;
        cityButtonNumbers = true;
    }
    public void onIterationsPlusButton()
    {
        iterationsAmountInt++;
        iterationsButtonNumbers = true;
    }
    public void oniterationsMinusButton()
    {
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
            if (graphToggle.isOn) {
                GenerateGraph();
            }
            switch (algorithmChoice.value)
            {
                case 0:
                    Debug.Log("BRUTE FORCE");
                    runningiterations = true;
                    fileToOutputTo = "TSPBruteForceData.csv";
                    StartCoroutine(CallBruteForceIterations());
                    break;
                case 1:
                    Debug.Log("NEAREST NEIGHBOUR");
                    runningiterations = true;
                    fileToOutputTo = "TSPNearestNeighbourData.csv";
                    StartCoroutine(CallNearestNeighbourIterations());
                    break;
                case 2:
                    Debug.Log("SIMULATED ANNEALING");
                    runningiterations = true;
                    fileToOutputTo = "TSPSimulatedAnnealingData.csv";
                    StartCoroutine(CallSimAnnealingIterations());
                    break;
                case 3:
                    Debug.Log("2-OPT");
                    runningiterations = true;
                    fileToOutputTo = "TSP2-OptData.csv";
                    StartCoroutine(CallTwoOptIterations());
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
                Debug.Log("STOP SIMULATED ANNEALING");
                runningiterations = false;
                StopCoroutine(CallSimAnnealingIterations());
                break;
            case 3:
                Debug.Log("STOP 2-OPT");
                runningiterations = false;
                StopCoroutine(CallTwoOptIterations());
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
        int iterations = 0;
        ResetAverage();
        for (int i = 0; i < iterationsAmountInt; i++)
        {
            if (graphToggle.isOn) {
                GenerateGraph();
            }
            currentShortestPath = 0;
            currentExecutionTime = 0;
            yield return StartCoroutine(BruteForceIterations());
            iterations = i + 1;
            CalcAverage(iterations);
            iterationsText.text = iterations.ToString();
            if (!runningiterations) {
                break;
            }
        }
        AddRecord("Brute Force", iterations, cityAmountInt, avarageExecutionTime, avarageShortestPath, fileToOutputTo);
    }

    private IEnumerator BruteForceIterations()
    {
        BruteForce bf = new BruteForce(graph);
        
        sw.Restart();
        List<Vertex> path = bf.ShortestPath();
        sw.Stop();
        
        currentExecutionTime = sw.Elapsed.TotalSeconds;
        currentShortestPath = bf.minDistance;
        
        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++) {
            positions[i] = new Vector3(path[i].position.x, path[i].position.y, -0.01f);
        }
        DrawLines(positions);

        yield return null;
    }

    private IEnumerator CallNearestNeighbourIterations()
    {
        int iterations = 0;
        ResetAverage();
        for (int i = 0; i < iterationsAmountInt; i++)
        {
            if (graphToggle.isOn) {
                GenerateGraph();
            }
            currentShortestPath = 0;
            currentExecutionTime = 0;
            yield return StartCoroutine(NearestNeighbourIterations());
            iterations = i + 1;
            CalcAverage(iterations);
            iterationsText.text = iterations.ToString();
            if (!runningiterations) {
                break;
            }
        }
        AddRecord("Nearest Neighbour", iterations, cityAmountInt, avarageExecutionTime, avarageShortestPath, fileToOutputTo);
    }

    private IEnumerator NearestNeighbourIterations()
    {
        NearestNeighbour nn = new NearestNeighbour(graph);
        
        sw.Restart();
        nn.CalcShortestPath();
        sw.Stop();
        
        currentExecutionTime = sw.Elapsed.TotalSeconds;
        currentShortestPath = nn.minDistance;
        
        List<Vertex> path = nn.ShortestPath;
        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = new Vector3(path[i].position.x, path[i].position.y, -0.01f);
        }
        DrawLines(positions);

        yield return null;
    }

    private IEnumerator CallSimAnnealingIterations()
    {
        int iterations = 0;
        ResetAverage();
        for (int i = 0; i < iterationsAmountInt; i++)
        {
            if (graphToggle.isOn) {
                GenerateGraph();
            }
            currentShortestPath = 0;
            currentExecutionTime = 0;
            yield return StartCoroutine(SimAnnealingIterations());
            iterations = i + 1;
            CalcAverage(iterations);
            iterationsText.text = iterations.ToString();
            if (!runningiterations) {
                break;
            }
        }
        AddRecord("Simulated Annealing",  iterations, cityAmountInt, avarageExecutionTime, avarageShortestPath, fileToOutputTo);
    }
    
    private IEnumerator SimAnnealingIterations()
    {
        SimulatedAnnealing sa = new SimulatedAnnealing(graph);
        
        sw.Restart();
        sa.CalcShortestPath(startTemp, simIterations, coolingRate);
        sw.Stop();
        
        currentExecutionTime = sw.Elapsed.TotalSeconds;
        currentShortestPath = sa.minDistance;
        
        List<Vertex> path = sa.ShortestPath;
        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = new Vector3(path[i].position.x, path[i].position.y, -0.01f);
        }
        DrawLines(positions);

        yield return null;
    }

    private IEnumerator CallTwoOptIterations()
    {
        int iterations = 0;
        ResetAverage();
        for (int i = 0; i < iterationsAmountInt; i++)
        {
            if (graphToggle.isOn) {
                GenerateGraph();
            }
            currentShortestPath = 0;
            currentExecutionTime = 0;
            yield return StartCoroutine(TwoOptIterations());
            iterations = i + 1;
            CalcAverage(iterations);
            iterationsText.text = iterations.ToString();
            if (!runningiterations) {
                break;
            }
        }
        AddRecord("2-Opt",  iterations, cityAmountInt, avarageExecutionTime, avarageShortestPath, fileToOutputTo);
    }
    
    private IEnumerator TwoOptIterations()
    {
        TwoOpt to = new TwoOpt(graph);
        
        sw.Restart();
        to.CalcShortestPath();
        sw.Stop();
        
        currentExecutionTime = sw.Elapsed.TotalSeconds;
        currentShortestPath = to.minDistance;
        
        List<Vertex> path = to.shortestPath;
        Vector3[] positions = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            positions[i] = new Vector3(path[i].position.x, path[i].position.y, -0.01f);
        }
        DrawLines(positions);
        
        yield return null;
    }
    
    private void CalcAverage(int i)
    {
        if (i == 1) {
            avarageExecutionTime = currentExecutionTime;
            avarageShortestPath = currentShortestPath;
        } else {
            avarageExecutionTime = avarageExecutionTime + (currentExecutionTime - avarageExecutionTime) / (i);
            avarageShortestPath = avarageShortestPath + (currentShortestPath - avarageShortestPath) / (i);
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
        if (fileOverrideToggle.isOn) {
            fileOverrideToggle.isOn = false;
            try {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, false)) {
                    file.WriteLine(algoType + "," + iterations + "," + cityAmount + "," + time + "," + distance);
                }
            }
            catch(Exception ex) {
                throw new AggregateException("This program messed up: ", ex);
            }
        }
        else {
            try {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@filepath, true)) {
                    file.WriteLine(algoType + "," + iterations + "," + cityAmount + "," + time + "," + distance);
                }
            }
            catch(Exception ex) {
                throw new AggregateException("This program messed up: ", ex);
            }
        }
    }
}
