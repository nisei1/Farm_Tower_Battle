using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;
using Photon.Realtime;

public class AnimalGenerator : MonoBehaviourPunCallbacks
{
    public GameObject[] animals;//どうぶつ取得配列
    public Camera mainCamera;//カメラ取得用変数
    public float pivotHeight = 6.5f;//生成位置の基準
    [SerializeField]
    public static int animalNum = 0;//生成された動物の個数を保管
    [SerializeField]
    public static bool isGameOver = false;//ゲームオーバー判定

    private GameObject geneAnimal;//どうぶつ生成（単品）

    [SerializeField]
    public bool isGene;//生成されているか
    [SerializeField]
    public bool isFall;//生成された動物が落下中か

    // ロビー最大人数
    [SerializeField] private int maxPlayers = 2;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        //Init();
    }

    void OnGUI()
    {
        //ログインの状態を画面上に出力
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Init();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Init()
    {
        animalNum = 0;
        isGameOver = false;
        Animal.isMoves.Clear();//移動してる動物のリストを初期化
        StartCoroutine(StateReset());
    }

    // 毎フレーム呼び出される(60fpsだったら1秒間に60回)
    void Update()
    {
        if (GameOver.isGameOverCollision) //GameOverColiderにオブジェクトが当たると
        {
            isGameOver = true; //ゲームオーバー
        }
        if (isGameOver)
        {
            return;//ゲームオーバーならここで止める
        }

        if (CheckMove(Animal.isMoves))
        {
            return;//移動中なら処理はここまで
        }

        if (!isGene)//生成されてるものがない
        {
            StartCoroutine(GenerateAnimal());//生成するコルーチンを動かす
            isGene = true;
            return;
        }
        if (!EventSystem.current.IsPointerOverGameObject())    //レイをボタンに当たらないようにする
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    //レイが当たった位置を得るよ
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                float x = hit.point.x;
                float z = hit.point.z;

                Vector3 v = new Vector3(x, pivotHeight, z); //オブジェクトを生成した高さで座標を得る

                //Vector3 v = new Vector3(mainCamera.ScreenToWorldPoint(Input.mousePosition).x, pivotHeight);

                if (Input.GetMouseButtonUp(0))//もし（マウス左クリックが離されたら）
                {
                    if (!RotateButton.onButtonDown)//ボタンをクリックしていたら反応させない
                    {
                        geneAnimal.transform.position = v;
                        geneAnimal.GetComponent<Rigidbody>().isKinematic = false;//――――物理挙動・オン
                        animalNum++;//どうぶつ生成

                    }
                    geneAnimal.GetComponent<Rigidbody>().isKinematic = false;//――――物理挙動・オン 上と同じ。バグを治すために一時的にここに書いた
                    RotateButton.onButtonDown = false;//マウスが上がったらボタンも離れたと思う
                    isFall = true;//落ちて、どうぞ
                }
                else if (Input.GetMouseButton(0))//ボタンが押されている間
                {
                    geneAnimal.transform.position = v;
                }
            }
        }
    }

    /// <summary>
    /// 生成・落下状態をリセットするコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator StateReset()
    {
        while (!isGameOver)
        {
            yield return new WaitUntil(() => isFall);//落下するまで処理が止まる
            yield return new WaitForSeconds(0.1f);//少しだけ物理演算処理を待つ（ないと無限ループ）
            isFall = false;
            isGene = false;
        }
    }

    /// <summary>
    /// どうぶつの生成コルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateAnimal()
    {
        while (CameraController.isCollision)
        {
            yield return new WaitForEndOfFrame();//フレームの終わりまで待つ（無いと無限ループ）
            mainCamera.transform.Translate(0, 0.1f, 0, Space.World);//カメラを少し上に移動
            pivotHeight += 0.1f;//生成位置も少し上に移動
        }
        geneAnimal = PhotonNetwork.Instantiate(animals[Random.Range(0, animals.Length)].name, new Vector3(0, pivotHeight), Quaternion.identity);//回転せずに生成
        geneAnimal.GetComponent<Rigidbody>().isKinematic = true;//物理挙動をさせない状態にする
    }

    /// <summary>
    /// どうぶつの回転
    /// ボタンにつけて使います
    /// </summary>
    public void RotateAnimal()
    {
        if (!isFall)
            geneAnimal.transform.Rotate(0, 0, -30);//30度ずつ回転
    }

    /// <summary>
    /// リトライボタン
    /// </summary>
    public void Retry()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    /// <summary>
    /// 移動中かチェック
    /// </summary>
    /// <param name="isMoves"></param>
    /// <returns></returns>
    bool CheckMove(List<Moving> isMoves)
    {
        if (isMoves == null)
        {
            return false;
        }
        foreach (Moving b in isMoves)
        {
            if (b.isMove)
            {
                Debug.Log("移動中(*'ω'*)");
                return true;
            }
        }
        return false;
    }
}