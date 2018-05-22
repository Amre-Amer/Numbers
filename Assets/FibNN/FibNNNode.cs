using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FibNNNode
{
    public float value;
    public string name;
    public int level = -1;
    public int index = -1;
	public FibNNNode fibNodeFrom;
    public FibNNManager fibManager;
	public FibNNNode fibNodeToContinue;
	public FibNNNode fibNodeOffspring;
    public GameObject go;
    public GameObject goLink;
    public GameObject goTextName;
    public GameObject goTextValue;
    public GameObject goValue;
	public GameObject goGraph;
	public GameObject goGraphBack;
	public GameObject pointGo;
	public float maxHeight = 10;
	public Vector3 position;
	public Vector3 posPoint;
	public FibNNNode(string name0, FibNNManager fibManager0)
    {
        name = name0;
        fibManager = fibManager0;
        //
		if (fibManager.ynMesh == false)
		{
			go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.parent = fibManager.parentNodes.transform;
            fibManager.cntGos++;
            go.name = name;
            fibManager.MakeMaterialTransparent(go.GetComponent<Renderer>().material);
			//
			goTextName = fibManager.CreateText(go, name);
			goTextValue = fibManager.CreateText(go, FormatValue(value));
			//
			goValue = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			goValue.transform.parent = fibManager.parent.transform;
			fibManager.cntGos++;
			goValue.name = "value " + name;
			goGraphBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
			goGraphBack.transform.parent = fibManager.parent.transform;
			fibManager.cntGos++;
			goGraphBack.name = "graphback " + name;
			fibManager.MakeMaterialTransparent(goGraphBack.GetComponent<Renderer>().material);
			//
			goGraph = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goGraph.transform.parent = fibManager.parent.transform;
            fibManager.cntGos++;
            goGraph.name = "graph " + name;
            fibManager.MakeMaterialTransparent(goGraph.GetComponent<Renderer>().material);
		}
		//
        goLink = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fibManager.cntGos++;
        goLink.transform.parent = fibManager.parent.transform;
        goLink.name = "link " + name;
//		goLink.name = "link " + name + " -> " + fibNodeFrom.name;
        goLink.GetComponent<Renderer>().material.color = Color.grey;
        //
		pointGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointGo.transform.parent = fibManager.parent.transform;
		//
		fibManager.cntNodes++;
    }
    public void Update()
    {
        UpdateGo();
        UpdateTextName();
        UpdateTextValue();
		UpdateGraph();
		UpdatePoint();
		UpdateLink();
    }
    public string FormatValue(float val)
    {
        return val.ToString("F2");
    }
	public void UpdatePoint() {
		pointGo.transform.position = posPoint;		
	}
	public void UpdateGraph() {
		float h = value;
		posPoint = position + Vector3.forward * -h;
		if (fibManager.ynMesh == true)
		{
			return;
		}
		Vector3 pos = position + Vector3.up * 2;
		goGraphBack.transform.position = pos;
		goGraphBack.GetComponent<Renderer>().material.color = new Color(.25f, .25f, .25f, .125f);
		//
		goGraph.transform.position = pos;
		//
		goGraph.transform.position += goGraph.transform.forward * -h / 2;
        goGraph.transform.localScale = new Vector3(1, h / 10, h);
		if (value > 0)
		{
			goGraph.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1);
		} else {
			goGraph.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1);
		}
	}
    public void UpdateTextValue()
    {
		if (fibManager.ynMesh == true) return;
        string txt = FormatValue(value);
        Text text = goTextValue.GetComponent<Text>();
        text.text = txt;
        goTextValue.transform.position = go.transform.position + go.transform.forward * -.67f;
        goTextValue.transform.position += go.transform.up * 1f;
        //
        goValue.transform.position = go.transform.position + go.transform.up * 1;
        //
        Vector3 sca = new Vector3(1, .5f, 1);
        sca.x = txt.Length * .5f;
        if (sca.x < 1) sca.x = 1;
        goValue.transform.localScale = new Vector3(sca.x, sca.y, sca.z);
        goValue.transform.eulerAngles = new Vector3(90, 0, 0);
        if (value == 0)
        {
            goValue.GetComponent<Renderer>().material.color = Color.black;
            text.color = Color.white;
        }
        else
        {
            goValue.GetComponent<Renderer>().material.color = Color.white;
            text.color = Color.black;
        }
    }
    public void UpdateTextName()
    {
		if (fibManager.ynMesh == true) return;
        string txt = name;
        Text text = goTextName.GetComponent<Text>();
        text.text = txt;
        goTextName.transform.position = go.transform.position + go.transform.forward * -.67f;
        Vector3 sca = go.transform.localScale;
        sca.x = txt.Length * .5f;
        if (sca.x < 1) sca.x = 1;
        go.transform.localScale = new Vector3(sca.x, sca.y, sca.z);
        if (fibManager.IsAdult(this) == true)
        {
            text.color = Color.white;
        }
        else
        {
            text.color = Color.black;
        }
    }
    public void UpdateGo()
    {
		position = new Vector3(index * 3, level * 14, 0);
		if (fibManager.ynMesh == true) return;
		go.transform.position = position;
        go.name = name;
        if (fibManager.IsAdult(this) == true)
        {
            go.GetComponent<Renderer>().material.color = new Color(0, 0, 1, .5f);
        }
        else
        {
            go.GetComponent<Renderer>().material.color = new Color(.75f, .75f, .75f, .5f);
        }
    }
    public void UpdateLink()
    {
		goLink.transform.position = (posPoint + fibNodeFrom.posPoint) / 2;
		goLink.transform.LookAt(posPoint);
		float dist = Vector3.Distance(posPoint, fibNodeFrom.posPoint);
        goLink.transform.localScale = new Vector3(.1f, .1f, dist);
    }
}
