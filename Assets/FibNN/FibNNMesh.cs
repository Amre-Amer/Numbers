using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FibMesh
{
	public Mesh mesh;
	public FibMesh (Vector3[] vertices, Vector2[] uvs, int[] triangles) {
		if (mesh == null) {
			mesh = new Mesh();
            mesh.name = "fibMesh";
		} else {
			mesh.Clear();			
		}
		mesh.vertices = vertices;
		mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
	}
}
public class FibNNMesh {
	GameObject[,] pointGos;
	bool ynShowPoints;
	float speed = 1;
	float cycle = 0;
	float cycle2 = 0;
	int cntFrames;
	float amplitude = 1;
	int numLengthDivs = 10;
	int numRadDivs = 10;
	public Mesh sharedMesh;
	public Vector3[] points;
	public Mesh CreateMeshFromPoints()
    {
        int numFacesPerQuad = 2;
        int numVerticesPerQuad = 4;
        int numQuads = numLengthDivs * numRadDivs;
        int numTrianglesPerQuad = numFacesPerQuad * 3;
        if (sharedMesh == null)
        {
            sharedMesh = new Mesh();
            sharedMesh.name = "Mesh";
        }
        else
        {
            sharedMesh.Clear();
        }
        Mesh m = sharedMesh;
        Vector3[] vertices = new Vector3[numVerticesPerQuad * numQuads];
        Vector2[] uvs = new Vector2[numVerticesPerQuad * numQuads];
        int[] triangles = new int[numTrianglesPerQuad * numQuads];
        int q = 0;
        for (int d = 0; d < numLengthDivs; d++)
        {
            for (int r = 0; r < numRadDivs; r++)
            {
                int n0 = getDR2N(r, d);
                int n1 = getDR2N(r + 1, d);
                int n2 = getDR2N(r + 1, d + 1);
                int n3 = getDR2N(r, d + 1);
                Vector3 pnt0 = points[n0];
                Vector3 pnt1 = points[n1];
                Vector3 pnt2 = points[n2];
                Vector3 pnt3 = points[n3];
                //
                vertices[q * numVerticesPerQuad + 0] = pnt0;
                vertices[q * numVerticesPerQuad + 1] = pnt1;
                vertices[q * numVerticesPerQuad + 2] = pnt2;
                vertices[q * numVerticesPerQuad + 3] = pnt3;
                //
                uvs[q * numVerticesPerQuad + 0] = new Vector2(0, 0);
                uvs[q * numVerticesPerQuad + 1] = new Vector2(0, 1);
                uvs[q * numVerticesPerQuad + 2] = new Vector2(1, 1);
                uvs[q * numVerticesPerQuad + 3] = new Vector2(1, 0);
                //
                triangles[q * numTrianglesPerQuad + 0] = q * numVerticesPerQuad + 0;
                triangles[q * numTrianglesPerQuad + 1] = q * numVerticesPerQuad + 1;
                triangles[q * numTrianglesPerQuad + 2] = q * numVerticesPerQuad + 2;
                triangles[q * numTrianglesPerQuad + 3] = q * numVerticesPerQuad + 0;
                triangles[q * numTrianglesPerQuad + 4] = q * numVerticesPerQuad + 2;
                triangles[q * numTrianglesPerQuad + 5] = q * numVerticesPerQuad + 3;
                q++;
            }
        }
        m.vertices = vertices;
        m.uv = uvs;
        m.triangles = triangles;
        m.RecalculateNormals();
        return m;
    }
	int getDR2N(int r, int d)
    {
        return d * (numRadDivs + 1) + r;
    }
	public void addSharedMat(GameObject go)
    {
		Material matShared;
        matShared = (Material)Resources.Load("Rocks", typeof(Material));
        MeshRenderer renderer = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.sharedMaterial = matShared;
    }

    public MeshFilter SetMeshFilter(GameObject go)
    {
        MeshFilter mf = (MeshFilter)go.GetComponent(typeof(MeshFilter));
        if (mf == null)
        {
            mf = (MeshFilter)go.AddComponent(typeof(MeshFilter));
        }
        return mf;
    }

