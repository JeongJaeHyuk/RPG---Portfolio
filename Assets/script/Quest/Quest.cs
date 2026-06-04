using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Kill,       // 처치
    Collect,    // 수집
    Visit,      // 방문
    Boss        // 보스
}

public enum QuestStatus
{
    InProgress, // 진행 중
    Completed   // 완료됨
}

[System.Serializable]
public class Quest
{
    public int questId;             // 퀘스트 고유 ID
    public string questName;        // 퀘스트 이름
    public int npcId;               // 퀘스트 제공 NPC ID
    public QuestType questType;     // 퀘스트 타입 (처치, 수집, 방문, 보스)
    public bool isRepeatable;       // 반복 가능 여부
    public int requiredLevel;       // 필요 레벨
    public int preQuestId;          // 선행 퀘스트 ID (0이면 없음)
    public string description;      // 퀘스트 설명
    public string targetType;       // 목표 대상 (몬스터/아이템/NPC 영문 이름)
    public string targetNameKorean; // 목표 대상 한글 이름
    public int targetCount;         // 목표 개수
    public int rewardExp;           // 경험치 보상
    public int rewardGold;          // 골드 보상
    public int rewardItemId;        // 보상 아이템 ID (0이면 없음)
    public int rewardItemCount;     // 보상 아이템 개수

    // 퀘스트 진행도 (런타임에 사용)
    private int currentProgress;   // 현재 진행도 (5마리 중 3마리 등)
    public QuestStatus status;     // 퀘스트 상태 (진행 중, 완료됨)

    public int CURRENTPROGRESS
    {
        get => currentProgress;
        set
        {
            if(currentProgress != value)  // 값이 실제로 변경될 때만 이벤트 발생
            {
                currentProgress = value;
                OnProgressChanged?.Invoke(this);  // 진행도 변경 시 이벤트 발생
            }
        }
    }

    // 진행도 변경 이벤트
    public event System.Action<Quest> OnProgressChanged;

    // 기본 생성자
    public Quest()
    {
        currentProgress = 0;
    }

    // 복사 생성자 (퀘스트 수락 시 원본 복사용)
    public Quest(Quest other)
    {
        this.questId = other.questId;
        this.questName = other.questName;
        this.npcId = other.npcId;
        this.questType = other.questType;
        this.isRepeatable = other.isRepeatable;
        this.requiredLevel = other.requiredLevel;
        this.preQuestId = other.preQuestId;
        this.description = other.description;
        this.targetType = other.targetType;
        this.targetNameKorean = other.targetNameKorean;
        this.targetCount = other.targetCount;
        this.rewardExp = other.rewardExp;
        this.rewardGold = other.rewardGold;
        this.rewardItemId = other.rewardItemId;
        this.rewardItemCount = other.rewardItemCount;
        this.currentProgress = 0;  // 새로 수락하면 진행도 0 (직접 필드 접근, 이벤트 발생 안 함)
        this.status = QuestStatus.InProgress;  // 수락하면 진행 중 상태
    }

    // 퀘스트 완료 여부 확인
    public bool IsCompleted()
    {
        return CURRENTPROGRESS >= targetCount;
    }

    // 진행도 증가
    public void AddProgress(int amount = 1)
    {
        CURRENTPROGRESS += amount;  // 프로퍼티 사용 -> 자동으로 이벤트 발생
        if(CURRENTPROGRESS > targetCount)
        {
            CURRENTPROGRESS = targetCount;
        }
    }
}
