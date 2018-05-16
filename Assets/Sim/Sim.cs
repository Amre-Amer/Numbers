using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSimClass
{
    public int level;
    public int numLevels = 5;
    public List<SimClass> sims;
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
        SimClass sim = new SimClass("0", global);
	}
	void Update()
	{
		if (global.level < global.numLevels)
		{
			Debug.Log(global.level + " ..............\n");
			for (int s = 0; s < global.sims.Count; s++)
			{
				SimClass sim = global.sims[s];
				sim.Grow();
			}
			global.level++;
		}
	}
}

public class SimClass {
	public GlobalSimClass global;
	public int age;
	public string name;
	public string nameFrom;
	public SimClass(string name0, GlobalSimClass global0) {
		name = name0;
		global = global0;
		age = 0;
		global.sims.Add(this);
		nameFrom = name0;
	}
	public void Grow() {
		age++;
		if (age >= 3) {
			int num = int.Parse(name);
			num *= 10;
			string txt = num.ToString();
			SimClass sim = new SimClass(txt, global);
			sim.nameFrom = IncrementName(name);
		}
		if (age >= 2) {
			name = IncrementName(name);
		}
		Debug.Log("level:" + global.level + " name:" + name + " age:" + age.ToString() + " from:"  + nameFrom + "\n");
	}
	public string IncrementName(string txt) {
		int num = int.Parse(txt);
        num++;
		string txtNew = num.ToString();
		return txtNew;
	}
}

