using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Numbers : MonoBehaviour
{
    DeltronClass deltron;
    int numData = 5;

    // Use this for initialization
    void Start()
    {
        deltron = new DeltronClass(1, 3, 3, 1, 1, 1);
        //Debug.Log(deltron.Sigmoid(2));
        //Debug.Log(deltron.Sigmoid(0));
        //Debug.Log(deltron.Sigmoid(-2f));
        //Debug.Log(deltron.Sigmoid(0));
        //Debug.Log(deltron.Sigmoid(1));
        //Debug.Log(deltron.Sigmoid(2));
        //Debug.Log(deltron.Sigmoid(4));
        //Debug.Log(deltron.Sigmoid(6));
        deltron = new DeltronClass(1, 3, 3, 1, 1, 1);
        deltron = new DeltronClass(1, 1, 3, 3, 1, 1);
        deltron = new DeltronClass(1, 1, 1, 3, 3, 1);
        deltron = new DeltronClass(1, 1, 1, 1, 3, 3);
        deltron = new DeltronClass(1, 3, 1, 1, 1, 3);
        deltron = new DeltronClass(1, 3, 3, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class DeltronClass
{
    int numData;
    DataStruct[] data;
    float[,] aggs;
    int numLevels;
    //public DeltronClass(DataStruct[] data0)
    //{
    //    numData = data0.Length;
    //    data = new DataStruct[numData];
    //    numLevels = numData;
    //    aggs = new float[numData, numLevels];
    //    LoadData(data0);
    //}
    public DeltronClass(float x1, float x2, float x3, float x4, float x5, float x6) {
        numData = 6;        
        data = new DataStruct[numData];
        numLevels = numData;
        aggs = new float[numData, numLevels];
        data[0].x = x1;
        data[1].x = x2;
        data[2].x = x3;
        data[3].x = x4;
        data[4].x = x5;
        data[5].x = x6;
        //        float agg = Feed();
        //        float agg = FeedAve();
        float agg = FeedThreshold();
        Debug.Log(x1 + " " + x2 + " " + x3 + " " + x4 + " " + x5 + " " + x6 + " = " + agg.ToString("F4") + "\n");
    }
    public void LoadData(DataStruct[]data0) {
        data = data0;        
    }
    public float FeedThreshold() {
        float a = 0;
        float b = 0;
        for (int d = 0; d < numData; d++)
        {
            aggs[d, 0] = data[d].x;
        }
        for (int level = 1; level < numLevels; level++)
        {
            for (int d = 0; d < numData - level; d++)
            {
                a = aggs[d, level - 1];
                b = aggs[d + 1, level - 1];
                if (a - b == 0) {
                    aggs[d, level] = (a + b) / 2f;
                } else {
                    aggs[d, level] = (a + b) / 2f;
                }
            }
        }
        float agg = aggs[0, numLevels - 1];
        //agg = Sigmoid(agg);
        return agg;
    } 
    public float FeedAve() {
        float a = 0;
        float b = 0;
        for (int d = 0; d < numData; d++)
        {
            aggs[d, 0] = data[d].x;
        }
        for (int level = 1; level < numLevels; level++)
        {
            for (int d = 0; d < numData - level; d++)
            {
                a = aggs[d, level - 1];
                b = aggs[d + 1, level - 1];
                aggs[d, level] = (a + b) / 2f;
            }
        }
        float agg = aggs[0, numLevels - 1];
        //agg = Sigmoid(agg);
        return agg;
    } 
    public float Feed() {
        float a = 0;
        float b = 0;
        for (int d = 0; d < numData; d++) {
            aggs[d, 0] = data[d].x;
        }
        for (int level = 1; level < numLevels; level++)
        {
            for (int d = 0; d < numData - level; d++)
            {
                if (d == 0)
                {
                    a = aggs[d, level - 1];
                }
                else
                {
                    a = aggs[d - 1, level];
                }
                b = aggs[d + 1, level - 1];
                aggs[d, level] = b / a;
            }
        }
        float agg = aggs[0, numLevels - 1];
        agg = Sigmoid(agg); 
        return agg;
    }
    public float Sigmoid(float x) {
        return 1 / (1 + Mathf.Exp(-x));
    }
    public float SquareRoot(float x)
    {
        return Mathf.Sqrt(x);
    }
    public void Show()
    {
        for (int d = 0; d < data.Length; d++)
        {
            Debug.Log(d + " " + data[d].x + "\n");
        }
    }
}

public struct DataStruct{
    public float x;
}