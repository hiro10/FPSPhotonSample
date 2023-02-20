using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    // íeñÚ
    public TextMeshProUGUI ammoText;

    public void SettingBulletsText(int ammoClip,int ammunition)
    {
        // É}ÉKÉWÉìì‡ÇÃíeêî
        ammoText.text = ammoClip + "/" + ammunition;
    }
}
