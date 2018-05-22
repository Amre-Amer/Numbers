using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sway : MonoBehaviour {
	GameObject meshGo;
	Mesh mesh;
	Vector3[] vertices;
	Vector2[] uvs;
	GameObject tmpGo;
	int[] triangles;
	int numX = 10;
 	int numY = 10;
	int numZ = 10;
	int cntFrames = 0;
	public List<SwayCell> swayCells;
	bool ynMesh = false;
	public int fps;
	int fpsCount;
	public bool ynOctTree = true;
	void Start () {
		InitSwayCells();
		//InvokeRepeating("SeedSwayCells", 1, 3);
		InvokeRepeating("ShowFps", 1, 1);
	}
	void ShowFps() {
		fps = fpsCount;
		fpsCount = 0;
	}
	void Update () {
		UpdateSwayCells();
//		SeedSwayCellsRandom();
		SeedSwayCellsCircle();
		if (ynMesh == true) {
			UpdateMesh();
		}
		cntFrames++;
		fpsCount++;
	}
	void UpdateSwayCells() {
		foreach(SwayCell swayCell in swayCells) {
            swayCell.Update();
        }       
	}
	void InitMesh() {
		tmpGo = new GameObject("tmpGo");
		meshGo = new GameObject("meshGo");

		vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1) , new Vector3(0, 0, 2), new Vector3(1, 0, 2), new Vector3(3, 0, 2) };
        uvs = new Vector2[] { new Vector2(0, 256), new Vector2(256, 256), new Vector2(256, 0) , new Vector2(0, 256), new Vector2(256, 256), new Vector2(256, 0) };
        triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        mesh = new Mesh();
        meshGo.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer meshRenderer = meshGo.AddComponent<MeshRenderer>();
        Material mat = meshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
        mat.color = new Color(0, 1, 0, .5f);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        Debug.Log("mesh:vertices:" + mesh.vertices.Length + "\n");
	}
	void UpdateMesh() {
		if (meshGo == null) {
			InitMesh();
		}
		mesh.Clear();
		LoadMesh();
	} 
	void LoadMesh() {
		int numPoints = swayCells.Count;
		int numVertices = numPoints * 4;
		int numUvs = numVertices;
		int numTriangles = numPoints * 6;
		vertices = new Vector3[numVertices];
		uvs = new Vector2[numUvs];
		triangles = new int[numTriangles];
		for (int n = 0; n < numPoints; n++) {
			Vector3 pos = swayCells[n].position;
			Vector3 rot = swayCells[n].rotation;
			tmpGo.transform.position = pos;
			tmpGo.transform.eulerAngles = rot;
			Vector3 p0 = tmpGo.transform.TransformPoint(new Vector3(-.5f, 0, -.5f)); 
			Vector3 p1 = tmpGo.transform.TransformPoint(new Vector3(-.5f, 0, .5f)); 
			Vector3 p2 = tmpGo.transform.TransformPoint(new Vector3(.5f, 0, .5f)); 
			Vector3 p3 = tmpGo.transform.TransformPoint(new Vector3(.5f, 0, -.5f));
			vertices[n*4 + 0] = p0;
			vertices[n*4 + 1] = p1;
			vertices[n*4 + 2] = p2;
			vertices[n*4 + 3] = p3;
			//
			triangles[n*6 + 0] = n*4 + 0;
			triangles[n*6 + 1] = n*4 + 1;
			triangles[n*6 + 2] = n*4 + 2;
			triangles[n*6 + 3] = n*4 + 0;
			triangles[n*6 + 4] = n*4 + 2;
			triangles[n*6 + 5] = n*4 + 3;
		}
		mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
	}
	void InitSwayCells() {
		swayCells = new List<SwayCell>();
        for (int x = 0; x < numX; x++)
        {
			for (int y = 0; y < numY; y++)
			{
				for (int z = 0; z < numZ; z++)
				{
					SwayCell swayCell = new SwayCell(new Vector3(x, y, z), swayCells, !ynMesh, ynOctTree);
				}
			}
        }
	}
	void SeedSwayCellsCircle() {
		float rad = numX / 3;
		float x = numX / 2 + rad * Mathf.Cos(cntFrames * Mathf.Deg2Rad);
		float y = numY / 2;
		float z = numZ / 2 + rad * Mathf.Sin(cntFrames * Mathf.Deg2Rad);
		float n = x * (numY + numZ) + y * numX + z;
		n = swayCells.Count / 2;
		swayCells[(int)n].AddHistory(new Vector3(45, 45, 0));
	}
	void SeedSwayCellsRandom() {
		float pitch = 30 * Mathf.Cos(3 * cntFrames * Mathf.Deg2Rad);
		int n = Random.Range(0, swayCells.Count - 1);
		//n = 0;
		//		pitch = 45;
		float yaw = 30 * Mathf.Sin(2 * cntFrames * Mathf.Deg2Rad);
		swayCells[n].AddHistory(new Vector3(pitch, yaw, 0));
	}
}

public class SwayCell {
	List<SwayCell> swayCells;
	public List<Vector3> history;
	public int index;
	public Vector3 position;
	public Vector3 rotation;
	public bool ynGo = true;
	public GameObject go;
	public bool ynOctTree = false;
	public SwayCell(Vector3 position0, List<SwayCell>swayCells0, bool ynGo0, bool ynOctTree0) {
		position = position0; 
		swayCells = swayCells0;
		ynGo = ynGo0;
		ynOctTree = ynOctTree0;
		swayCells.Add(this);
		index = swayCells.Count - 1;
		history = new List<Vector3>();
		if (ynGo == true) {
			go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.position = position0;
			go.transform.localScale = new Vector3(.9f, .1f, .9f);
		}
	}
	public void Update() {
		if (ynOctTree == true) {
			ProcessOctTree();			
		} else {
			Process();
		}
		ProcessOctTree();
		AdvanceHistory();
	}
	public void ProcessOctTree() {
		LoadOctTree();	
		Vector3 vectorSum = Vector3.zero;
        for (int n = 0; n < swayCells.Count; n++)
        {
			float dist = Vector3.Distance(swayCells[n].position, position);
			//if (dist < 2)
			//{
				int h = GetHistoryIndexByDistance(n);
				if (h < swayCells[n].history.Count)
				{
					Vector3 vector = swayCells[n].history[h] / (h + 1);
					vectorSum += vector;
				}
//			}
        }
        rotation += vectorSum;
        if (ynGo == true)
        {
            go.transform.eulerAngles = rotation;
        }
	}
	public void LoadOctTree() {
		
	}
	public void Process() {
		Vector3 vectorSum = Vector3.zero;
		for (int n = 0; n < swayCells.Count; n++)
        {
			int h = GetHistoryIndexByDistance(n);
			if (h < swayCells[n].history.Count)
			{
				Vector3 vector = swayCells[n].history[h] / (h + 1);
				vectorSum += vector;
			}
        }
		rotation += vectorSum;
		if (ynGo == true) {
			go.transform.eulerAngles = rotation;
		}
	}
	public int GetHistoryIndexByDistance(int n) {
		float dist = Vector3.Distance(swayCells[n].position, position);
        int h = (int)Mathf.Round(dist);
		return h;
	}
	public void AdvanceHistory() {
		history.Insert(0, Vector3.zero);
	}
	public void AddHistory(Vector3 rot) {
		history[0] = rot;
	}
}
	
