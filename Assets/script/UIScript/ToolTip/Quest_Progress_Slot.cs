using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quest_Progress_Slot : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] Text questName;                // 퀘스트 제목 (예: 암석거인 처지, 돌맹이 수집)
    [SerializeField] Text progressText;             // 진행도 텍스트 (예: "암석거인 처치 3/5")
    string actionText = string.Empty;
    Quest currentQuest;                             // 현재 표시 중인 퀘스트

    public void SetQuest(Quest _quest)
    {
        // 퀘스트 설정 (Quest_Progress_Popup의 AddTrackedQuest에서 호출)
        currentQuest = _quest;
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
        // 이벤트 구독 (진행도 변경 시 자동으로 UpdateProgress 호출됨)
        currentQuest.OnProgressChanged += UpdateProgress;

        // 초기 진행도 표시
        progressText.text = $"{currentQuest.targetNameKorean} {actionText} : {currentQuest.CURRENTPROGRESS} / {currentQuest.targetCount}";
        progressText.color = Color.white;
        questName.text = currentQuest.questName;
        // 슬롯 활성화는 Quest_Progress_Popup에서 함
    }


    // 구독해야할 함수 (Quest 파라미터 필요)
    public void UpdateProgress(Quest _quest)
    {
        progressText.text = $"{_quest.targetNameKorean} {actionText} : {_quest.CURRENTPROGRESS} / {_quest.targetCount}";
        // 퀘스트의 목표 달성시
        if(_quest.CURRENTPROGRESS >= _quest.targetCount)
        {
            // 목표 이상 카운트 안올라가게 방어
            _quest.CURRENTPROGRESS = _quest.targetCount;
            // 텍스트 색깔을 초록색으로 표시
            progressText.color = Color.green;
        }
    }

    public void Clear()
    {
        // 슬롯 초기화 (Quest_Progress_Popup의 RemoveTrackedQuest에서 호출)
        // 이벤트 구독 해제
        if(currentQuest != null)
        {
            currentQuest.OnProgressChanged -= UpdateProgress;
        }

        currentQuest = null;
        questName.text = string.Empty;
        progressText.text = string.Empty;
        actionText = string.Empty;
        // 슬롯 비활성화는 Quest_Progress_Popup에서 함
    }

    public Quest GetQuest()
    {
        // TODO: 현재 퀘스트 반환
        return currentQuest;
    }
}
