using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSimClass
{
    public int level;
	public int cntSims;
    public int numLevels = 7;
    public List<SimClass> sims;
	public List<string>[] peers;
	public bool ynFlag;
}

public class Sim : MonoBehaviour
{
	GlobalSimClass global;
	int numLast = 1;
	int num = 2;
	void Start()
	{
		//ShowSequence();
  //      //
		//TestSequence();
		//TestSequence();
		//TestSequence();
		//TestSequence();
		Test();
	}
	void TestSequence() {
		int sum = num + numLast;
		numLast = num;
		num = sum;
		ShowSequence();
	}
	void ShowSequence() {
		Debug.Log(numLast + " , " + num + "\n");
	}
	void Test() {
		global = new GlobalSimClass();
        global.sims = new List<SimClass>();
		global.peers = new List<string>[global.numLevels];
        SimClass sim = new SimClass("0", global);
	}
	void Update()
	{
		if (global.level < global.numLevels)
		{
			global.peers[global.level] = new List<string>();
			global.cntSims = 0;
			Debug.Log(global.level + " ..............\n");
			for (int s = 0; s < global.sims.Count; s++)
			{
				SimClass sim = global.sims[s];
				sim.Grow();
			}
			ShowPeers();
			global.level++;
		}
	}
	//void ShowLinks() {
	//	if (global.ynFlag == false && global.level == 3) {
	//		global.ynFlag = true;
	//		//string namFrom = "20";
	//		//string namTo = GetPrevious(namFrom);
	//		//ShowLink(namFrom, namTo);
	//		ShowLink("0", "1");
 //           //
	//		ShowLink("1", "2");
	//		ShowLink("2", "10");
 //           //
	//		ShowLink("2", "3");
	//		ShowLink("10", "11");
	//		ShowLink("3", "20");
	//	}		
	//}
	void ShowLinkPrevious(string namTo) {
		string nameFrom = GetPrevious(namTo);
		Debug.Log("link:" + nameFrom + " -> " + namTo + "\n");
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
		if (nam.Substring(nam.Length - 1, 1) == "0")
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
		Debug.Log(global.level + " _____\n");
		string result = "";
		List<string> group = global.peers[global.level];
		group.Sort();
		int num = group.Count;
		for (int n = 0; n < num; n++) {
			string nam = group[n];
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = nam;
			float x = n * 3 - (num * 3 / 2); //(float)n / (float)num * -4;
            float y = global.level * -4;
            float z = 0;
            go.transform.position = new Vector3(x, y, z);
			if (nam.Substring(nam.Length - 1, 1) == "0") {
				go.GetComponent<Renderer>().material.color = Color.white;
			} else {
				go.GetComponent<Renderer>().material.color = Color.blue;
			}
			ShowLinkPrevious(nam);
		}
		foreach(string txt in group) {
			result += ", " + txt;			
		}
		Debug.Log(result + "\n");
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
		//go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		//go.name = name;
		global.cntSims++;
//		UpdatePos();
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
			//UpdateGoName();
		}
		string txtDesc = "";
		if (age == 1) {
			txtDesc = "*";
		}
		//UpdatePos();
		//	AdjustHeight();
		//UpdateLink();
		AddPeer();
		Debug.Log("level:" + global.level + " name:" + name + " age:" + age.ToString() + " from:"  + nameFrom + " " + txtDesc + "\n");
	}
	public void AddPeer() {
		global.peers[global.level].Add(name);		
	}
	//public void UpdatePos() {
	//	go.transform.position = new Vector3(age * 4, 0, global.cntSims * 3);
	//}
	//public void AdjustHeight() {
	//	Vector3 sca = go.transform.localScale;
	//	sca.y = age;
	//	go.transform.localScale = sca;
	//	Vector3 pos = go.transform.position;
	//	pos.y = age;
	//	go.transform.position = pos;
	//}
	//public void UpdateLink() {
	//	SimClass sim = Find(nameFrom, global);
	//	if (sim == null) {
	//		return;
	//	}
	//	if (goLink == null)
	//	{
	//		goLink = GameObject.CreatePrimitive(PrimitiveType.Cube);
	//		goLink.GetComponent<Renderer>().material.color = Random.ColorHSV();
	//	}
	//	goLink.name = "link " + nameFrom + " " + name;
	//	Vector3 posFrom = sim.go.transform.position;
 //       Vector3 posTo = go.transform.position;
 //       goLink.transform.position = (posFrom + posTo) / 2;
 //       goLink.transform.LookAt(posTo);
 //       float dist = Vector3.Distance(posFrom, posTo);
 //       goLink.transform.localScale = new Vector3(.1f, .1f, dist);
	//}
	//public void UpdateGoName() {
	//	go.name = name;
	//}
	public string IncrementName(string txt) {
		int num = int.Parse(txt);
        num++;
		string txtNew = num.ToString();
		return txtNew;
	}
	public string AddZero(string txt) {
		int num = int.Parse(txt);
        num *= 10;
		return num.ToString();
	}
}

