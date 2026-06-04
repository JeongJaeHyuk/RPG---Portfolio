using System;
using System.Collections.Generic;

// =====================================================================
// SaveData : 세이브 파일 하나에 저장되는 모든 데이터를 담는 클래스
// =====================================================================
// [직렬화(Serialization)란?]
// C# 클래스(메모리 안의 데이터)를 파일로 저장하려면 "텍스트 or 바이트"로
// 변환해야 하는데, 이 변환 과정을 직렬화라고 합니다.
// JsonUtility.ToJson(this) 를 호출하면 이 클래스가 JSON 문자열로 변환됩니다.
// [Serializable] 어트리뷰트가 없으면 JsonUtility가 변환을 거부합니다.
// =====================================================================
[Serializable]
public class SaveData
{
    // ------------------------------------------------------------------
    // 슬롯 목록 화면에 표시되는 미리보기 정보
    // (실제 게임 데이터와 별개로 "어떤 파일인지" 식별하는 메타데이터)
    // ------------------------------------------------------------------
    public string saveName;     // 플레이어가 직접 입력한 세이브 이름 (파일 이름으로도 사용)
    public string saveDate;     // 마지막 저장 시각 (yyyy-MM-dd HH:mm:ss 형식)
    public int playerLevel;     // 저장 시점의 플레이어 레벨 (목록에 Lv.5 형태로 표시용)
    public int maxLevel;        // 최대 레벨

    // ------------------------------------------------------------------
    // 실제 플레이어 진행 데이터
    // 게임씬에서 SaveGame() 호출 시 아래 값들을 채워서 저장합니다
    // 불러오기 시 아래 값들을 읽어서 각 스크립트(PlayerSpecs 등)에 적용합니다
    // ------------------------------------------------------------------

    // 플레이어 위치 (다음 게임 시작 시 이 위치에서 스폰)
    public float posX;
    public float posY;
    public float posZ;

    // 플레이어 스탯 (PlayerSpecs 스크립트와 연동)
    public float currentHp;
    public float maxHp;
    public float currentMp;
    public float maxMp;
    public float basicDamage;
    public float basicDefense;

    // 플레이어 성장 (PlayerProgression 스크립트와 연동)
    public float currentExp;
    public float maxExp;
    public float currentLevel;
    public float skillPoint;    // 남은 스킬 포인트

    // 골드 (UI_Inventory 스크립트와 연동)
    public int gold;

    // ------------------------------------------------------------------
    // 인벤토리 데이터
    // Dictionary는 JsonUtility가 직렬화 못하므로 List로 저장합니다
    // ItemSaveData는 아이템 ID + 개수만 저장 → 로드 시 Item_Data_Manager에서
    // ID로 아이템 전체 정보를 다시 불러옵니다 (DB에서 조회하는 개념)
    // ------------------------------------------------------------------
    public List<ItemSaveData> inventory = new List<ItemSaveData>();

    // ------------------------------------------------------------------
    // 퀘스트 데이터
    // ------------------------------------------------------------------
    public List<int> completedQuestIds = new List<int>();       // 완료한 퀘스트 ID 목록
    public List<QuestSaveData> activeQuests = new List<QuestSaveData>(); // 진행 중인 퀘스트 목록
}

// =====================================================================
// ItemSaveData : 인벤토리 아이템 하나의 저장 정보
// =====================================================================
// 아이템의 모든 정보(이름, 설명, 스탯 등)를 다 저장하지 않습니다.
// itemId(고유번호)와 count(개수)만 저장하고,
// 게임 로드 시 itemId로 Item_Data_Manager(CSV DB)에서 나머지 정보를 조회합니다.
// → DB 설계의 핵심 개념: 중복 저장 없이 ID로 참조
// =====================================================================
[Serializable]
public class ItemSaveData
{
    public int itemId;          // 아이템 고유 ID (Item_Data_Manager CSV의 itemId와 일치해야 함)
    public int count;           // 보유 개수
    public string itemCategory; // 인벤토리 분류용 ("Equip" / "Consume" / "Material")
}

// =====================================================================
// QuestSaveData : 진행 중인 퀘스트 하나의 저장 정보
// =====================================================================
// 퀘스트 전체 정보는 Quest_Data_Manager(CSV)에 있으므로 questId만 저장하고,
// 로드 시 questId로 퀘스트 정보를 다시 조회합니다.
// 진행도(currentProgress)는 플레이어마다 다르므로 같이 저장합니다.
// =====================================================================
[Serializable]
public class QuestSaveData
{
    public int questId;         // 퀘스트 고유 ID (Quest_Data_Manager CSV의 questId와 일치)
    public int currentProgress; // 현재 진행도 (몇 마리 처치했는지 등)
}
