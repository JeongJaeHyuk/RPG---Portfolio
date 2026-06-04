using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Quest_Slot : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] Text questNameText;        // 퀘스트 이름 텍스트
    [SerializeField] Text questProgressText;    // 진행도 텍스트 (예: "3/5")
    [SerializeField] Image bgImage;             // 배경 이미지 (선택 시 색 변경용)
    Color originalColor = new Color (0.64f,0.64f,0.64f);    // 배경이미지 초기값
    [Header("팝업 전용")]
    [SerializeField] Image check;
    bool IsPopUpCheck = false;

    Quest quest;  // 이 슬롯이 담고 있는 퀘스트 데이터

    // 퀘스트 슬롯에 데이터 설정
    public void SetQuest(Quest _quest)
    {
        // 슬롯에 Quest데이터를 참조
        quest = _quest;
        // - questNameText.text = quest.questName 설정
        // - 반복 퀘스트면 "[반복]" 같은 표시 추가해도 좋음 _quest.questType 사용
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
        questNameText.text = $"[{actionText}]{_quest.questName}";
        // 처음 배경이미지 설정
        bgImage.color = originalColor;

        // 팝업 체크 상태 복원 (이미 추적 중인지 확인)
        if(Quest_Progress_Popup.instance.IsTracking(_quest))
        {
            // 이미 추적 중이면 체크 표시 유지
            check.color = new Color(1,1,1,1);
            IsPopUpCheck = true;
        }
        else
        {
            // 추적 중이 아니면 체크 해제
            check.color = new Color(1,1,1,0);
            IsPopUpCheck = false;
        }
    }

    public void UpdateProgress()
    {
        // TODO: 진행도 업데이트
        // - questProgressText.text 업데이트 (quest.currentProgress / quest.targetCount)
    }

    public void OnSlotClick()
    {
        // 슬롯 클릭 시
        UI_Quest.instance.SelectQuest(quest, this);
        // 선택된 색상으로 변경 (빨간색 등)
        bgImage.color = new Color(1, 0, 0);
    }
    // 슬롯 옆에있는 체크 버튼 누르면 팝업에 설정 한번 더누르면 해제
    public void SelectedPopUpClick()
    {
        IsPopUpCheck = !IsPopUpCheck;
        if(IsPopUpCheck)
        {
            // 체크 활성화 - 퀘스트 추적 등록
            if (Quest_Progress_Popup.instance.QuestCheck())
            {
                // TODO: 퀘스트 추적 추가
                Quest_Progress_Popup.instance.AddTrackedQuest(quest);
                check.color = new Color(1,1,1,1);
                Debug.Log("팝업에 보내줌");
            }
            else
            {
                // 슬롯이 꽉 찼을 경우
                IsPopUpCheck = false;
                check.color = new Color(1,1,1,0);
            }
        }
         // 체크 비활성화 - 퀘스트 추적 해제
        else
        {
            // 체크 비활성화 - 퀘스트 추적 해제
            // TODO: 퀘스트 추적 제거
            Quest_Progress_Popup.instance.RemoveTrackedQuest(quest);
            check.color = new Color(1,1,1,0);
            Debug.Log("팝업에서 제거");
        }
    }
    public void ResetColor()
    {
        // 원래 색상으로 복구
        bgImage.color = originalColor;
    }

    // 슬롯 초기화
    public void Clear()
    {
        // 참조한 Quest정보 초기화 (비우기)
        quest = null;
        // 제목 초기화
        questNameText.text = string.Empty;
        // 배경 색 초기화
        bgImage.color = originalColor;
    }
}
