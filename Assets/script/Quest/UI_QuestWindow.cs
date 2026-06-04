using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestWindow : MonoBehaviour
{
    // n
    public static UI_QuestWindow instance = null;

    [Header("UI 요소")]
    [SerializeField] GameObject questListPanel;      // 왼쪽: 퀘스트 리스트 패널
    [SerializeField] GameObject questDetailPanel;    // 오른쪽: 퀘스트 상세 정보 패널
    [SerializeField] Transform questSlotParent;      // 퀘스트 슬롯들의 부모 (Content)

    [Header("상세 정보 텍스트")]
    [SerializeField] Text questNameText;             // 퀘스트 이름
    [SerializeField] Text questDescriptionText;      // 퀘스트 설명
    [SerializeField] Text questTargetText;           // 목표 (예: "암석거인 처치 0/5")
    [SerializeField] Text questRewardGoldText;       // 보상 정보(골드)
    [SerializeField] Text questRewardEXPText;        // 보상 정보(경험치)

    [Header("버튼")]
    [SerializeField] Button acceptButton;            // 수락 버튼
    [SerializeField] Button declineButton;           // 거절 버튼

    List<Quest> currentNPCQuests;                    // 현재 NPC의 퀘스트 리스트
    Quest selectedQuest;                             // 현재 선택된 퀘스트
    QuestSlot previousSelectedSlot = null;           // 이전에 선택된 슬롯

    void Awake()
    {
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
        // 퀘스트창 켜져질때 상세정보창 꺼져야함
        questDetailPanel.gameObject.SetActive(false);
    }
    void Start()
    {
        // 로드 필요 없음, Quest_Data_Manager에서 이미 로드됨
        gameObject.SetActive(false);
    }

    // 퀘스트창 열기 퀘스트 NPC스크립트에서 사용
    public void ShowQuestWindow(int _npcId)
    {
        // Quest_Data_Manager에서 NPC의 퀘스트 리스트 가져오기
        currentNPCQuests = Quest_Data_Manager.instance.GetQuestsByNPC(_npcId);
        if(currentNPCQuests != null)
        {
            // 왼쪽 리스트 갱신
            RefreshQuestList();
            gameObject.SetActive(true);
        }
    }

    void RefreshQuestList()
    {
        // 수락 가능한 퀘스트만 필터링
        List<Quest> availableQuests = new List<Quest>();
        foreach(Quest quest in currentNPCQuests)
        {
            // 1. 이미 수락한 퀘스트면 스킵
            if(UI_Quest.instance.IsQuestAccepted(quest.questId))
                continue;

            // 2. 반복 퀘스트가 아니고 완료한 퀘스트면 스킵
            if(!quest.isRepeatable && UI_Quest.instance.IsQuestCompleted(quest.questId))
                continue;

            // 3. 수락 가능한 퀘스트만 리스트에 추가
            availableQuests.Add(quest);
        }

        // 모든 슬롯 초기화 및 갱신
        for (int i = 0; i < questSlotParent.childCount; i++)
        {
            QuestSlot questSlot = questSlotParent.GetChild(i).GetComponent<QuestSlot>();
            questSlot.Clear();

            // 수락 가능한 퀘스트만 표시
            if (i < availableQuests.Count)
            {
                questSlot.SetQuest(availableQuests[i]);
                questSlot.gameObject.SetActive(true);
            }
            else
            {
                // 남은 슬롯은 비활성화
                questSlot.gameObject.SetActive(false);
            }
        }
    }

    public void SelectQuest(Quest _quest, QuestSlot _slot)
    {
        // 이전 슬롯 색상 복구
        if(previousSelectedSlot != null)
        {
            previousSelectedSlot.ResetColor();
        }

        // 현재 슬롯을 이전 슬롯으로 저장
        previousSelectedSlot = _slot;

        // 선택된 퀘스트 저장
        selectedQuest = _quest;

        // 퀘스트 이름
        questNameText.text = _quest.questName;

        // 퀘스트 설명
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
        questTargetText.text = $"{_quest.targetNameKorean} {actionText} : 0 / {_quest.targetCount}";

        // 보상 정보
        questRewardGoldText.text = $"{_quest.rewardGold}";  // 골드텍스트 설정
        questRewardEXPText.text = $"{_quest.rewardExp}";    // 경험치 텍스트 설정

        // 상세 정보 패널 활성화
        questDetailPanel.SetActive(true);
    }
    
    // 수락 버튼 클릭 시
    public void OnAcceptButtonClick()
    {
        // UI_Quest (플레이어 퀘스트) 수락함수를 호출
        UI_Quest.instance.AcceptQuest(selectedQuest);
        // - 지금은 Debug.Log("퀘스트 수락: " + selectedQuest.questName)만 출력
        Debug.Log(selectedQuest.questName);
        // - 상세정보창 오브젝트 끄기
        questDetailPanel.gameObject.SetActive(false);
         // 이미 수락한 퀘스트를 다시 수락할수있기때문에 안보이게 가려놔야함
        RefreshQuestList();
    }
    // 거절 버튼 클릭 시
    public void OnDeclineButtonClick()
    {
        // 상세정보창 오브젝트 끄기
        questDetailPanel.gameObject.SetActive(false);
        // 내가 선택한 퀘스트도 초기화해서 리프레쉬 함수 호출
        RefreshQuestList();
        
    }

    public void CloseWindow()
    {
        // NPC퀘스트창 끄기
        gameObject.SetActive(false);
        // 선택된퀘스트 정보 비우기
        selectedQuest = null;
        // 상세정보창 비활성화
        questDetailPanel.SetActive(false);
    }

    void Update()
    {
        // ESC 키로 창 닫기
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseWindow();
        }

    }
}
