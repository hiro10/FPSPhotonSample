using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    // �e��
    public TextMeshProUGUI ammoText;

    public void SettingBulletsText(int ammoClip,int ammunition)
    {
        // �}�K�W�����̒e��
        ammoText.text = ammoClip + "/" + ammunition;
    }
}
