using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//To convert data from one type to another
public class Converter 
{
    public static List<object> ToObjects(Vector3[] _arr)
    {
        List<object> temp = new List<object>();
        for (int i = 0; i < _arr.Length; i++)
        {
            string data = String.Format("{0},{1},{2}", _arr[i].x, _arr[i].y, _arr[i].z);
            temp.Add(data);

        }
        return temp;
    }

    public static Vector3[] ToArray(List<object> _arr)
    {
        List<Vector3> temp = new List<Vector3>();

        for (int i = 0; i < _arr.Count; i++)
        {
            string[] data = _arr[i].ToString().Split(',');

            temp.Add(new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2])));
        }

        return temp.ToArray();
    }

}
