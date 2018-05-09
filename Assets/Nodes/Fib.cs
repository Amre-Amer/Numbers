using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fib : MonoBehaviour {
    GlobalClassTiny global;

	// Use this for initialization
	void Start () {
        global = new GlobalClassTiny();
        global.levelMax = 4;
        global.ynStep = true;
        global.ynStepEach = true;
        global.delay = .5f;
        global.scaleHistory = .75f;
        global.lengthLinksForward = 2;
        global.lengthLinksLeft = -2;
        global.lengthLinksRight = 2;
        global.lengthLinksUp = 2;
        global.lengthLinksDown = -2;
        global.parentLinks = new GameObject("parentLinks");
        global.nodes = new List<NodeClass>();
        global.audioSource1 = gameObject.AddComponent<AudioSource>();
        global.audioSource1.clip = Resources.Load("sound1") as AudioClip;
        global.audioSource2 = gameObject.AddComponent<AudioSource>();
        global.audioSource2.clip = Resources.Load("sound2") as AudioClip;
        global.ynAudio = false;
        global.timeStart = Time.realtimeSinceStartup;
        Debug.Log("Fib\n");
        Show();
        //
        Add("0");
//        Manual();
	}

    void Radius() {
        
    }

    void Manual() {
        Find("0").AdvanceLevel();
        Find("1").GiveBirth();
        Find("1").AdvanceLevel();
        Find("10").AdvanceLevel();
        Find("11").AdvanceLevel();
        Find("12").GiveBirth();
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
        if (global.ynStep == true && Time.realtimeSinceStartup - global.timeStart < global.delay) {
            return;
        }
        if (global.level >= global.levelMax && global.index >= global.nodes.Count)
        {
            return;
        }
        global.timeStart = Time.realtimeSinceStartup;
        if (global.ynStepEach == true) {
            UpdateNextNode();
        } else {
            UpdateNodes();
        }
	}

    void ResetLevel() {
        global.index = 0;
        global.level++;
        Debug.Log("Level:" + global.level + "................(nodes:" + global.nodes.Count + " gos:" + global.cntGameObjects + ")\n");
    }

    void UpdateNodes() {
        ResetLevel();
        for (int n = 0; n < global.nodes.Count; n++) {
            NodeClass node = global.nodes[n];
            node.AdvanceLevel();
        }
        Show();
    }

    void UpdateNextNode() {
        if (global.index >= global.nodes.Count || global.index == 0)
        {
            ResetLevel();
        }
        NodeClass node = global.nodes[global.index];
        node.AdvanceLevel();
        global.index++;
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
    public float lengthLinksForward;
    public float lengthLinksLeft;
    public float lengthLinksRight;
    public float lengthLinksUp;
    public float lengthLinksDown;
    public float scaleHistory;
    public GameObject parentLinks;
    public int lastLinkInRow;
    public int index;
    public bool ynStepEach;
    public int cntGameObjects;
    public AudioSource audioSource1;
    public AudioSource audioSource2;
    public bool ynAudio;
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
        global.cntGameObjects++;
        goResult.name = txt;
        goResult.transform.localScale = new Vector3(1f, .1f, 1f);
        AdjustColorByName(goResult);
        goResult.name = txt;
        return goResult;
    }
    public void AddToNodes(NodeClass node)
    {
        global.nodes.Add(node);
        node.index = global.nodes.Count - 1;
    }
    public void AdvanceLevel()
    {
        if (ynJustBorn == true) {
            ynJustBorn = false;
            return;
        }
        Vector3 pos = go.transform.position;
        LeaveCopy();
        if (IsAdult(go)) {
            GiveBirth();
        } else {
            if (global.ynAudio) global.audioSource2.Play();
        }
        AddOneToName();
        MoveForward();
        //MoveUp();
        //MoveLeft();
        AddLinkFrom(pos);
    }
    public void GiveBirth() {
        string nameNew = AddZeroToName(go.name);
        NodeClass node = new NodeClass(nameNew, global);
        node.ynJustBorn = true;
        node.MatchPosition(go);
        node.MoveForward();
        node.MoveRight();
        //node.MoveDown();
        node.AddLinkFrom(go.transform.position);
        if (global.ynAudio) global.audioSource2.Play();
        if (global.ynAudio) global.audioSource1.Play();
    }
    public void AddLinkFrom(Vector3 posFrom)
    {
        Vector3 posTo = go.transform.position;
        GameObject go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        global.cntGameObjects++;
        go0.transform.parent = global.parentLinks.transform;
        go0.name = "link";
        float sca = Vector3.Distance(posFrom, posTo);
        go0.transform.position = (posFrom + posTo) / 2;
        go0.transform.LookAt(posTo);
        go0.transform.localScale = new Vector3(.1f, .1f, sca);
        go0.GetComponent<Renderer>().material.color = Color.grey;
        //if (index == 0) go0.GetComponent<Renderer>().material.color = Color.red;
        //if (index == 1) go0.GetComponent<Renderer>().material.color = Color.green;
        //if (index == 2) go0.GetComponent<Renderer>().material.color = Color.blue;
        //if (index == 3) go0.GetComponent<Renderer>().material.color = Color.yellow;
        //if (index == 4) go0.GetComponent<Renderer>().material.color = Color.magenta;
        //if (index == 5) go0.GetComponent<Renderer>().material.color = Color.grey;
//        global.lastLinkInRow++;
    }
    //public void MoveForwardAutoRight()
    //{
    //    Debug.Log(go.name + " " + global.lastLinkInRow + "\n");
    //    MoveForward();
    //    for (int n = 0; n < global.lastLinkInRow; n++)
    //    {
    //        MoveRight();
    //    }
    //    global.lastLinkInRow++;
    //}
    public void MoveForward()
    {
        go.transform.position += Vector3.forward * global.lengthLinksForward;
        textGo.transform.position += Vector3.forward * global.lengthLinksForward;
    }
    public void MoveLeft()
    {
        float dist = global.lengthLinksLeft; // * (global.levelMax - global.level);
        go.transform.position += Vector3.right * dist;
        textGo.transform.position += Vector3.right * dist;
    }
    public void MoveRight()
    {
        float dist = global.lengthLinksRight; // * (global.levelMax - global.level);
        go.transform.position += Vector3.right * dist;
        textGo.transform.position += Vector3.right * dist;
    }
    public void MoveLeftHalf()
    {
        float dist = global.lengthLinksLeft / 2; // * (global.levelMax - global.level);
        go.transform.position += Vector3.right * dist;
        textGo.transform.position += Vector3.right * dist;
    }
    public void MoveRightHalf()
    {
        float dist = global.lengthLinksRight / 2; // * (global.levelMax - global.level);
        go.transform.position += Vector3.right * dist;
        textGo.transform.position += Vector3.right * dist;
    }
    public void MoveDown()
    {
        go.transform.position += Vector3.up * global.lengthLinksDown;
        textGo.transform.position += Vector3.up * global.lengthLinksDown;
    }
    public void MoveUp()
    {
        go.transform.position += Vector3.up * global.lengthLinksUp;
        textGo.transform.position += Vector3.up * global.lengthLinksUp;
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
        if (IsAdult(go0))
        {
            col = Color.white;
        }
        else
        {
            col = Color.blue;
        }
        textGo0.color = col;
    }
    public Vector3 getTextPos(GameObject go0) {
         return go0.transform.position + Vector3.up * .15f;
    }
    public void AdjustColorByName(GameObject go0) {
        Color col = Color.black;
        if (IsAdult(go0)) {
            col = Color.blue;            
        } else {
            col = Color.white;            
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
    public bool IsAdult(GameObject go0) {
        string nameCheck = go0.name;
        string rightChar = GetRightChar(nameCheck);
        if (rightChar == "0") {
            return false;
        } else {
            return true;
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
        global.cntGameObjects++;
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
