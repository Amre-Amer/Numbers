using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

public class FibNNExample : MonoBehaviour
{
    FibNNManager fibManager;
    float startTime;
    float delay = .1f;
    bool ynDelay = true;

    // Use this for initialization
    void Start()
    {
        startTime = Time.realtimeSinceStartup;
		int numLevels = 11; //7;
        fibManager = new FibNNManager(numLevels);
    }

    // Update is called once per frame
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
