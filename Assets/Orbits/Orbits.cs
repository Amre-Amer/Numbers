using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbits : MonoBehaviour {
	int numLevels = 4;
	// Use this for initialization
	void Start () {
		for (int lev = 0; lev < numLevels; lev++)
		{
			float rad = lev + 1;
			float radSquared = rad * rad;
			float numBalls = 4 * Mathf.PI * radSquared;
			numBalls *= 4;
			Debug.Log(lev + " " + numBalls + "\n");
			for (int n = 0; n < numBalls; n++)
			{
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				go.name = lev.ToString();
				float w = Mathf.Sqrt(numBalls);
				float h = w;
				float azim = n % w * (360 / w);
				float alt = n / w * (180 / h) - 90;
				go.transform.eulerAngles = new Vector3(alt, azim, 0);
				go.transform.position = Vector3.zero + go.transform.forward * rad;
				float s = 1f / radSquared;
				go.transform.localScale = new Vector3(s, s, s);
				Color col = Random.ColorHSV();
				if (lev == 0) col = Color.red;
				if (lev == 1) col = Color.green;
				if (lev == 2) col = Color.blue;
				if (lev == 3) col = Color.yellow;
				if (lev == 4) col = Color.cyan;
				go.GetComponent<Renderer>().material.color = col;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
