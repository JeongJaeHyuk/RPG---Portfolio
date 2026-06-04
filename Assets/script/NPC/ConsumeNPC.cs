using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeNPC : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Shop_Manager.Instance.IsSetNpc(Shop_Manager.NPCType.Consume, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Shop_Manager.Instance.IsSetNpc(Shop_Manager.NPCType.Consume, false);
        }
    }
}
