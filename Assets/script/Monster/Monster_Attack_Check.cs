using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Attack_Check : MonoBehaviour
{
    Monster_Attack_Hit monsterAttack;
    private void Awake()
    {
        monsterAttack = transform.parent.GetComponent<Monster_Attack_Hit>();
    }
    private void OnTriggerStay(Collider other)
    {
        string name = LayerMask.LayerToName(other.gameObject.layer);
        if (name == "Player")
        {
            monsterAttack.ISATTACK = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        string name = LayerMask.LayerToName(other.gameObject.layer);
        if (name == "Player")
        {           
            monsterAttack.ISATTACK = false;
        }
    }
}
