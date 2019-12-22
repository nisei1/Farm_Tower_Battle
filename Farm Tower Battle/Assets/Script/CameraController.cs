using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static bool isCollision;//衝突検出変数

    /// <summary>
    /// 衝突中を伝えるメソッド
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay(Collider collision)
    {
        isCollision = true;
        //Debug.Log(isCollision);
    }

    /// <summary>
    /// 衝突終了を伝えるメソッド
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit(Collider collision)
    {
        isCollision = false;
    }
}