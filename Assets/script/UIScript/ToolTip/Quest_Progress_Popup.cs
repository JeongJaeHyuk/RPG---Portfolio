using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest_Progress_Popup : MonoBehaviour
{
    public static Quest_Progress_Popup instance = null;

    [Header("슬롯 설정")]
    [SerializeField] Transform slotParent;          // 슬롯들의 부모 (Vertical Layout Group)
    [SerializeField] int maxSlotCount = 3;          // 최대 추적 가능한 퀘스트 개수

    List<Quest_Progress_Slot> slots;                // 슬롯 리스트
    List<Quest> trackedQuests;                      // 추적 중인 퀘스트 리스트

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        slots = new List<Quest_Progress_Slot>();
        trackedQuests = new List<Quest>();

        for(int i = 0; i < slotParent.childCount; i++)
        {
            Quest_Progress_Slot slot = slotParent.GetChild(i).GetComponent<Quest_Progress_Slot>();
            slots.Add(slot);
            slot.gameObject.SetActive(false);

            if(slots.Count > maxSlotCount)
            {
                Debug.LogWarning("슬롯의 최대갯수는 3개입니다");
            }
        }
    }

    // 팝업에 추가 가능한 슬롯이 남아있는지 확인 (Player_Quest_Slot의 추적 버튼에서 호출)
    public bool QuestCheck()
    {
        return trackedQuests.Count < maxSlotCount;
    }

    // 추적 중인 퀘스트 리스트 반환 (저장 시 사용)
    public List<Quest> GetTrackedQuests()
    {
        return trackedQuests;
    }

    // 해당 퀘스트가 이미 팝업에 추적 중인지 확인
    public bool IsTracking(Quest _quest)
    {
        return trackedQuests.Contains(_quest);
    }

    // 퀘스트를 팝업에 추적 등록 (Player_Quest_Slot의 추적 버튼에서 호출)
    public void AddTrackedQuest(Quest _quest)
    {
        if(trackedQuests.Contains(_quest))
        {
            return;
        }
        Quest_Progress_Slot slot = GetAvailableSlot();
        if(slot == null)
        {
            return;
        }
        trackedQuests.Add(_quest);
        // SetQuest 내부에서 OnProgressChanged 이벤트 구독 — 진행도 변경 시 자동 갱신
        slot.SetQuest(_quest);
        slot.gameObject.SetActive(true);
    }

    // 퀘스트 추적 해제 — 퀘스트 완료/포기 시 UI_Quest에서 호출
    public void RemoveTrackedQuest(Quest _quest)
    {
        if(!trackedQuests.Contains(_quest))
        {
            return;
        }
        trackedQuests.Remove(_quest);

        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].GetQuest() == _quest)
            {
                // Clear 내부에서 OnProgressChanged 이벤트 구독 해제
                slots[i].Clear();
                slots[i].gameObject.SetActive(false);
                break;
            }
        }
    }

    // 비활성화된 빈 슬롯을 찾아 반환 (없으면 null)
    Quest_Progress_Slot GetAvailableSlot()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            if(!slots[i].gameObject.activeSelf)
            {
                return slots[i];
            }
        }
        return null;
    }

    // 로드 시 호출 — 새 Quest 객체로 교체되면 기존 이벤트 구독이 끊기므로
    // 팝업을 완전히 초기화하고 플레이어가 다시 등록하도록 유도
    public void ClearAllTrackedQuests()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Clear();
            slots[i].gameObject.SetActive(false);
        }
        trackedQuests.Clear();
    }
}
