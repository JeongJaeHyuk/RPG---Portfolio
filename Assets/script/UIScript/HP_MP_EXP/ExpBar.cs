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
    void Start()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        if (objPlayer != null)
        {
            pps = objPlayer.GetComponent<PlayerProgression>();
            pps.ExpChage += UpdateExp;
            if (pps.MAX_EXP > 0)
                UpdateExp(pps.CURRENT_EXP / pps.MAX_EXP);
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
