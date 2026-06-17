using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHp_UI : MonoBehaviour
{
    [SerializeField] Monster_Spec monsterSpec;
    [SerializeField] Image hpBar;
    private void Awake()
    {
        hpBar = gameObject.GetComponent<Image>();
        if (monsterSpec != null)
        {
            monsterSpec.MonsterHP += UpdateMonsterHP;
        }
    }

    void UpdateMonsterHP(float _value)
    {
        hpBar.fillAmount = _value;
    }

  
  
}
