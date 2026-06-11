using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class UI_BackGround_Status : MonoBehaviour
{
    [SerializeField] UI_Status  Status;
    void OnEnable()
    {
        Status.StatusRefresh();
        UI.Instance.isPlayerStatOpen = true;
    }
    void OnDisable()
    {
        UI.Instance.isPlayerStatOpen = false;
    }
}
