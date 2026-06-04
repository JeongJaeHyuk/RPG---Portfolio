using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 퀵슬롯 키 입력 관리 매니저 (싱글톤)
public class QuickSlotManager : MonoBehaviour
{
    public static QuickSlotManager Instance { get; private set; } = null;

    [SerializeField] Qick_Slot[] quickSlots; // 4개의 퀵슬롯 배열

    void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        // 1번 키
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            quickSlots[0].UseQuickSlot();
        }
        // 2번 키
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            quickSlots[1].UseQuickSlot();
        }
        // 3번 키
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            quickSlots[2].UseQuickSlot();
        }
        // 4번 키
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            quickSlots[3].UseQuickSlot();
        }
    }
}
