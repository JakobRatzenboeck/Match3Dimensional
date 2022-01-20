using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeLineHelp : MonoBehaviour
{
    [SerializeField]
    private Transform xPositive;
    [SerializeField]
    private Transform xNegative;
    [SerializeField]
    private Transform yPositive;
    [SerializeField]
    private Transform yNegative;
    [SerializeField]
    private Transform zPositive;
    [SerializeField]
    private Transform zNegative;


    public void ReseizeLineHelper(int xp, int xn, int yp, int yn, int zp, int zn)
    {
        xPositive.localScale = Vector3.Scale(xPositive.localScale, (Vector3.right * xp));
        xNegative.localScale = Vector3.Scale(xNegative.localScale, (Vector3.right * xn));
        yPositive.localScale = Vector3.Scale(yPositive.localScale, (Vector3.up * yp));
        yNegative.localScale = Vector3.Scale(yNegative.localScale, (Vector3.up * yn));
        zPositive.localScale = Vector3.Scale(zPositive.localScale, (Vector3.forward * zp));
        zNegative.localScale = Vector3.Scale(zNegative.localScale, (Vector3.forward * zn));
    }
}
