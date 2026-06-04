using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] UI ui;
    [SerializeField] PlayerSpecs plsp;
    [SerializeField] Image hpbar;
    private void Awake()
    {
        Transform parentUI = gameObject.transform.root;
        ui = parentUI.GetComponent<UI>();
        plsp = ui.GetPlayerSpecs();
        if (ui != null && plsp != null)
        {
            // player.HpChage += UpdateHp; // Hp변경 이벤트 구독
            plsp.HpChage += UpdateHp;
        }
    }

    private void OnDestroy()
    {
        //if (player != null)
        //    player.HpChage -= UpdateHp; // 구독해제
        if (plsp != null)
            plsp.HpChage -= UpdateHp; //구독해제
    }
    void UpdateHp(float _value)
    {
        hpbar.fillAmount = _value;
    }
}
