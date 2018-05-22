using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FibNNManager {
	public int numLevels;
	public List<FibNNNode>[] fibNodes;
	public int level;
	public GameObject parent;
	public GameObject parentNodes;
	public int indexData;
	public int cntNodesLastLevel;
	public int cntNodes;
	public int cntGos;
	public bool ynMesh = false;
	public GameObject meshGo;
	public FibNNManager(int numLevels0) {
 		numLevels = numLevels0;
		InitParent();
		InitLevels();
		StartLevels();
		Debug.Log("FibManager:numLevels:" + numLevels + "\n");
	}
	public void InitMesh() {
		meshGo = new GameObject("meshGo");
	}
	public void InitParent() {
		parent = new GameObject("misc");
		parentNodes = new GameObject("nodes");
	}
	public void InitLevels() {
		fibNodes = new List<FibNNNode>[numLevels];
        for (int l = 0; l < numLevels; l++)
        {
            fibNodes[l] = new List<FibNNNode>();
        }
	}
	public void AdvanceCurrentLevel() {
		for (int n = 0; n < fibNodes[level].Count; n++) {
			FibNNNode fibNode = fibNodes[level][n];
			string nam = fibNode.name;
			if (IsAdult(fibNode) == true) {
				Birth(fibNode);
			}
			Continue(fibNode);
		}
	}
	public void Advance() {
		if (level < numLevels) {
			if (level < numLevels - 1)
            {
				AdvanceCurrentLevel();
                ShowCurrentLevel();
			}
		} else {
			GotoLastLevel();
			ShowStatsLevel(level);
			StreamData(level);
            ShowCurrentLevel();
            Process();
            UpdateFibNodes();
		}
		level++;
	}
	public void StreamData(int lev)
    {
		bool ynRandom = false;
		if (indexData % 100 > 50) {
			ynRandom = true;
		}
		Debug.Log("Data:" + indexData + "\n");
        for (int n = 0; n < fibNodes[lev].Count; n++)
        {
            FibNNNode fibNode = fibNodes[lev][n];
			//float num = (n + indexData) % 2 * 10;
			float num = Mathf.Cos((n + indexData) * 10 * Mathf.Deg2Rad) * 10;
			if (ynRandom == true) {
				num += Random.Range(-1f, 1f) * 3;
			}
            fibNode.value = num;
        }
		indexData++;
    }
	public void GotoLastLevel() {
		level = numLevels - 1;
	}
	public void ShowStatsLevel(int lev) {
		cntNodesLastLevel = fibNodes[lev].Count;
		Debug.Log("Nodes:" + cntNodes + " lastLevel:" + cntNodesLastLevel + " cntGos:" + cntGos + "\n");
	}
	public void UpdateFibNodes()
	{
		for (int lev = 0; lev < numLevels; lev++) {
			UpdateFibNodesInLevel(lev);
		}
	}
	public void UpdateFibNodesInLevel( int lev) {
		foreach(FibNNNode fibNode in fibNodes[lev]) {
			fibNode.Update();			
		}
	}
	public bool IsAdult(FibNNNode fibNode) {
		string nam = fibNode.name;
		if (nam.Substring(nam.Length - 1, 1) != "0")
        {
			return true;
		} else {
			return false;
		}
	}
	public void Birth(FibNNNode fibNode) {
		string nam = AddZeroName(fibNode.name);
        FibNNNode fibNodeOffspring = new FibNNNode(nam, this);
        fibNodeOffspring.fibNodeFrom = fibNode;
        fibNode.fibNodeOffspring = fibNodeOffspring;
		AddFibNodeToLevel(fibNodeOffspring, level + 1);
	}
	public void Continue(FibNNNode fibNode) {
		string nam = fibNode.name;
		nam = IncrementName(nam);
        FibNNNode fibNodeContinue = new FibNNNode(nam, this);
        fibNodeContinue.fibNodeFrom = fibNode;
        fibNode.fibNodeToContinue = fibNodeContinue;
		AddFibNodeToLevel(fibNodeContinue, level + 1);
	}
	public void StartLevels()
    {
		int lev = 0;
        string nam = "0";
        FibNNNode fibNode = new FibNNNode(nam, this);
        fibNode.fibNodeFrom = fibNode;
        AddFibNodeToLevel(fibNode, lev);
    }
	public string AddZeroName(string nam) {
		return (int.Parse(nam) * 10).ToString();
	}
	public string IncrementName(string nam) {
		return (int.Parse(nam) + 1).ToString();
	}
	public void AddFibNodeToLevel(FibNNNode fibNode, int lev) {
		fibNodes[lev].Add(fibNode);
        fibNode.level = lev;
        fibNode.index = fibNodes[lev].Count;
        fibNode.Update();
	}
	public void Process() {
		for (int lev = numLevels - 2; lev >= 0; lev--)	{
			string txt = "";
			for (int n = 0; n < fibNodes[lev].Count; n++)
			{
				int cnt = 0;
				float sum = 0;
				FibNNNode fibNode = fibNodes[lev][n];
				txt += "name:" + fibNode.name;
				if (fibNode.fibNodeOffspring != null) {
					cnt++;
					sum += fibNode.fibNodeOffspring.value;
					txt += " offspring:" + fibNode.fibNodeOffspring.name + "(" + fibNode.fibNodeOffspring.value + ")";
				}
				if (fibNode.fibNodeToContinue != null) {
					cnt++;
					sum += fibNode.fibNodeToContinue.value;
					txt += " continue:" + fibNode.fibNodeToContinue.name + "(" + fibNode.fibNodeToContinue.value + ")";
				}
//				fibNode.value = (Sigmoid(sum) - .5f) * 10;
				fibNode.value = Sigmoid(sum) * 10;
				//fibNode.value = sum / (float)cnt;
				//float num = sum;
				//fibNode.value = num;
				txt += " = " + sum.ToString("F2") + " | ";
			}
		}	
	}
	public float Sigmoid(float f) {
		return 1 / (1 + Mathf.Exp(-f));		
	}
	public void ShowLevels() {
		for (int lev = 0; lev < numLevels; lev++) {
			ShowLevel(lev);
		}
	}
	public void ShowCurrentLevel() {
		ShowLevel(level);
	}
	public void ShowLevel(int lev)
    {
        string txt = "";
        foreach (FibNNNode fibNode in fibNodes[lev])
        {
			txt += fibNode.name + "(" + fibNode.value.ToString("F0") + "), ";
        }
//		Debug.Log("Level:" + lev + " = " + txt + "\n");
    }
	public GameObject CreateText(GameObject go, string txt)
    {
		if (ynMesh == true) return null;
        GameObject goText = new GameObject("text");
		cntGos++;
		goText.name = txt;
		goText.transform.SetParent(GameObject.Find("Canvas").transform);
		goText.transform.eulerAngles = go.transform.eulerAngles;
		goText.transform.position = go.transform.position + go.transform.forward * -.67f;
		goText.transform.localScale = new Vector3(.02f, .02f, .02f);
		Text text = goText.AddComponent<Text>();
		RectTransform rect = goText.GetComponent<RectTransform>();
        Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = font;
        text.fontSize = 40;
		text.name = "text |" + goText.name + "|";
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.alignment = TextAnchor.MiddleCenter;
        text.text = txt;
        return goText;
    }
	public void MakeMaterialTransparent(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}
