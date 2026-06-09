using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MpBar : MonoBehaviour
{
    [SerializeField] UI ui;
    [SerializeField] PlayerSpecs plsp;
    [SerializeField] Image mpbar;
    void Start()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null)
        {
            plsp = objPlayer.GetComponent<PlayerSpecs>();
            plsp.MpChage += UpdateMp;
            if (plsp.MAX_MP > 0)
                UpdateMp(plsp.CURRENT_MP / plsp.MAX_MP);
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
