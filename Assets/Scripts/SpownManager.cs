using UnityEngine;

public class SpownManager : MonoBehaviour
{
    // �X�|�[���|�C���g�i�[�z��
    public Transform[] spownPoints;

    /// <summary>
    /// �J�n����
    /// </summary>
    private void Start()
    {
        foreach(Transform position in spownPoints)
        {
            position.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �����_���ɃX�|�[���|�C���g�̈��I��
    /// </summary>
    public Transform GetSpownPoint()
    {
        return spownPoints[Random.Range(0, spownPoints.Length)];
    }
}
