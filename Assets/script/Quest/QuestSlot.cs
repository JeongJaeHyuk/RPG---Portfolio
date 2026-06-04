using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestSlot : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] Text questNameText;     // 퀘스트 이름 텍스트
    [SerializeField] Image bgImage;          // 배경 이미지 (선택 시 색 변경용)
    Color originalColor = new Color (0.64f,0.64f,0.64f);

    Quest quest;  // 이 슬롯이 담고 있는 퀘스트 데이터

    public void SetQuest(Quest _quest)
    {
        // TODO: 퀘스트 슬롯에 데이터 설정
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
    }

    public void OnSlotClick()
    {
        // 슬롯 클릭 시 오른쪽 상세 정보 패널에 이 퀘스트 정보 표시
        UI_QuestWindow.instance.SelectQuest(quest, this);  // 자기 자신(this)을 넘김

        // 선택된 색상으로 변경
        bgImage.color = new Color(1, 0, 0);
    }

    public void ResetColor()
    {
        // 원래 색상으로 복구
        bgImage.color = originalColor;
    }

    public void Clear()
    {
        // 슬롯 초기화
        quest = null;
        questNameText.text = string.Empty;
        // 배경 색 초기화
        bgImage.color = originalColor;
    }
}
