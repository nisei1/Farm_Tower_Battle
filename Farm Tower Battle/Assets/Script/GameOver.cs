using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public static bool isGameOverCollision = false;//衝突検出変数
    [SerializeField]
    public GameObject RetryButton;
    /// <summary>
    /// 衝突中を伝えるメソッド
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider other)
    {
        isGameOverCollision = true;
        Debug.Log(isGameOverCollision);
        Destroy(other.gameObject);
        RetryButton.SetActive(true);
    }

}
