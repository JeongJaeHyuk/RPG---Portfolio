         using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_CoolTime : MonoBehaviour
{
    public Skill skill; // public으로 변경하여 외부에서 접근 가능 (Player.cs에서 사용)
    [SerializeField] Image coolImg;
    [SerializeField] GameObject coolObj;
    [SerializeField] Text coolText;
    public bool isCoolDown = false;
    
    private void Start()
    {
        coolImg = gameObject.transform.GetChild(0).GetComponent<Image>();
        coolObj = gameObject.transform.GetChild(0).gameObject;
        coolObj.SetActive(false);
    }
    public IEnumerator SkillCoolStart()
    {
        coolObj.SetActive(true);
        isCoolDown = true;
        float coolTime = skill.CURRENT_COOLTIME;
        float timer = 0;
        while (timer < coolTime)
        {
            timer += Time.deltaTime;
            // 1f - 로 해주는이유는 fillAmount값이 0 ~ 1로 올라가는게 아니라 1 ~ 0 으로 내려가는 코드이기떄문에 이렇게 구현
            coolImg.fillAmount = 1f - (timer / coolTime);
            float coolTImeText = coolTime - timer;
            coolText.text = Mathf.CeilToInt(coolTImeText).ToString();
            yield return null;
        }
        // 쿨타임이 끝났으니 다시 초기값으로 변경
        coolImg.fillAmount = 0f;
        isCoolDown = false;
        coolObj.SetActive(false);
    }
    public void skillSet(Skill _skill)
    {
        skill = _skill;
    }
}
