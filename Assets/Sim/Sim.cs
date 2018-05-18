using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalSimClass
{
    public int level;
	public int cntSims;
	public int numLevels = 9; //10;
    public List<SimClass> sims;
	public List<string>[] nodes;
	public List<bool>[] values;
	public bool ynFlag;
	public bool ynRadial = false; // true;
	public float angRange = 360;
	public float timeStart;
	public float delay = .25f;
	public bool ynDelay = true;
	public int dataOffset = 0; 
	public int dataStride = 4; 
	public int dataModulus = 0;
	public GameObject parent;
	public GlobalSimClass() {
		sims = new List<SimClass>();
        nodes = new List<string>[numLevels];
        values = new List<bool>[numLevels];
		timeStart = Time.realtimeSinceStartup;
		parent = new GameObject("parent");
//		SimClass sim = new SimClass("0", this);
	}
}

public class Sim : MonoBehaviour
{
	GlobalSimClass global;
	void Start()
	{
		Test();
	}
	void Test() {
		global = new GlobalSimClass();
        SimClass sim = new SimClass("0", global);
	}
	void Update()
	{
		if (global.level < global.numLevels)
		{
			global.nodes[global.level] = new List<string>();
			global.values[global.level] = new List<bool>(); 
			global.cntSims = 0;
			for (int s = 0; s < global.sims.Count; s++)
			{
				SimClass sim = global.sims[s];
				sim.Grow();
			}
			ShowPeers();
			global.level++;
		}
		else
		{
			Process();
		}
	}
	void Process() {
		if (global.ynDelay == true && Time.realtimeSinceStartup - global.timeStart < global.delay) {
			return;
		}
		global.timeStart = Time.realtimeSinceStartup;
		//Debug.Log("Process:\n");
		if (global.ynFlag == true) {
//			return;
		}
		//		global.ynFlag = true;
		ClearValuesAndData();
		LoadData();
		for (int level = global.numLevels - 2; level > 0; level--) {
			for (int n = 0; n < global.nodes[level].Count; n++) {
				string nam = global.nodes[level][n];
				bool val = global.values[level][n];
				bool ynResult = GetValueNextLevel(nam, level);
				global.values[level][n] = ynResult;
				ShowValue(nam, level, ynResult);
			}
		}
		ShowData();
		global.dataOffset++;
//		global.dataStride++;
//		global.dataModulus++;
	}
	void ClearValuesAndData() {
		if (global.parent != null) {
			DestroyImmediate(global.parent);
		}
		global.parent = new GameObject("parent");
	}
	void LoadData()
    {
//		Debug.Log("LoadData:dataOffset:" + global.dataOffset + " dataStride:" + global.dataStride + " dataModulus:" + global.dataModulus + "\n");
        List<bool> groupValues = global.values[global.numLevels - 1];
		string txt = "";
        for (int n = 0; n < groupValues.Count; n++)
        {
//			if ((n + global.dataOffset) % global.dataStride == global.dataModulus)
			if ((n + global.dataOffset) % 5 >= 3)
            {
                groupValues[n] = true;
            }
            else
            {
                groupValues[n] = false;
            }
			//if (n == 0)
			//{
			//	Debug.Log("n:" + n + " value:" + groupValues[n] + "\n");
			//}
			txt += groupValues[n].ToString() + ", ";
        }
		Debug.Log("LoadData:" + txt + "\n");
    }
	void ShowData() {
		int level = global.numLevels - 1;
		for (int n = 0; n < global.nodes[level].Count; n++)
        {
            string nam = global.nodes[level][n];
            bool ynResult = GetValue(nam, level);
			ShowValue(nam, level, ynResult);
        }
	}
	void ShowValue(string nam, int level, bool yn) {
		GameObject goNode = GameObject.Find(nam);
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
		go.transform.parent = global.parent.transform;
		go.transform.position = goNode.transform.position;
		go.transform.position += go.transform.up * -1;
		go.transform.eulerAngles = goNode.transform.eulerAngles;
		go.transform.Rotate(-90, 0, 0);
		if (yn == true) {
			go.GetComponent<Renderer>().material.color = Color.green;			
		} else {
			go.GetComponent<Renderer>().material.color = Color.red;           
		}
	}
	bool GetValue(string nam, int level)
    {
        int n = global.nodes[level].IndexOf(nam);
		bool yn = global.values[level][n];
		return yn;
    }
	bool GetValueNextLevel(string nam, int level) {
		// any true value passes
		string txt = "";
		bool yn = false;
		int cnt = 0;
		txt = SimClass.IncrementName(nam);
        int n = global.nodes[level + 1].IndexOf(txt);
        if (global.values[level + 1][n] == true)
        {
			cnt++;
        }
		if (SimClass.HasZeroRight(nam) == false)
        {
            txt = SimClass.AddZero(nam);
            n = global.nodes[level + 1].IndexOf(txt);
            if (global.values[level + 1][n] == true)
            {
				cnt++;
            }
        }
		if (cnt >= 1) {
			yn = true;
		}
		return yn;
	}
	void ShowLinkPrevious(string namTo) {
		string nameFrom = GetPrevious(namTo);
		ShowLink(nameFrom, namTo);		
	}
	void ShowLink(string namFrom, string namTo) {
		GameObject goFrom = GameObject.Find(namFrom);
        GameObject goTo = GameObject.Find(namTo);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "link " + namFrom + " -> " + namTo;
        Vector3 posFrom = goFrom.transform.position;
        Vector3 posTo = goTo.transform.position;
        go.transform.position = (posFrom + posTo) / 2;
        go.transform.LookAt(posTo);
        go.transform.localScale = new Vector3(.1f, .1f, Vector3.Distance(posFrom, posTo));
	}
	string GetPrevious(string nam) {
		string result = "?";
		if (SimClass.HasZeroRight(nam) == true) 
        {
			result = nam.Substring(0, nam.Length - 1);
		} else {
			result = (int.Parse(nam) - 1).ToString();			
		}
		if (result == "") {
			result = "0";
		}
		return result;
	}
	void ShowPeers() {
		List<string> group = global.nodes[global.level];
		group.Sort();
		int num = group.Count;
		Debug.Log(global.level + " = " + num + " \n");
		for (int n = 0; n < num; n++) {
			string nam = group[n];
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = nam;
			//			go.transform.position = GetPos(n);
			if (global.ynRadial == true) {
				SetPosRotRadial(n, go);
			} else {
	            SetPosition(n, go);
			}
			ColorPeer(n, go);
			float wText = .65f;
            float s = nam.Length * wText;
			if (s < 1) s = 1;
			go.transform.localScale = new Vector3(s, 1, 1);
			ShowLinkPrevious(nam);
			CreateText(go, nam);
		}
	}
	void SetPosition(int n, GameObject go)
    {
        go.transform.position = GetPos(n);
    }
    Vector3 GetPos(int n)
    {
        float w = 4;
        float h = -4;
        float x = n * w - (global.nodes[global.level].Count * w / 2);
        float y = global.level * h;
        float z = 0;
        return new Vector3(x, y, z);
    }
    void SetPosRotRadial(int n, GameObject go)
    {
        float w = 4;
        float h = -4;
        int num = global.nodes[global.level].Count;
        if (num <= 1)
        {
            go.transform.position = Vector3.zero;
            return;
        }
		float angDiff = global.angRange / num;
        float ang = n * angDiff;
        float rad = (global.nodes[global.level].Count - 1) * w / (2 * Mathf.PI);
        float x = rad * Mathf.Cos(ang * Mathf.Deg2Rad);
        float y = global.level * h;
        float z = rad * Mathf.Sin(ang * Mathf.Deg2Rad);
        go.transform.position = new Vector3(x, y, z);
        go.transform.eulerAngles = new Vector3(0, 270 - ang, 0);
    }
	Text CreateText(GameObject go, string txt)
    {
        GameObject go0 = new GameObject("text");
        go0.name = txt;
        go0.transform.SetParent(GameObject.Find("Canvas").transform);
		go0.transform.eulerAngles = go.transform.eulerAngles;
		go0.transform.position = go.transform.position + go.transform.forward * -.67f;
        go0.transform.localScale = new Vector3(.02f, .02f, .02f);
        Text text = go0.AddComponent<Text>();
        RectTransform rect = go0.GetComponent<RectTransform>();
        Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = font;
        text.fontSize = 40;
        text.name = "." + go0.name + ".";
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
		if (SimClass.HasZeroRight(txt) == true)
        {
            text.color = Color.black;
        }
        else
        {
            text.color = Color.white;
        }
        text.alignment = TextAnchor.MiddleCenter;
        text.text = txt;
        return text;
    }
	void ColorPeer(int n, GameObject go) {
		string nam = global.nodes[global.level][n];
		if (SimClass.HasZeroRight(nam) == true) 
        {
            go.GetComponent<Renderer>().material.color = Color.white;
        }
        else
        {
            go.GetComponent<Renderer>().material.color = Color.blue;
        }
	}
}

