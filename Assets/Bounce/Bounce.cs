using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour {
    GlobalBallClass global;

	// Use this for initialization
	void Start () {
        global = new GlobalBallClass();
        global.balls = new List<BallClass>();
        global.numLevels = 2; //4; //12;
        global.radiusStudio = 10;
        global.smooth = .95f;
        global.ynSmooth = true;
        global.ynStep = true;
        global.delay = 1;
        global.parent = new GameObject("parent");
        global.ynLeaveTrail = false;
        //
        BallClass ball = new BallClass(-1, global, "embryo");
        global.startTime = Time.realtimeSinceStartup;
	}
	
	// Update is called once per frame
	void Update () {
        if (global.ynStep == true && Time.realtimeSinceStartup - global.startTime < global.delay) {
//            return;
        } else {
            global.startTime = Time.realtimeSinceStartup;
            UpdateBalls();
            global.level++;
        }
        Smooth();
	}
    void UpdateBalls() {
        if (global.level < global.numLevels)
        {
            Debug.Log("Level:" + global.level + " ...................................................................(" + global.balls.Count + ")\n");
            for (int b = 0; b < global.balls.Count; b++)
            {
                BallClass ball = global.balls[b];
                ball.Advance();
            }
        }
        ShowSummary();
    }
    void ShowSummary() {
        if (global.level == global.numLevels)
        {
            Debug.Log("numLevels:" + global.numLevels + " balls:" + global.balls.Count + " copies:" + global.cntCopies + "\n");
        }
    }
    void Smooth() {
        for (int b = 0; b < global.balls.Count; b++)
        {
            BallClass ball = global.balls[b];
            //if (ball.cycle > 0)
            //{
                ball.MoveToTarget();
            //}
        }
    }
}

