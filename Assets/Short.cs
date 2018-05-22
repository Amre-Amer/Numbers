using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Short : MonoBehaviour {
	int resX = 4; //6;
	int resZ = 4; //6;
	int numLevels = 4; //6;
	List<ShortNode> shortNodes;
	int cntFrames;
	void Start () {
		Generate();
	}
	void Generate() {
		shortNodes = new List<ShortNode>();
		for (int l = 0; l < numLevels; l++)
		{
			for (int x = 0; x < resX - l; x++)
			{
				for (int z = 0; z < resZ - l; z++)
				{
					float xOffset = l / 2f;
					float zOffset = l / 2f;
					float xf = xOffset + x;
					float yf = l;
					float zf = zOffset + z;
					ShortNode shortNode = new ShortNode(xf, yf, zf, shortNodes);
				}
			}
		}
//		AddFourToNodes();
		AddOthersToNodes();
	}
	void AddOthersToNodes() {
		for (int n = 0; n < shortNodes.Count; n++)
        {
			ShortNode shortNode = shortNodes[n];
			for (int n0 = 0; n0 < shortNodes.Count; n0++)
			{
				if (n != n0) {
					ShortNode shortNode0 = shortNodes[n0];
                    Vector3 pos = shortNode0.position;
                    shortNode.AddToToNodes(pos);
				}
			}
        }
	}
	void AddFourToNodes() {
		for (int n = 0; n < shortNodes.Count; n++)
        {
            ShortNode shortNode = shortNodes[n];
            if (shortNode.position.y > 0)
            {
                for (float x = -.5f; x <= .5f; x += 1)
                {
                    for (float z = -.5f; z <= .5f; z += 1)
                    {
                        Vector3 pos = shortNode.position + new Vector3(x, -1, z);
                        shortNode.AddToToNodes(pos);
                    }
                }
            }
        }
	}
	void Update () {
		for (int n = 0; n < shortNodes.Count; n++)
		{
			ShortNode shortNode = shortNodes[n];
//			shortNode.EvenDistanceToToNodes();
//			shortNode.SpreadOutSphere();
			shortNode.TypicalAvoid();
			shortNode.RandomTarget();
			shortNode.Update();
		}
		cntFrames++;
	}
}

