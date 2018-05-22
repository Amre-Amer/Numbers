using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : MonoBehaviour {
	int numX = 20; //0;
	int numY = 20; //0;
	int numZ = 20; //0;
	float[,,] cells;
	float[,,] cellsNext;
	GameObject[,,] cellGos;
	int cntFrames;
	public int fps;
	int fpsCount;
	float startTime;
	float delay = .25f;
	bool ynStep = true;
	bool ynGos = false;
	bool ynMesh = true;
	GameObject tmpGo;
	GameObject meshGo;
	Mesh mesh;
    Vector3[] vertices;
    Vector2[] uvs;
    int[] triangles;
	public int numPoints;
	GameObject targetGo;
	// Use this for initialization
	void Start () {
		targetGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		cells = new float[numX, numY, numZ];
		cellsNext = new float[numX, numY, numZ];
		if (ynGos == true)
		{
			cellGos = new GameObject[numX, numY, numZ];
		}
		if (ynMesh == true) {
			InitMesh();
		}
		InvokeRepeating("ShowFps", 1, 1);
		startTime = Time.realtimeSinceStartup;
	}
	void ShowFps() {
		fps = fpsCount;
		fpsCount = 0;
	}
	// Update is called once per frame
	void Update () {
		if (ynStep == true && Time.realtimeSinceStartup - startTime < delay) {
			return;
		}
		startTime = Time.realtimeSinceStartup;
		if (cntFrames % 5 == 0) {
			int x = Random.Range(0, numX - 1);
			int y = Random.Range(0, numY - 1);
			int z = Random.Range(0, numZ - 1);
			cellsNext[x, y, z] = 100;
			targetGo.transform.position = new Vector3(x, y, z);
//			Debug.Log(cells[0,0] + "\n");
		}
		SwapCells();
		UpdateCells();
		if (ynMesh == true) {
			UpdateMesh();
		}
		cntFrames++;
		fpsCount++;
	}
	void UpdateCells() {
		for (int x = 0; x < numX; x++) 
		{
			for (int y = 0; y < numY; y++)
			{
				for (int z = 0; z < numZ; z++)
				{
					ProcessCell(x, y, z);
				}
			}
		}
	}
	void SwapCells() {
		cells = cellsNext;
		cellsNext = new float[numX, numY, numZ];
	}
	void ProcessCell(int x, int y, int z) {
		if (ynGos == true)
		{
			if (cellGos[x, y, z] == null)
			{
				cellGos[x, y, z] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				cellGos[x, y, z].transform.position = new Vector3(x, y, z);
				cellGos[x, y, z].transform.localScale = new Vector3(.05f, .05f, .05f);
			}
		}
		float sum = 0;
		for (int xn = -1; xn <= 1; xn ++) 
		{
			for (int yn = -1; yn <= 1; yn++)
			{
				for (int zn = -1; zn <= 1; zn++)
				{
					if (xn != 0 || yn != 0 || zn != 0)
					{
						int x0 = x + xn;
						int y0 = y + yn;
						int z0 = z + zn;
						if (x0 >= 0 && x0 < numX && y0 >= 0 && y0 < numY && z0 >= 0 && z0 < numZ)
						{
							int cnt = 26;
							float val = cells[x0, y0, z0] / cnt;  // 8 in 2d  (3 x 3 - 1), (3x3x3 - 1) in 3d
							//Debug.Log("val:" + val + "\n");
							sum += val;
						}
					}
				}
			}
		}
		cellsNext[x, y, z] = sum;
		//		Debug.Log(x + "," + y + "," + z  + " = " + sum + "\n");
		if (ynGos == true)
		{
			cellGos[x, y, z].transform.localScale = new Vector3(sum * 3, sum * 3, sum * 3);
			if (sum > 0.05f)
			{
				cellGos[x, y, z].GetComponent<Renderer>().material.color = Color.green;
			}
			else
			{
				cellGos[x, y, z].GetComponent<Renderer>().material.color = Color.red;
			}
		}
	}
	void InitMesh()
    {
        tmpGo = new GameObject("tmpGo");
        meshGo = new GameObject("meshGo");

        vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(0, 0, 2), new Vector3(1, 0, 2), new Vector3(3, 0, 2) };
        uvs = new Vector2[] { new Vector2(0, 256), new Vector2(256, 256), new Vector2(256, 0), new Vector2(0, 256), new Vector2(256, 256), new Vector2(256, 0) };
        triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        mesh = new Mesh();
        meshGo.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer meshRenderer = meshGo.AddComponent<MeshRenderer>();
		Material mat = meshRenderer.material = new Material(Shader.Find("Cull Off/Diffuse"));
        mat.color = new Color(0, 1, 0, .5f);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        Debug.Log("mesh:vertices:" + mesh.vertices.Length + "\n");
    }
    void UpdateMesh()
    {
        if (meshGo == null)
        {
            InitMesh();
        }
        mesh.Clear();
        LoadMesh();
    }
    void LoadMesh()
    {
		numPoints = numX * numY * numZ;
        int numVertices = numPoints * 4;
        int numUvs = numVertices;
        int numTriangles = numPoints * 6;
        vertices = new Vector3[numVertices];
        uvs = new Vector2[numUvs];
        triangles = new int[numTriangles];
        for (int n = 0; n < numPoints; n++)
        {
			int x = n / (numY * numZ);
			int y = (n - x * (numY * numZ)) / numZ;
			int z = n % numY;
			Vector3 pos = new Vector3(x, y, z);
			float sum = cells[x, y, z];
			//sum = .5f;
			if (sum > .5f) sum = .5f;
			float s = 1;
			Vector3 sca = new Vector3(sum*s, sum*s, sum*s);
            tmpGo.transform.position = pos;
			tmpGo.transform.localScale = sca;
			tmpGo.transform.LookAt(targetGo.transform.position);
			tmpGo.transform.Rotate(-90, 0, 0);
            Vector3 p0 = tmpGo.transform.TransformPoint(new Vector3(-.5f, 0, -.5f));
            Vector3 p1 = tmpGo.transform.TransformPoint(new Vector3(-.5f, 0, .5f));
            Vector3 p2 = tmpGo.transform.TransformPoint(new Vector3(.5f, 0, .5f));
            Vector3 p3 = tmpGo.transform.TransformPoint(new Vector3(.5f, 0, -.5f));
            vertices[n * 4 + 0] = p0;
            vertices[n * 4 + 1] = p1;
            vertices[n * 4 + 2] = p2;
            vertices[n * 4 + 3] = p3;
            //
            triangles[n * 6 + 0] = n * 4 + 0;
            triangles[n * 6 + 1] = n * 4 + 1;
            triangles[n * 6 + 2] = n * 4 + 2;
            triangles[n * 6 + 3] = n * 4 + 0;
            triangles[n * 6 + 4] = n * 4 + 2;
            triangles[n * 6 + 5] = n * 4 + 3;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }
}
