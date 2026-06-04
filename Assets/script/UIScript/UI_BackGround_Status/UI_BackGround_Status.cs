using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BackGround_Status : MonoBehaviour
{
    void OnEnable()
    {
        UI.instance.isPlayerStatOpen = true;
    }
    void OnDisable()
    {
        UI.instance.isPlayerStatOpen = false;
    }
}