public class ShortNode {
	public Vector3 velocity;
	public Vector3 position;
	public Vector3 positionTarget;
	GameObject go; 
	ShortNode fromNode;
	List<ShortNode> toNodes;
	List<GameObject> goLinks;
	List<ShortNode> shortNodes;
	public int index;
	public int cnt;
	public ShortNode(float x0, float y0, float z0, List<ShortNode> shortNodes0) {
		position = new Vector3(x0, y0, z0);
		positionTarget = position;
		shortNodes = shortNodes0;
		InitToNodes();
		AddToShortNodes();
		CreateGo();
		ColorGo();
	}
	public void TypicalAvoid()
    {
        int nNearest = -1;
        float distNearest = -1;
        for (int n = 0; n < shortNodes.Count; n++)
        {
            if (n != index)
            {
                float dist = Vector3.Distance(position, shortNodes[n].position);
                if (nNearest == -1 || dist < distNearest)
                {
                    distNearest = dist;
                    nNearest = n;
                }
            }
        }
		float distMove = .125f;
        float distNear = 3.5f;
		float distNeighbor = Vector3.Distance(position, shortNodes[nNearest].position);
		if (distNeighbor < distNear) {
			Vector3 vector = position - shortNodes[nNearest].position;
            Vector3 posTargetAvoid = shortNodes[nNearest].position + Vector3.Normalize(vector) * distMove;
			SetTarget(posTargetAvoid);
		}
		//
		//Vector3 posCenter = new Vector3(0, 0, 0);
		//float distCenter = Vector3.Distance(posCenter, position);
		//float distRadMax = 6f;
		//if (distCenter < distRadMax) {
		//	Vector3 vectorCenter = posCenter - position;
		//	Vector3 posCenterAttract = position + Vector3.Normalize(vectorCenter) * -distMove; 
  //          SetTarget(posCenterAttract);
		//}
    }
	public void RandomTarget() {
		if (cnt % 1000 == 0) {
			Vector3 posRandom = position + Random.insideUnitSphere * 1;
			SetTarget(posRandom);
		}
		float distNear = 1f;
        ShortNode shortNode = FindNearestWithin(distNear);
        if (shortNode != null)
        {
            Vector3 vector = position - shortNode.position;
            positionTarget = position + Vector3.Normalize(vector) * distNear * 2;
        } 
	}
	public void SpreadOutSphere() {
		int nNearest = -1;
		float distNearest = -1;
		for (int n = 0; n < shortNodes.Count; n++) {
			if (n != index) {
				float dist = Vector3.Distance(position, shortNodes[n].position);
				if (nNearest == -1 || dist < distNearest) {
					distNearest = dist;
					nNearest = n;
				}                				
			}
		}
		float distGoalNearest = 1;
		Vector3 vector = position - shortNodes[nNearest].position;
		Vector3 posTargetNearest = shortNodes[nNearest].position + Vector3.Normalize(vector) * distGoalNearest;
//		SetTarget(posTarget);		
        //
		int nFarthest = -1;
        float distFarthest = -1;
        for (int n = 0; n < shortNodes.Count; n++) {
            if (n != index) {
                float dist = Vector3.Distance(position, shortNodes[n].position);
				if (nFarthest == -1 || dist > distFarthest) {
					distFarthest = dist;
					nFarthest = n;
                }                               
            }
        }
		float distGoalFarthest = 2;
		vector = position - shortNodes[nFarthest].position;
		Vector3 posTargetFarthest = shortNodes[nFarthest].position + Vector3.Normalize(vector) * distGoalFarthest;
		float smooth = .05f;
		Vector3 posTarget = smooth * posTargetNearest + posTargetFarthest * (1 - smooth);
		posTarget += Random.insideUnitSphere * .5f;
        SetTarget(posTarget);       
	}
	public void EvenDistanceToToNodes()
    {
		Vector3 posCentroid = FindCentroidOfOtherNodes();
		go.transform.LookAt(posCentroid);
		float distGoal = 6;
		float distCentroid = Vector3.Distance(posCentroid, position);
		Vector3 posTarget = posCentroid + go.transform.forward * (distCentroid - distGoal);
		SetTarget(posTarget);
    }
	public Vector3 FindCentroidOfOtherNodes()
    {
        Vector3 posSum = Vector3.zero;
        for (int n = 0; n < shortNodes.Count; n++)
        {
            if (n != index)
            {
                ShortNode shortNode = shortNodes[n];
                posSum += shortNode.position;
            }
        }
        return posSum / (shortNodes.Count - 1);
    }
	public void SetDistanceToNode(ShortNode shortNode, float distGoal) {
		Vector3 vector = shortNode.position - position;
		positionTarget = position + Vector3.Normalize(vector) * distGoal;
	}
	public Vector3 FindCentroidOfToNodes()
    {
		Vector3 posSum = Vector3.zero;
        for (int n = 0; n < toNodes.Count; n++)
        {
            ShortNode shortNode = toNodes[n];
			posSum += shortNode.position;
        }
        return posSum / toNodes.Count;
    }
	public float FindAverageDistToNodes()
    {
        float sum = 0;
		for (int n = 0; n < toNodes.Count; n++)
        {
			ShortNode shortNode = toNodes[n];
            float dist = Vector3.Distance(shortNode.position, position);
            sum += dist;
        }
		return sum / toNodes.Count;
    }
	public float FindAverageDist() {
		float sum = 0;
        for (int n = 0; n < shortNodes.Count; n++)
        {
            if (n != index)
            {
                ShortNode shortNode = shortNodes[n];
                float dist = Vector3.Distance(shortNode.position, position);
				sum += dist;
            }
        }
		return sum / (shortNodes.Count - 1);
	}
	public void Avoid() {
		float distNear = 2.1f;
		ShortNode shortNode = FindNearestWithin(distNear);
		if (shortNode != null)
		{
			Vector3 vector = position - shortNode.position;
			positionTarget = position + Vector3.Normalize(vector) * distNear;
		} 
	}
	public ShortNode FindNearest()
    {
        int nNear = -1;
		float distNear = 0;
        for (int n = 0; n < shortNodes.Count; n++)
        {
            if (n != index)
            {
                ShortNode shortNode = shortNodes[n];
                float dist = Vector3.Distance(shortNode.position, position);
				if (dist < distNear || nNear == -1)
                {
                    distNear = dist;
                    nNear = n;
                }
            }
        }
        if (nNear != -1)
        {
            return shortNodes[nNear];
        }
        else
        {
            return null;
        }
    }
	public ShortNode FindNearestWithin(float distNear) {
        int nNear = -1;
        for (int n = 0; n < shortNodes.Count; n++)
        {
            if (n != index)
            {
                ShortNode shortNode = shortNodes[n];
                float dist = Vector3.Distance(shortNode.position, position);
				if (dist < distNear)
                {
                    distNear = dist;
                    nNear = n;
                }
            }
        }
		if (nNear != -1) {
			return shortNodes[nNear];		
		} else {
			return null;
		}
	}
	public void SetTarget(Vector3 positionTarget0) {
		positionTarget = positionTarget0;
	}
	public void Update()
    {
		float smooth = .95f;
		position = smooth * position + (1 - smooth) * positionTarget;
        go.transform.position = position;
		UpdateLinks();
    }
	public void AddToShortNodes() {
		shortNodes.Add(this);
        index = shortNodes.Count - 1;
	}
	public void InitToNodes() {
		toNodes = new List<ShortNode>();
		goLinks = new List<GameObject>();
	}
	public void AddToToNodes(Vector3 posToNode) {
		for (int n = 0; n < shortNodes.Count; n++) {
			if (shortNodes[n].position == posToNode) {
				toNodes.Add(shortNodes[n]);
				shortNodes[n].fromNode = this;
				goLinks.Add(CreateLink(shortNodes[n].position));
				UpdateLink(toNodes.Count - 1);
				break;
			}
		}
	}
	public GameObject CreateLink(Vector3 posNode) {
		GameObject goLink = GameObject.CreatePrimitive(PrimitiveType.Cube);
		goLink.name = "link";
		goLink.transform.position = position;
		goLink.transform.localScale = new Vector3(.05f, .05f, .05f);
		goLink.GetComponent<Renderer>().material.color = Color.grey;
		return goLink;
	}
	public void UpdateLinks() {
		for (int n = 0; n < toNodes.Count; n++) {
			UpdateLink(n);
		}
	}
	public void UpdateLink(int n) {
		Vector3 posFrom = toNodes[n].position;
		Vector3 posTo = position;
		GameObject goLink = goLinks[n];
		goLink.transform.position = (posFrom + posTo) / 2;
		goLink.transform.LookAt(posTo);
		goLink.transform.localScale = new Vector3(.015f, .015f, Vector3.Distance(posFrom, posTo));
	}
	public void CreateGo() {
		go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.name = index + " " + position.ToString();
        go.transform.position = position;
        go.transform.localScale = new Vector3(.25f, .25f, .25f);
	}
	public void SetColor(Color col) {
		go.GetComponent<Renderer>().material.color = col;
	}
	public void ColorGo() {
		if (position.y == 1)
        {
            go.GetComponent<Renderer>().material.color = Color.blue;
        }
	}
}
