using UnityEngine;
using Photon.Pun;

public class SpownManager : MonoBehaviour
{
    // スポーンポイント格納配列
    public Transform[] spownPoints;

    // 生成するプレイヤーオブジェクト
    [SerializeField] private GameObject playerPrefab;

    // 生成したプレイヤーオブジェクト
    private GameObject player;

    // スポーンまでのインターバル
    public float respownInterval = 5f;

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        foreach(Transform position in spownPoints)
        {
            position.gameObject.SetActive(false);
        }

        // 生成関数呼び出し
        if(PhotonNetwork.IsConnected)
        {
            SpownPlayer();
        }
    }

    /// <summary>
    /// ランダムにスポーンポイントの一つを選択
    /// </summary>
    public Transform GetSpownPoint()
    {
        return spownPoints[Random.Range(0, spownPoints.Length)];
    }

    /// <summary>
    /// プレイヤーを生成する
    /// </summary>
    private void SpownPlayer()
    {
        // ランダムなスポーン位置
        Transform spownPoint = GetSpownPoint();

        // ネットワークオブジェクトの生成
        player = PhotonNetwork.Instantiate(playerPrefab.name, spownPoint.position, spownPoint.rotation);
    }
}
