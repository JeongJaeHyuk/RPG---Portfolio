using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    [SerializeField] UI ui;
    [SerializeField] PlayerProgression pps;
    [SerializeField] Image expbar;
    [SerializeField] Text expText;
    private void Awake()
    {
        Transform parentUI = gameObject.transform.root;
        ui = parentUI.GetComponent<UI>();
        pps = ui.GetPlayerProg();
        if (ui != null && pps != null)
        {
            pps.ExpChage += UpdateExp; // Hp변경 이벤트 구독
        }
    }

    private void OnDestroy()
    {
        if (pps != null)
            pps.ExpChage -= UpdateExp; // 구독해제
    }
    void UpdateExp(float _value)
    {
        expbar.fillAmount = _value;
        string currentExp = pps.CURRENT_EXP.ToString();
        string maxExp = pps.MAX_EXP.ToString();
        expText.text = currentExp + " / " + maxExp;
    }
}
