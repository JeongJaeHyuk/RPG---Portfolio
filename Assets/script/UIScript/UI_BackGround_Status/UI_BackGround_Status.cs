using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BackGround_Status : MonoBehaviour
{
    void OnEnable()
    {
        UI.Instance.isPlayerStatOpen = true;
    }
    void OnDisable()
    {
        UI.Instance.isPlayerStatOpen = false;
    }
}
