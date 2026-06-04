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
        // 싱글톤 설정
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // TODO: 초기화
        slots = new List<Quest_Progress_Slot>();
        trackedQuests = new List<Quest>();
        // slotParent의 모든 자식 순회 (for문으로 slotParent.childCount)
        for(int i = 0; i < slotParent.childCount; i++)
        {
            // GetChild(i).GetComponent<Quest_Progress_Slot>()로 슬롯 가져오기
            Quest_Progress_Slot slot = slotParent.GetChild(i).GetComponent<Quest_Progress_Slot>();
            // slots.Add(slot)으로 리스트에 추가
            slots.Add(slot);
            // slot.gameObject.SetActive(false)로 모든 슬롯 비활성화
            slot.gameObject.SetActive(false);
            // 슬롯를 4개이상 생성했을경우 경고문 실행
            if(slots.Count > maxSlotCount)
            {
                Debug.LogWarning("슬롯의 최대갯수는 3개입니다");
            }
        }
    }
    // 보유중인 퀘스트를 팝업에 넣을떄 사용하는함수
    // 현재 팝업에 등록된 갯수가 최대갯수보다 작을떄 참을 리턴
    public bool QuestCheck()
    {
        if(trackedQuests.Count < maxSlotCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 퀘스트가 이미 추적 중인지 확인
    public bool IsTracking(Quest _quest)
    {
        return trackedQuests.Contains(_quest);
    }
    public void AddTrackedQuest(Quest _quest)
    {
        // 퀘스트 추적 등록 (Player_Quest_Slot의 SelectedPopUpClick에서 호출됨)
        // 중복 체크
        if(trackedQuests.Contains(_quest))
        {
            return; // 이미 추적 중이면 추가하지 않음
        }
        // 빈 슬롯 찾기
        Quest_Progress_Slot slot = GetAvailableSlot();
        if(slot == null)
        {
            return; // 빈 슬롯이 없으면 추가 불가
        }
        // 추적 리스트에 추가
        trackedQuests.Add(_quest);
        // 슬롯에 퀘스트 설정 (슬롯에서 이벤트 구독 처리)
        slot.SetQuest(_quest);
        slot.gameObject.SetActive(true);
    }

    public void RemoveTrackedQuest(Quest _quest)
    {
        // 퀘스트 추적 해제 (Player_Quest_Slot의 SelectedPopUpClick에서 호출됨)
        // trackedQuests에서 제거
        if(!trackedQuests.Contains(_quest))
            return;
        trackedQuests.Remove(_quest);

        // 해당 퀘스트를 표시하는 슬롯 찾기
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].GetQuest() == _quest)
            {
                slots[i].Clear(); // Clear에서 이벤트 구독 해제 처리
                slots[i].gameObject.SetActive(false);
                break;
            }
        }
    }

    // 사용가능한 빈슬롯 찾는 함수
    Quest_Progress_Slot GetAvailableSlot()
    {
        for(int i = 0; i <slots.Count; i++)
        {
            // 슬롯들중 슬롯오브젝트가 비활성화 되어있는 경우 실행
            if(!slots[i].gameObject.activeSelf)
            {
                return slots[i];
            }
        }
        // - 빈 슬롯이 없으면 return null;
        return null;
    }

    public void ClearAllTrackedQuests()
    {
        // TODO: 모든 추적 중인 퀘스트 해제 (게임 종료 시 등)
        // 1. 추적 리스트 초기화
        //    - trackedQuests.Clear();
        //
        // 2. 모든 슬롯 초기화
        //    - for문으로 slots 순회
        //    - slots[i].Clear(); (Clear에서 이벤트 구독 해제 처리)
        //    - slots[i].gameObject.SetActive(false);
    }
}
