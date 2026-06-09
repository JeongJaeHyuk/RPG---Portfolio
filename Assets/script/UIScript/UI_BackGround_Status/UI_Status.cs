using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Status : MonoBehaviour
{
    [SerializeField] List<Text> statusText;
    [SerializeField] UI ui;
    [SerializeField] PlayerProgression pps;
    [SerializeField] PlayerSpecs plsp;

    private void Awake()
    {
        Transform parentUI = gameObject.transform.root;
        ui = parentUI.GetComponent<UI>();
        pps = ui.GetPlayerProg();
        plsp = ui.GetPlayerSpecs();
        if (ui != null && pps != null && plsp != null)
        {
            pps.LevChage += UpdateLevel;
            plsp.HpChage += UpdateHp;
            plsp.MpChage += UpdateMp;
            pps.ExpChage += UpdateExp;
            plsp.TotalDamage += UpdateDamage;
            plsp.TotalDefense += UpdateDefens;
        }
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            Debug.Log("player감지못함");
            return;
        }
        pps = player.GetComponent<PlayerProgression>();
        plsp = player.GetComponent<PlayerSpecs>();
        statusText[4].text = plsp.TOTAL_DAMAGE.ToString();
        statusText[5].text = plsp.TOTAL_DEFENSE.ToString();
    }
    private void OnDestroy()
    {
        if (pps != null)
    {
        pps.LevChage -= UpdateLevel;
        pps.ExpChage -= UpdateExp;
    }
    if (plsp != null)
    {
        plsp.HpChage -= UpdateHp;
        plsp.MpChage -= UpdateMp;
        plsp.TotalDamage -= UpdateDamage;
        plsp.TotalDefense -= UpdateDefens;
    }
    }

    void UpdateLevel(float _value)
    {
        string text = _value.ToString();
        statusText[0].text = text;
    }
    void UpdateHp(float _value)
    {
        string maxHp = plsp.MAX_HP.ToString();
        string currentHp = plsp.CURRENT_HP.ToString();
        statusText[1].text = currentHp + " / " + maxHp;
    }
    void UpdateMp(float _value)
    {
        string maxHp = plsp.MAX_MP.ToString();
        string currentHp = plsp.CURRENT_MP.ToString();

        statusText[2].text = currentHp + " / " + maxHp;
    }
    void UpdateExp(float _value)
    {
        string maxExp = pps.MAX_EXP.ToString();
        string currentExp = pps.CURRENT_EXP.ToString();

        statusText[3].text = currentExp + " / " + maxExp;
    }
    void UpdateDamage(float _value)
    {
        string damage = _value.ToString();
        statusText[4].text = damage;
    }
    void UpdateDefens(float _value)
    {
        string defense = _value.ToString();
        statusText[5].text = defense;
    }
}
