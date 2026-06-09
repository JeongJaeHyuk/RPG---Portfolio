using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] UI ui;
    [SerializeField] PlayerSpecs plsp;
    [SerializeField] Image hpbar;
    void Start()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null)
        {
            plsp = objPlayer.GetComponent<PlayerSpecs>();
            plsp.HpChage += UpdateHp;
            if (plsp.MAX_HP > 0)
                UpdateHp(plsp.CURRENT_HP / plsp.MAX_HP);
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