public class SimClass {
	public GlobalSimClass global;
	public int age;
	public string name;
	public string nameFrom;
	public GameObject go;
	public GameObject goLink;
	public SimClass(string name0, GlobalSimClass global0) {
		name = name0;
		global = global0;
		age = 0;
		global.sims.Add(this);
		nameFrom = name0;
		global.cntSims++;
	}
	public SimClass Find(string txt, GlobalSimClass global) {
		SimClass result = null;
		foreach(SimClass sim in global.sims) {
			if (sim.name == txt) {
				result = sim;
				break;				
			}
		}
		return result;
	}
	public void Grow() {
		age++;
		if (age >= 3) {
			string txt = AddZero(name);
			SimClass sim = new SimClass(txt, global);
			sim.nameFrom = IncrementName(name);
		}
		if (age >= 2) {
			nameFrom = name;
			name = IncrementName(name);
		}
		AddPeer();
	}
	public void AddPeer() {
		global.nodes[global.level].Add(name);
		global.values[global.level].Add(false);
	}
	public static string IncrementName(string txt) {
		int num = int.Parse(txt);
        num++;
		string txtNew = num.ToString();
		return txtNew;
	}
	public static string AddZero(string txt) {
		int num = int.Parse(txt);
        num *= 10;
		return num.ToString();
	}
	public static bool HasZeroRight(string nam) {
		if (nam.Substring(nam.Length - 1, 1) == "0")
        {
			return true;
		} else {
			return false;
		}
	}
}

