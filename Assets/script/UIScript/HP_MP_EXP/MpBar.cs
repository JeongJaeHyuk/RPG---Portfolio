using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MpBar : MonoBehaviour
{
    [SerializeField] UI ui;
    [SerializeField] PlayerSpecs plsp;
    [SerializeField] Image mpbar;
    private void Awake()
    {
        Transform parentUI = gameObject.transform.root;
        ui = parentUI.GetComponent<UI>();
        plsp = ui.GetPlayerSpecs();
        if (ui != null && plsp != null)
        {
            plsp.MpChage += UpdateMp; // Hp변경 이벤트 구독
        }
    }

    private void OnDestroy()
    {
        if (plsp != null)
            plsp.MpChage -= UpdateMp; // 구독해제
    }
    void UpdateMp(float _value)
    {
        mpbar.fillAmount = _value;
    }
}
