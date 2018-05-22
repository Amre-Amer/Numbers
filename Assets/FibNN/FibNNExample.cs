using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class FibNNExample : MonoBehaviour
{
	GameObject meshGo;
    FibNNManager fibManager;
    float startTime;
    float delay = .1f;
    bool ynDelay = true;

    void Start()
    {
		meshGo = new GameObject("meshGo");
        startTime = Time.realtimeSinceStartup;
		int numLevels = 10; //11
        fibManager = new FibNNManager(numLevels);

		//float f = 6;
		//Debug.Log("Sigmoid:" + f + " = " + fibManager.Sigmoid(f) + "\n");

		//      FibMesh fibMesh = new FibMesh(vertices, uvs, triangles);

		//int numLevels = 3;
		//List<Vector3>[]levels = new List<Vector3>[numLevels];
		//levels[0] = new List<Vector3> { new Vector3(0, 0, 0) };
		//levels[1] = new List<Vector3> { new Vector3(0, 0, 1) , new Vector3(1, 0, 1) };
		//levels[2] = new List<Vector3> { new Vector3(0, 0, 2) , new Vector3(1, 0, 2) , new Vector3(3, 0, 2) };

		////Vector3[] vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1) };
  //      //Vector2[] uvs = new Vector2[] { new Vector2(0, 256), new Vector2(256, 256), new Vector2(256, 0) };
  //      //int[] triangles = new int[] { 0, 1, 2 };

		//Vector3[] vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1) , new Vector3(0, 0, 2), new Vector3(1, 0, 2), new Vector3(3, 0, 2) };
		//Vector2[] uvs = new Vector2[] { new Vector2(0, 256), new Vector2(256, 256), new Vector2(256, 0) , new Vector2(0, 256), new Vector2(256, 256), new Vector2(256, 0) };
  //      int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };

		//Mesh mesh = new Mesh();
		//meshGo.AddComponent<MeshFilter>().mesh = mesh;
		//MeshRenderer meshRenderer = meshGo.AddComponent<MeshRenderer>();
		//Material mat = meshRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));
		//mat.color = new Color(0, 1, 0, .5f);

		//mesh.vertices = vertices;
		//mesh.triangles = triangles;
		//mesh.uv = uvs;
		//mesh.RecalculateNormals();
		//Debug.Log("mesh:vertices:" + mesh.vertices.Length + "\n");
    }

    void Update()
    {
        if (ynDelay == true && Time.realtimeSinceStartup - startTime < delay)
        {
            return;
        }
        startTime = Time.realtimeSinceStartup;
        fibManager.Advance();
    }
}