    public void AddMeshMaterial(GameObject go, string matName)
    {
        Material mat = (Material)Resources.Load(matName, typeof(Material));
        MeshRenderer mRend = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
        if (mRend == null)
        {
            mRend = go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        }
        mRend.sharedMaterial = mat;
    }
	public void updateMesh()
    {
        updateMeshPoints();
		//
		MeshFilter meshFilter = null;
		GameObject gameObject = null;
		Material sharedMat = null;
        if (meshFilter == null)
        {
            meshFilter = SetMeshFilter(gameObject);
        }
        if (sharedMat == null)
        {
            AddMeshMaterial(gameObject, "Rocks");
        }
        sharedMesh = CreateMeshFromPoints();
        meshFilter.sharedMesh = sharedMesh;
    }
	void updateMeshPoints()
    {
		GameObject goTmp = null;
		int numPoints = 10;
		int numPointsLast = 10;
		int cntVertices = 10;
		float distStartEnd = 1;
		Transform transform = null;
		Vector3 endPoint = Vector3.zero;
		Color colMesh = Color.red;

        numPoints = (numRadDivs + 1) * (numLengthDivs + 1);
        cntVertices += numPoints;
        if (points == null || numPoints != numPointsLast)
        {
            points = new Vector3[numPoints];
        }
        numPointsLast = numPoints;
        //setStartEndPoints();
        float angIncr = 360 / numRadDivs;
        float distIncr = distStartEnd / numLengthDivs;
        int n = 0;
        //      setRadStartEnd ();
        float radStart = 1;
        float radEnd = 1;
        float radIncr = (radEnd - radStart) / numLengthDivs;
        //
        float spacing = 1f;
        //      float distMax = Mathf.Sqrt(fb.numLengthDivs * spacing * fb.numLengthDivs * spacing + fb.numRadDivs * spacing * fb.numRadDivs * spacing);
        //
        for (int d = 0; d <= numLengthDivs; d++)
        {
            float radThis = radStart + d * radIncr;
            radThis += Mathf.Cos(d * cycle * Mathf.Deg2Rad);
            radThis = 1f;
            //
            goTmp.transform.eulerAngles = transform.eulerAngles;
            goTmp.transform.position = endPoint + goTmp.transform.forward * d * distIncr;
            Color col = colMesh;
            for (int r = 0; r <= numRadDivs; r++)
            {
                float ang = r * angIncr;
                float x = radThis * Mathf.Cos(ang * Mathf.Deg2Rad);
                float y = radThis * Mathf.Sin(ang * Mathf.Deg2Rad);
                float z = 0;
                //
                x = -r * spacing;
                z = d * spacing;
                y = getY(x, z);
                //
                Vector3 pos = goTmp.transform.position;
                pos += goTmp.transform.right * x;
                pos += goTmp.transform.up * y;
                pos += goTmp.transform.forward * z;
                if (ynShowPoints == true)
                {
                    if (pointGos == null)
                    {
                        pointGos = new GameObject[numRadDivs + 1, numLengthDivs + 1];
                    }
                    GameObject go = pointGos[r, d];
                    if (go == null)
                    {
                        go = addPoint(pos);
                        setColor(go, col);
                        pointGos[r, d] = go;
                    }
                    go.transform.position = pos;
                }
                points[n] = transform.InverseTransformPoint(pos);
                n++;
            }
        }
    }
	GameObject addPoint(Vector3 pos)
    {
		GameObject marker = null;
        float sca = .05f;
        Color col = Color.yellow;
		GameObject go = null; //Instantiate(marker);
        //go.transform.parent = pointsParent.transform;
        go.transform.position = pos;
        go.transform.localScale = new Vector3(sca, sca, sca);
        //setColor(go, col);
        //cntPointGos++;
        return go;
    }

	float getY(float x, float z)
    {
        float y;
        float dist1 = Mathf.Sqrt(x * x + z * z);
        float dist2 = Mathf.Sqrt((x - 1) * (x - 1) + (z - 1) * (z - 1));
        float y1 = amplitude * Mathf.Cos((speed * 10 * cntFrames + dist1 * cycle * 5) * Mathf.Deg2Rad);
        float y2 = amplitude * Mathf.Sin((speed * 10 * cntFrames + dist2 * (cycle2 + 20) * 5) * Mathf.Deg2Rad);
        y = (y1 + y2) / 2;
        return y;
    }
	void setColor(GameObject go, Color col)
    {
        Renderer rend = go.GetComponent<Renderer>();
        rend.material.color = col;
    }
}
