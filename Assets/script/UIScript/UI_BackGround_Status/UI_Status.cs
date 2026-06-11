using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Status : MonoBehaviour
{
    [SerializeField] List<Text> statusText;
    [SerializeField] PlayerProgression pps;
    [SerializeField] PlayerSpecs plsp;


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
        if (pps != null && plsp != null)
        {
            pps.LevChage += UpdateLevel;
            plsp.HpChage += UpdateHp;
            plsp.MpChage += UpdateMp;
            pps.ExpChage += UpdateExp;
            plsp.TotalDamage += UpdateDamage;
            plsp.TotalDefense += UpdateDefens;
        }
        // 첫 활성화 시 UIBackground가 StatusRefresh를 Start 전에 호출해서 null 반환했으므로 여기서 직접 갱신
        StatusRefresh();
    }
    // 실제로 오브젝트 온오프되는곳에서 실행
    public void StatusRefresh()
    {
        // Start() 이전 첫 호출 시 pps/plsp가 아직 null이므로 스킵
        if (pps == null || plsp == null)
        {
            return;
        }
        statusText[0].text = pps.CURRENT_LEVEL.ToString();
        statusText[1].text = plsp.CURRENT_HP + " / " + plsp.MAX_HP;
        statusText[2].text = plsp.CURRENT_MP + " / " + plsp.MAX_MP;
        statusText[3].text = pps.CURRENT_EXP + " / " + pps.MAX_EXP;
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
