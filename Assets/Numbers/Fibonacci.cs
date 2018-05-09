using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fibonacci : MonoBehaviour {
    public int numLives = 2; // 7;
    public bool ynStep = true;
    public bool ynLeaveTrail;
    float startTime;
    public float delay = 1;
    public int life;
    public GlobalClass global;
    int maxCells = 100;
    CellClass cellFirst;

    // Use this for initialization
	void Start () {
//        Fib();
        Test();
        //TestClass test = new TestClass();
        //test.Birth();
	}

    void Test() {
              
    }
	
	// Update is called once per frame
	void Update () {
        if (ynStep == true && Time.realtimeSinceStartup - startTime < delay) {
            return;
        }
        if (life >= numLives) {
            return;
        }  
        startTime = Time.realtimeSinceStartup;
        LoadGlobal();
        UpdateCycle();
        //Show();
        life++;
	}

    void LoadGlobal() {
        global.ynLeaveTrail = ynLeaveTrail;
    }

    public void Fib() {
        global = new GlobalClass();
        global.cells = new CellClass[maxCells];
        global.sRight = -2;
        global.sUp = -1;
        global.sForward = 2;
        global.parentCircles = new GameObject("parentCircles");
        global.parentLinks = new GameObject("parentLinks");
        global.colorBlue = new Color(0, 0, 1, .875f);
        global.colorWhite = new Color(.95f, .95f, 1, .875f);
        global.colorRed = new Color(.95f, 0, 0, .875f);
        cellFirst = new CellClass(null, global);
    } 
    public void UpdateCycle() {
        AddCircle(Vector3.zero, (life + 1) * global.sForward);
        //
        for (int c = 0; c < global.lastCell; c++) {
            global.cells[c].Live();
        }
    }
    public void Show()
    {
        for (int l = 0; l < numLives; l++)
        {
            Debug.Log("life cycle:" + l + ".........................\n");
            for (int c = 0; c < global.lastCell; c++)
            {
                global.cells[c].Show();
            }
        }
    }
    public void AddCircle(Vector3 center, float radius) {
//        float sca = (radius) * 2 + cells[0].go.transform.localScale.x;
        float sca = (radius) * 2 + 1;
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.transform.parent = global.parentCircles.transform;
        go.transform.position = center + Vector3.up * -.25f * (life + 1);
        go.name = "circle " + (life + 1);
        go.transform.localScale = new Vector3(sca, .1f, sca);
        go.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture2D>("circle");
        MakeMaterialTransparent(go.GetComponent<Renderer>().material);
        go.GetComponent<Renderer>().material.color = new Color(1, 1, 1, .5f);
    }
    void MakeMaterialTransparent(Material material)
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

public class TestClass {
    public TestClass() {
        Debug.Log("x\n");
    }
    public void Birth() {
        TestClass child = new TestClass();
    }
}

public class CellClass {
    public int c;
    public CellClass goParent;
    public int age;
    public GameObject go;
    public GlobalClass global;
    public CellClass(CellClass goParent0, GlobalClass global0) {
        goParent = goParent0;
        global = global0;
        age = 0;
        go = CreateCellGo();
        c = global.lastCell;
        global.cells[c] = this;
        global.cells[c].go.name = getName();
        //
        global.lastCell++;
    }
    string getName() {
        string txt = "first";
        if (goParent != null) {
            txt = goParent.go.name;
        }
        return c + " cell age:" + age + " (" + txt + ")";
    }
    public void Live() {
        LeaveCopy();
        age++;
        UpdateColorByAge();
        UpdateNameWithAge();
//        LeaveCopy();
        GiveBirthIfMature();
        MoveForward();
    }
    public void GiveBirthIfMature()
    {
        if (age < 2)
        {
            return;
        }
        Debug.Log("birth:" + go.name + "\n");
        //return;
        Vector3 posLast = go.transform.position;
        CellClass goChild = new CellClass(this, global);
        goChild.go.transform.position = go.transform.position;
        goChild.go.transform.eulerAngles = go.transform.eulerAngles;
        goChild.go.transform.position += goChild.go.transform.forward * global.sForward;
        goChild.go.transform.position += goChild.go.transform.right * global.sRight;
        goChild.go.GetComponent<Renderer>().material.color = global.colorWhite;
        CreateLink(posLast, goChild.go.transform.position);
    }
    void UpdateNameWithAge() {
        go.name = getName();
    }
    public void UpdateColorByAge() {
        if (age == 1)
        {
            go.GetComponent<Renderer>().material.color = global.colorWhite;
        } else {
            go.GetComponent<Renderer>().material.color = global.colorBlue;
        }
    }
    public void LeaveCopy()
    {
        if (global.ynLeaveTrail == false)
        {
            return;
        }
        //if (age <= 1) {
        //    return;
        //}
        GameObject goCopy = CreateCellGo();
        goCopy.transform.position = go.transform.position;
        goCopy.transform.eulerAngles = go.transform.eulerAngles;
        goCopy.GetComponent<Renderer>().material.color = go.GetComponent<Renderer>().material.color;
        goCopy.name = "copy " + getName();
    }
    public void MoveForward() {
        if (age <= 1)
        {
            return;
        }
        Vector3 posLast = go.transform.position;
        go.transform.position += go.transform.forward * global.sForward;
        Debug.Log("Move:" + go.name + "\n");
        CreateLink(posLast, go.transform.position);
    }
    void CreateLink(Vector3 posFrom, Vector3 posTo) {
        GameObject go0 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go0.transform.parent = global.parentLinks.transform;
        float dist = Vector3.Distance(posFrom, posTo);
        go0.transform.position = (posFrom + posTo) / 2;
        go0.transform.LookAt(posTo);
        go0.transform.localScale = new Vector3(.1f, .1f, dist);
    }
    void MakeMaterialTransparent(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
    GameObject CreateCellGo() {
        GameObject go0 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go0.name = "?";
        go0.transform.localScale = new Vector3(1, .125f, 1);
        MakeMaterialTransparent(go0.GetComponent<Renderer>().material);
        go0.GetComponent<Renderer>().material.color = global.colorRed;
        return go0;
    }
    public void Show() {
        Debug.Log("cell:" + c + " " + go.name + "\n");        
    }
}
public class GlobalClass {
    public CellClass[] cells;
    public int lastCell;
    public bool ynLeaveTrail;
    public float sRight;
    public float sUp;
    public float sForward;
    public GameObject parentCircles;
    public GameObject parentLinks;
    public Color colorWhite;
    public Color colorBlue;
    public Color colorRed;
}

