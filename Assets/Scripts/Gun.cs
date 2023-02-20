using UnityEngine;

public class Gun : MonoBehaviour
{
    // ËŒ‚ŠÔŠu
    [Tooltip("ËŒ‚ŠÔŠu")]
    public float shotInterval = 0.1f;

    // ˆĞ—Í
    [Tooltip("ËŒ‚ˆĞ—Í")]
    public int shotDamage = 1;

    // ƒY[ƒ€
    [Tooltip("”`‚«‚İƒY[ƒ€")]
    public float adsZoom;

    // ”`‚«‚İ‚Ì‘¬“x
    [Tooltip("”`‚«‚İ‘¬“x")]
    public float adsSpeed;

    // ’e­
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
