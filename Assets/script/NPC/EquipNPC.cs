using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipNPC : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Shop_Manager.Instance.IsSetNpc(Shop_Manager.NPCType.Equip, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Shop_Manager.Instance.IsSetNpc(Shop_Manager.NPCType.Equip, false);
        }
    }
}
