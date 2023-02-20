using UnityEngine;

public class SpownManager : MonoBehaviour
{
    // スポーンポイント格納配列
    public Transform[] spownPoints;

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        foreach(Transform position in spownPoints)
        {
            position.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ランダムにスポーンポイントの一つを選択
    /// </summary>
    public Transform GetSpownPoint()
    {
        return spownPoints[Random.Range(0, spownPoints.Length)];
    }
}