public class BallClass {
    public bool ynActive;
    public Vector3 posTarget;
    public Vector3 rotTarget;
    public Vector3 scaTarget;
    public int indexFrom;
    public GameObject go;
    public GameObject goLink;
    public GameObject goLinkFrom;
    public int index;
    public GlobalBallClass global;
    public int cycle;
	public string mode;
	public BallClass(int indexFrom0, GlobalBallClass global0, string mode0) {
		mode = mode0;
		cycle = -1;
        ynActive = true;
        indexFrom = indexFrom0;
        global = global0;
        go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        SetPosition();
        UpdateLink();
        AddToBalls(this);
        SetColor();
        global.cntCopies++;
    }
    public void Advance() {
        UpdateName();
		SetColor();
        if (cycle > 0)
        {
			ContinueLiving();
        }
		if (cycle == 0)
        {
			Embryo();
        }
        if (cycle == 1)
        {
			GiveBirth();
        }
        cycle++;
    }
	public void ContinueLiving() {
		BallClass ball = new BallClass(index, global, "continue");
	}
	public void Embryo() {
		BallClass ball = new BallClass(index, global, "embryo");
	}
	public void GiveBirth() {
		BallClass ball = new BallClass(index, global, "birth");
	}
    public void SetTarget()
    {
        Debug.Log("SetTarget:ball:" + index + " cycle:" + cycle + "\n");
        float angDelta = global.level * 90 / global.numLevels;
        float dist = dist = 1 + (1f - (float)global.level / (float)global.numLevels) * 3;
        angDelta = 0;
        posTarget = global.balls[indexFrom].go.transform.position;
        rotTarget = global.balls[indexFrom].go.transform.eulerAngles;
        rotTarget += new Vector3(0, 0, angDelta);
        posTarget += global.balls[indexFrom].go.transform.forward * dist;
        //
        string nam = "null";
        if (indexFrom != -1) nam = global.balls[indexFrom].go.name + " = " + global.balls[indexFrom].go.transform.position;
        //
        Debug.Log("SetTarget:ball:" + index + " cycle:" + cycle + " goFrom:" + nam + " dist:" + dist + " = " + posTarget + "\n");
    }
    public void UpdateName() {
        go.name = "ball:" + index + " cycle:" + cycle + " level:" + global.level;
    }
    public void SetPosition() {
        if (global.balls[indexFrom].go == null)
        {
            go.transform.position = Vector3.zero;
            go.transform.eulerAngles = new Vector3(-90, 0, 0);
            go.transform.localScale = new Vector3(1, 1, 1);
        } else {
            go.transform.position = global.balls[indexFrom].go.transform.position;
            go.transform.eulerAngles = global.balls[indexFrom].go.transform.eulerAngles;
            go.transform.localScale = global.balls[indexFrom].go.transform.localScale;
            //posTarget = goFrom.transform.position;
            //rotTarget = goFrom.transform.eulerAngles;
            //scaTarget = goFrom.transform.localScale;
        }
        Debug.Log("SetPosition:ball:" + index + " cycle:" + cycle + " = " + go.transform.position + "\n");
    }
    public void MoveToTarget()
    {
        if (global.ynSmooth == true) {
            go.transform.position = global.smooth * go.transform.position + (1f - global.smooth) * posTarget;
        } else {
            go.transform.position = posTarget;
        }
        UpdateLink();
        //Debug.Log(".\n");        
    }
    //public void LeaveCopy()
    //{
    //    goFrom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    goFrom.transform.parent = global.parent.transform;
    //    goFrom.name = "copy go " + go.name;
    //    goFrom.transform.position = go.transform.position;
    //    if (global.ynLeaveTrail == false) {
    //        goFrom.transform.localScale = new Vector3(.1f, .1f, .1f);
    //    } else {
    //        goFrom.transform.localScale = go.transform.localScale;
    //    }
    //    goFrom.transform.eulerAngles = go.transform.eulerAngles;
    //    goFrom.GetComponent<Renderer>().material.color = go.GetComponent<Renderer>().material.color;
    //    global.cntCopies++;
    //    if (global.ynLeaveTrail == true)
    //    {
    //        if (goLink != null)
    //        {
    //            goLinkFrom = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //            goLinkFrom.transform.parent = global.parent.transform;
    //            goLinkFrom.name = "copy link " + goLink.name;
    //            goLinkFrom.transform.position = goLink.transform.position;
    //            goLinkFrom.transform.localScale = goLink.transform.localScale;
    //            goLinkFrom.transform.eulerAngles = goLink.transform.eulerAngles;
    //        }
    //    }
    //}
    public void UpdateLink() {
        if (global.ynLeaveTrail == false) {
            return;
        }
		if (indexFrom == -1) {
            return;
        }
        if (goLink == null) {
            goLink = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goLink.transform.parent = global.parent.transform;
        }
		Vector3 posFrom = global.balls[indexFrom].go.transform.position;
        Vector3 posTo = go.transform.position;
        float dist = Vector3.Distance(posFrom, posTo);
        goLink.transform.position = (posFrom + posTo) / 2;
        goLink.transform.LookAt(posTo);
        goLink.transform.localScale = new Vector3(.125f, .5f, dist);
    }
    public void SetColor() {
        Color col = Color.gray;
        //if (global.cycle == 0) col = Color.red;
        //if (global.cycle == 1) col = Color.green;
        //if (global.cycle == 2) col = Color.blue;
        if (cycle == 0) col = Color.red;
        if (cycle == 1) col = Color.white;
        if (cycle >= 2) col = Color.blue;
        go.GetComponent<Renderer>().material.color = col;
    }
    public void AddToBalls(BallClass ball) {
        global.balls.Add(ball);
        ball.index = global.balls.Count - 1;
    }
}

public class GlobalBallClass {
    public List<BallClass> balls;
    public int level;
    public int numLevels;
    public float radiusStudio;
    public int cntCopies;
    public float smooth;
    public bool ynSmooth;
    public bool ynStep;
    public float delay;
    public float startTime;
    public GameObject parent;
    public bool ynLeaveTrail;
}