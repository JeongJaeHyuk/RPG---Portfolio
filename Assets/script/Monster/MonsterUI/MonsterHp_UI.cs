using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHp_UI : MonoBehaviour
{
    [SerializeField] Monster_Stats monsterStats;
    [SerializeField] Image hpBar;
    private void Awake()
    {
        hpBar = gameObject.GetComponent<Image>();
        if (monsterStats != null)
        {
            monsterStats.MonsterHP += UpdateMonsterHP;
        }
    }
    void Start()
    {
        
    }

    void UpdateMonsterHP(float _value)
    {
        hpBar.fillAmount = _value;
    }

  
  
}
