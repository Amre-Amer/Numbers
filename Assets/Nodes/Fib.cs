using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fib : MonoBehaviour {
    GlobalClassTiny global;

	// Use this for initialization
	void Start () {
        global = new GlobalClassTiny();
        global.levelMax = 3;
        global.ynStep = true;
        global.delay = 2;
        global.scaleHistory = .75f;
        global.lengthLinks = 2;
        global.parentLinks = new GameObject("parentLinks");
        global.nodes = new List<NodeClass>();
        global.timeStart = Time.realtimeSinceStartup;
        Debug.Log("Level:" + global.level + "................\n");
        Show();
        //
        Add("0");
        //Find("0").AdvanceLevel();
        //Find("1").GiveBirth();
        //Find("1").AdvanceLevel();
        //Find("10").AdvanceLevel();
        //Find("11").AdvanceLevel();
        //Find("12").GiveBirth();
	}

    NodeClass Find(string txt) {
        return NodeClass.FindNode(txt, global.nodes);
    }
	
    NodeClass Add(string txt)
    {
        return new NodeClass(txt, global);
    }

    // Update is called once per frame
	void Update () {
        if (global.ynStep == true && Time.realtimeSinceStartup - global.timeStart < global.delay || global.level >= global.levelMax) {
            return;
        }
        global.level++;
        global.timeStart = Time.realtimeSinceStartup;
        UpdateNodes();
	}

    void UpdateNodes() {
        Debug.Log("Level:" + global.level + "................\n");
        for (int n = 0; n < global.nodes.Count; n++) {
            NodeClass node = global.nodes[n];
            node.AdvanceLevel();
        }
        Show();
    }

    void Show() {
        for (int n = 0; n < global.nodes.Count; n++)
        {
            NodeClass node = global.nodes[n];
            Debug.Log("n:" + n + " = " + node.go.name + "\n");
        }
    }
}

public class GlobalClassTiny {
    public List<NodeClass> nodes;
    public float timeStart;
    public float delay;
    public bool ynStep;
    public int level;
    public int levelMax;
    public float lengthLinks;
    public float scaleHistory;
    public GameObject parentLinks;
}

