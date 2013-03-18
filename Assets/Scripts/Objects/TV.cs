using UnityEngine;
using System.Collections;

public class TV : MonoBehaviour
{
    public Material normalMat;

    public void Start()
    {
        if (normalMat == null)
            normalMat = renderer.material;
    }

    public void Change(Material mat)
    {
        if (renderer.material == normalMat)
            renderer.material = mat;
        else
            renderer.material = normalMat;
    }
}
