using UnityEngine;

public class Gun : MonoBehaviour
{
    // �ˌ��Ԋu
    [Tooltip("�ˌ��Ԋu")]
    public float shotInterval = 0.1f;

    // �З�
    [Tooltip("�ˌ��З�")]
    public int shotDamage = 1;

    // �Y�[��
    [Tooltip("�`�����݃Y�[��")]
    public float adsZoom;

    // �`�����ݎ��̑��x
    [Tooltip("�`�����ݑ��x")]
    public float adsSpeed;

    // �e��
    public GameObject bulletInpact;

    Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void GunLockOn(bool aim)
    {   
         anim.SetBool("LookIn",aim);

    }
}
