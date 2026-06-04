using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Quest : MonoBehaviour
{
    public static UI_Quest instance = null;
    [Header("Player 경험치 접근")]
    [SerializeField] PlayerProgression playerProgression;

    [Header("UI 요소")]
    [SerializeField] Transform questSlotParent;         // 퀘스트 슬롯들의 부모 (Content)
    [SerializeField] GameObject questDetailPanel;       // 퀘스트 상세 정보 패널
    [SerializeField] GameObject questSlot;              // 퀘스트 슬롯 프리팹

    [Header("상세 정보 텍스트")]
    [SerializeField] Text questNameText;                // 퀘스트 이름
    [SerializeField] Text questDescriptionText;         // 퀘스트 설명
    [SerializeField] Text questProgressText;            // 진행도 (예: "3 / 5")
    [SerializeField] Text questRewardGoldText;          // 보상 골드
    [SerializeField] Text questRewardEXPText;           // 보상 경험치

    [Header("버튼")]
    [SerializeField] Button completeButton;             // 퀘스트완료 버튼
    [SerializeField] Button abandonButton;              // 퀘스트 포기 버튼
    [SerializeField] Sprite hoverButton;                // 퀘스트 클릭 가능 이미지
    [SerializeField] Sprite pushButton;                 // 퀘스트 클릭 불가능 이미지

    List<Quest> playerQuests = new List<Quest>();       // 플레이어가 수락한 퀘스트 리스트
    List<int> completedQuestIds = new List<int>();     // 완료한 퀘스트 ID 리스트
    Quest selectedQuest;                                // 현재 선택된 퀘스트
    Player_Quest_Slot previousSelectedSlot = null;      // 이전에 선택된 슬롯

    void Awake()
    {
        // TODO: 싱글톤 설정
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void OnEnable()
    {
        // 퀘스트창 여닫는 기능은 UI 스크립트에 구현해놧음
        RefreshQuestList();
    }
    void OnDisable()
    {
        questDetailPanel.SetActive(false);
        selectedQuest = null;
    }

    void Start()
    {
        // TODO: 퀘스트 창 비활성화
        gameObject.SetActive(false);
    }

    void RefreshQuestList()
    {
        // TODO: 퀘스트 슬롯 갱신
        // - questSlotParent의 모든 자식 슬롯 Clear()
        for(int i = 0; i < questSlotParent.childCount; i++)
        {
            Player_Quest_Slot questSlot = questSlotParent.GetChild(i).GetComponent<Player_Quest_Slot>();
            questSlot.Clear();
            if(i < playerQuests.Count)
            {
                // playerQuests 개수만큼 슬롯에 SetQuest() 호출
                questSlot.SetQuest(playerQuests[i]);
                // 오브젝트 활성화
                questSlot.gameObject.SetActive(true);
            }
            else
            {
                // 남은 슬롯은 비활성화
                questSlot.gameObject.SetActive(false);
            }
        }
    }

    public void SelectQuest(Quest _quest, Player_Quest_Slot _slot)
    {
        // TODO: 퀘스트 선택 시 상세 정보 표시
        // - 이전 슬롯 색상 복구 (previousSelectedSlot)
        if(previousSelectedSlot != null)
        {
            previousSelectedSlot.ResetColor();
        }
        // - 현재 슬롯 저장
        previousSelectedSlot = _slot;
        // - selectedQuest 저장
        selectedQuest = _quest;
        // - questNameText, questDescriptionText 설정
        questNameText.text = _quest.questName;
        questDescriptionText.text = _quest.description;
        // 목표 정보 설정 (타입에 따라 텍스트 변경)
        string actionText = "";
        switch (_quest.questType)
        {
            case QuestType.Kill:
                actionText = "처치";
                break;
            case QuestType.Collect:
                actionText = "수집";
                break;
            case QuestType.Visit:
                actionText = "방문";
                break;
            case QuestType.Boss:
                actionText = "토벌";
                break;
        }
        questProgressText.text = $"{_quest.targetNameKorean} {actionText} : {_quest.CURRENTPROGRESS} / {_quest.targetCount}";

        // 보상 정보
        questRewardGoldText.text = $"{_quest.rewardGold}";  // 골드텍스트 설정
        questRewardEXPText.text = $"{_quest.rewardExp}";    // 경험치 텍스트 설정

        // 상세 정보 패널 활성화
        questDetailPanel.SetActive(true);
        if(selectedQuest.CURRENTPROGRESS >= selectedQuest.targetCount)
        {
            // 퀘스트 버튼 활성화 시키고
            completeButton.enabled = true;
            // 퀘스트 버튼 이미지 push에서 hover변경
            completeButton.GetComponent<Image>().sprite = hoverButton;
        }
        else
        {
            // 퀘스트 버튼 활성화 시키고
            completeButton.enabled = false;
            // 퀘스트 버튼 이미지 push에서 hover변경
            completeButton.GetComponent<Image>().sprite = pushButton;
        }
    }
    

    // 퀘스트 수락했을떄 받아오는 함수 
    public void AcceptQuest(Quest _quest)
    {
        // - 복사 생성자로 새 Quest 객체 만들기 (new Quest(_quest))
        Quest quest = new Quest(_quest);
        // - playerQuests에 추가
        playerQuests.Add(quest);
        // - RefreshQuestList() 호출 (창이 열려있으면)
        RefreshQuestList();
        // 퀘스트 수락 함수 찾아서 수정
        if(quest.questType == QuestType.Collect)
        {
            int ownedCount = UI_Inventory.inven.GetItemCount(quest.targetType);
            if(ownedCount > 0)
            {
                CollectItemQuest(quest.targetType, ownedCount);
            }
        }
    }

    // 완료 버튼 눌렀을때
    public void CompleteQuestClick()
    {
        if(selectedQuest == null) return;

        // 목표를 달성했는지 확인 (진행도로 확인)
        if(selectedQuest.CURRENTPROGRESS < selectedQuest.targetCount)
        {
            Debug.Log("퀘스트 목표를 달성하지 못했습니다.");
            return;
        }
        // 완료 상태로 변경
        selectedQuest.status = QuestStatus.Completed;

        // 수집 퀘스트인 경우 인벤토리에서 아이템 제거
        if(selectedQuest.questType == QuestType.Collect)
        {
            UI_Inventory.inven.RemoveItem(selectedQuest.targetType, selectedQuest.targetCount);
        }

        // 반복 퀘스트가 아니면 완료 기록
        if(!selectedQuest.isRepeatable)
        {
            completedQuestIds.Add(selectedQuest.questId);
        }
        // 보상관련
        // 골드 보상
        UI_Inventory.inven.GOLD += selectedQuest.rewardGold;
        // 경험치 보상
        playerProgression.CURRENT_EXP += selectedQuest.rewardExp;
        // 아이템이 있다면
        // - 아이템: selectedQuest.rewardItemId, selectedQuest.rewardItemCount
        Debug.Log("골드 추가됨 : " + selectedQuest.rewardGold);
        Debug.Log("경험치 추가됨 : " + selectedQuest.rewardExp);
        
        // 추적 중이면 제거
        Quest_Progress_Popup.instance.RemoveTrackedQuest(selectedQuest);

        // playerQuests에서 제거
        playerQuests.Remove(selectedQuest);

        // UI 갱신
        questDetailPanel.SetActive(false);
        selectedQuest = null;
        RefreshQuestList();
    }

    public void OnAbandonButtonClick()
    {
        if(selectedQuest == null) return;

        // 추적 중이면 제거
        Quest_Progress_Popup.instance.RemoveTrackedQuest(selectedQuest);

        // playerQuests에서 제거
        playerQuests.Remove(selectedQuest);

        // UI 갱신
        questDetailPanel.SetActive(false);
        selectedQuest = null;
        RefreshQuestList();
    }
    public void KillMonsterQuest(string _monsName)
    {
        // 몬스터의 이름의 뒤에 번호로 다른것이 있을수 있어 _부터 다 잘라버리고 순수이름만 가져옴
        string name = _monsName.Split('_')[0];
        for (int i = 0 ; i < playerQuests.Count; i++)
        {
            Quest quest = playerQuests[i];
            // 퀘스트가 사냥 퀘스트 일경우
            if(quest.questType == QuestType.Kill)
            {
                // 퀘스트 타켓의 이름과 몬스터 이름이 동일한 경우
                if(quest.targetType == name)
                {
                    // 현재 처치 증가
                    quest.CURRENTPROGRESS++;
                    if(quest.CURRENTPROGRESS >= quest.targetCount)
                    {
                        quest.CURRENTPROGRESS = quest.targetCount;
                    }
                }
            }
        }
    }

    // 아이템 수집 퀘스트 처리 (아이템 획득 시 호출)
    public void CollectItemQuest(string _itemTypeName, int _count)
    {      
        // playerQuests를 for문으로 순회
        for (int i =0; i < playerQuests.Count; i++)
        {
            Quest quest = playerQuests[i];
            // 수집퀘스트인지 확인
            if(quest.questType == QuestType.Collect)
            {
                // 퀘스트아이템이 일치하는지 확인
                if(quest.targetType == _itemTypeName)
                {
                    // 일치하면 quest.CURRENTPROGRESS에 _count만큼 진행도 증가
                    quest.CURRENTPROGRESS += _count;
                    // 최대값 초과 방지 (targetCount 넘지 않게)
                    if(quest.CURRENTPROGRESS >= quest.targetCount)
                    {
                        // 최대값으로 유지
                        quest.CURRENTPROGRESS = quest.targetCount;
                    }
                }
            }
        }
    }

    // 장소 방문 퀘스트 처리 (특정 위치/NPC 도착 시 호출)
    public void VisitLocationQuest(string _locationName)
    {
        // 1. 위치 이름 정제 (필요시 Split 등 사용)
        // csv파일의 targetType랑 NPC스크립트 npcName이랑 동일해야 정상작동됨
        string name = _locationName.Split('_')[0];
        for (int i = 0 ; i < playerQuests.Count; i++)
        {
            Quest quest = playerQuests[i];
            // 퀘스트가 방문 퀘스트 일경우
            if(quest.questType == QuestType.Visit)
            {
                
                if(quest.targetType == name)
                {
                    // 방문카운트증가
                    quest.CURRENTPROGRESS++;
                    // 초과되지않게 방지
                    if(quest.CURRENTPROGRESS >= quest.targetCount)
                    {
                        quest.CURRENTPROGRESS = quest.targetCount;
                    }
                }
            }
        }
        // 2. playerQuests를 for문으로 순회
        // 3. quest.questType이 QuestType.Visit인지 확인
        // 4. quest.targetType과 _locationName이 일치하는지 확인
        // 5. 일치하면 quest.CURRENTPROGRESS++ (보통 방문은 1회만)
        // 6. 최대값 초과 방지 (targetCount 넘지 않게)
    }

    // 퀘스트 완료 여부 확인 (NPC 창에서 필터링할 때 사용)
    public bool IsQuestCompleted(int questId)
    {
        return completedQuestIds.Contains(questId);
    }

    // 퀘스트 수락 여부 확인 (NPC 창에서 필터링할 때 사용)
    public bool IsQuestAccepted(int questId)
    {
        return playerQuests.Exists(q => q.questId == questId);
    }
}