public class NodeClass {
    public bool ynJustBorn;
    public GameObject go;
    public int index;
    GlobalClassTiny global;
    public Text textGo;
    public NodeClass(string name0, GlobalClassTiny global0) {
        global = global0;
        ynJustBorn = false;
        go = AddGo(name0);
        Vector3 pos = getTextPos(go);
        textGo = CreateText(pos, go.name);
        AddToNodes(this);
    }
    public static NodeClass FindNode(string nameCheck, List<NodeClass>nodes) {
        NodeClass result = null;
        foreach( NodeClass nodeCheck in nodes) {
            if (nodeCheck.go.name == nameCheck) {
                result = nodeCheck;
                break;
            }
        }
        return result;
    }
    public GameObject AddGo(string txt)
    {
        GameObject goResult = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        goResult.name = txt;
        goResult.transform.localScale = new Vector3(1f, .1f, 1f);
        AdjustColorByName(goResult);
        goResult.name = txt;
        return goResult;
    }
    public void AddToNodes(NodeClass node)
    {
        global.nodes.Add(node);
        node.index = global.nodes.Count;
    }
    public void AdvanceLevel()
    {
        if (ynJustBorn == true) {
            ynJustBorn = false;
            return;
        }
        Vector3 pos = go.transform.position;
        LeaveCopy();
        if (IsAdolescent(go)) {
            AddOneToName();
            MoveForward();
            AddLink(pos, go.transform.position);
        } else {
            GiveBirth();
            AddOneToName();
            MoveForward();
            AddLink(pos, go.transform.position);
        }
    }
    public void GiveBirth() {
        string nameNew = AddZeroToName(go.name);
        NodeClass node = new NodeClass(nameNew, global);
        node.ynJustBorn = true;
        node.MatchPosition(go);
        node.MoveForward();
        node.MoveRight();
        AddLink(node.go.transform.position, go.transform.position);
    }
    public void MatchPosition(GameObject go0)
    {
        go.transform.position = go0.transform.position;
        Vector3 pos = getTextPos(go);
        textGo.transform.position = pos; 
    }
    public void LeaveCopy()
    {
        string nameCopy = go.name;
        GameObject goCopy = AddGo(nameCopy);
        goCopy.transform.localScale = new Vector3(global.scaleHistory, .1f, global.scaleHistory);
        goCopy.transform.position = go.transform.position;
        Text textGo0 = CreateText(getTextPos(goCopy), goCopy.name);
        AdjustTextColor(goCopy, textGo0);
    }
    public void AddLink(Vector3 posFrom, Vector3 posTo) {
        GameObject go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go0.transform.parent = global.parentLinks.transform;
        go0.name = "link";
        float sca = Vector3.Distance(posFrom, posTo);
        go0.transform.position = (posFrom + posTo) / 2;
        go0.transform.LookAt(posTo);
        go0.transform.localScale = new Vector3(.1f, .1f, sca);
        go0.GetComponent<Renderer>().material.color = Color.gray;
    }
    public string AddZeroToName(string nameCheck)
    {
        return nameCheck += "0";
    }
    public void AddOneToName()
    {
        string nameCheck = go.name;
        nameCheck = AddNumberToName(nameCheck, 1);
        go.name = nameCheck;
        AdjustColorByName(go);
        AdjustText();
    }
    public void AdjustText() {
        textGo.text = go.name;
        AdjustTextColor(go, textGo);
    }
    public void AdjustTextColor(GameObject go0, Text textGo0) {
        Color col = Color.black;
        if (IsAdolescent(go0))
        {
            col = Color.blue;
        }
        else
        {
            col = Color.white;
        }
        textGo0.color = col;
    }
    public Vector3 getTextPos(GameObject go0) {
         return go0.transform.position + Vector3.up * .15f;
    }
    public void MoveForward() {
        go.transform.position += Vector3.forward * global.lengthLinks;
        textGo.transform.position += Vector3.forward * global.lengthLinks;
    }
    public void MoveRight()
    {
        go.transform.position += Vector3.right * global.lengthLinks;
        textGo.transform.position += Vector3.right * global.lengthLinks;
    }
    public void AdjustColorByName(GameObject go0) {
        Color col = Color.black;
        if (IsAdolescent(go0)) {
            col = Color.white;            
        } else {
            col = Color.blue;            
        }
        Color colBefore = go0.GetComponent<Renderer>().material.color;
        go0.GetComponent<Renderer>().material.color = col;
    }
    public int GetY() {
        return GetLevel();      
    }
    public string SubtractOneFromName(string nameCheck)
    {
        return AddNumberToName(nameCheck, -1).ToString();
    }
    public string AddNumberToName(string nameCheck, int num)
    {
        int n = int.Parse(nameCheck);
        n += num;
        return n.ToString();
    }
    public string LoseZero(string nameCheck) {
        return GetWithoutRightChar(nameCheck);
    }
    public string GetWithoutRightChar(string nameCheck) {
        return nameCheck.Substring(0, nameCheck.Length - 1);
    }
    public string GetRightChar(string nameCheck) {
        return nameCheck.Substring(nameCheck.Length - 1, 1);
    }
    public bool IsAdolescent(GameObject go0) {
        string nameCheck = go0.name;
        string rightChar = GetRightChar(nameCheck);
        if (rightChar == "0") {
            return true;
        } else {
            return false;
        }
    }
    public int GetLevel() {
        return GetSumOfNameNumbers() + GetCountOfNameNumbers() - 1;         
    }
    public int GetSumOfNameNumbers() {
        int sum = 0;
        for (int c = 0; c < go.name.Length; c++) {
            string c0 = go.name.Substring(c, 1);
            sum += int.Parse(c0);
        }
        return sum;
    }
    public int GetCountOfNameNumbers() {
        return go.name.Length;        
    }
    public int GetFibanocci(int level)
    {
        int answer = -1;
        if (level == 0)
        {
            answer = 0;
        }
        if (level == 1)
        {
            answer = 1;
        }
        if (level == 2)
        {
            answer = 2;
        }
        if (level == 3)
        {
            answer = 3;
        }
        if (level == 4)
        {
            answer = 5;
        }
        if (level == 5)
        {
            answer = 8;
        }
        if (level == 6)
        {
            answer = 13;
        }
        if (level == 7)
        {
            answer = 21;
        }
        return answer;
    }
    Text CreateText(Vector3 pos, string txt)
    {
        GameObject go0 = new GameObject("text");
        go0.name = txt;
        go0.transform.SetParent(GameObject.Find("Canvas").transform);
        go0.transform.Rotate(89, 0, 0);
        go0.transform.position = pos;
        go0.transform.localScale = new Vector3(.02f, .02f, .02f);
        Text text = go0.AddComponent<Text>();
        RectTransform rect = go0.GetComponent<RectTransform>();
        Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = font;
        text.name = "." + go0.name + ".";
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        text.text = txt;
        return text;
    }
}
