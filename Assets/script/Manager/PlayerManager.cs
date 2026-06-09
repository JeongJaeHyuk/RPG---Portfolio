using UnityEngine;

// =====================================================================
// PlayerManager : 씬 진입 시 플레이어를 생성하고 데이터를 적용하는 스크립트
// =====================================================================
// [역할]
// - 게임플레이 씬(TownScene, OrcFieldScene, RaidScene) 진입 시
//   1. PlayerResourceManager에서 플레이어 프리팹 생성
//   2. PlayerDataManager.LoadGame()으로 .sav 파일을 읽어서
//      PlayerSpecs, PlayerProgression에 데이터 적용
//
// [주의사항]
// - MAX_EXP, MAX_LEVEL을 반드시 먼저 대입한 뒤 CURRENT_EXP, CURRENT_LEVEL 대입
//   (프로퍼티 Invoke 때문에 순서가 중요)
// - MAX_HP, MAX_MP도 반드시 먼저 대입 후 CURRENT_HP, CURRENT_MP 대입
// =====================================================================
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; } = null;

    PlayerSpecs playerSpecs;
    PlayerProgression playerProgression;
    GameObject player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // 1. 플레이어 프리팹 생성
        player = PlayerResourceManager.Instance.SpawnPlayer();

        // 생성된 플레이어에서 컴포넌트 가져오기
        playerSpecs       = player.GetComponent<PlayerSpecs>();
        playerProgression = player.GetComponent<PlayerProgression>();

        // 2. 세이브 데이터 로드
        SaveData data = PlayerDataManager.LoadGame();

        if (data == null)
        {
            Debug.LogError("[PlayerManager] 세이브 데이터를 불러오지 못했습니다.");
            return;
        }

        // 3. 데이터 적용
        ApplyData(data);
    }

    // ------------------------------------------------------------------
    // 현재 게임 상태를 SaveData로 수집 후 저장 (씬 이동 전 / 게임 종료 시 호출)
    // ------------------------------------------------------------------
    public void SaveCurrentData()
    {
        SaveData data = PlayerDataManager.LoadGame();
        if (data == null) return;

        // ── 스탯 ──────────────────────────────────────────────────────
        data.currentHp    = playerSpecs.CURRENT_HP;
        data.maxHp        = playerSpecs.MAX_HP;
        data.currentMp    = playerSpecs.CURRENT_MP;
        data.maxMp        = playerSpecs.MAX_MP;
        data.basicDamage  = playerSpecs.BASICDAMAGE;
        data.basicDefense = playerSpecs.BASICDEFENSE;

        // ── 레벨 / 경험치 ──────────────────────────────────────────────
        data.playerLevel = playerProgression.CURRENT_LEVEL;
        data.maxLevel    = playerProgression.MAX_LEVEL;
        data.currentExp  = playerProgression.CURRENT_EXP;
        data.maxExp      = playerProgression.MAX_EXP;
        data.skillPoint  = playerProgression.SKILL_POINT;

        // ── 위치 ──────────────────────────────────────────────────────
        if (player != null)
        {
            data.posX = player.transform.position.x;
            data.posY = player.transform.position.y;
            data.posZ = player.transform.position.z;
        }

        // ── 골드 / 인벤토리 ───────────────────────────────────────────
        // 아이템 전체 정보를 저장하지 않고 itemId + count + category 만 저장
        // 불러올 때 itemId로 Item_Manager에서 원본 정보를 다시 조회하는 방식 (DB 참조 방식)
        if (UI_Inventory.inven != null)
        {
            data.gold = UI_Inventory.inven.GOLD;

            // 이전 저장 내용을 비우고 현재 인벤토리 상태로 덮어씀
            data.inventory.Clear();
            foreach (Item item in UI_Inventory.inven.EQUIPLIST)
                data.inventory.Add(new ItemSaveData { itemId = item.itemId, count = item.currentCount, itemCategory = item.type });
            foreach (Item item in UI_Inventory.inven.CONSUMELIST)
                data.inventory.Add(new ItemSaveData { itemId = item.itemId, count = item.currentCount, itemCategory = item.type });
            foreach (Item item in UI_Inventory.inven.MATERIALLIST)
                data.inventory.Add(new ItemSaveData { itemId = item.itemId, count = item.currentCount, itemCategory = item.type });
        }

        // ── 스킬 ──────────────────────────────────────────────────────
        data.skills.Clear();
        foreach (Skill skill in Skill_Manager.Instance.GetAllSkills())
        {
            data.skills.Add(new SkillSaveData
            {
                skillName    = skill.skillName,
                currentLevel = skill.CURRENT_LEVEL
            });
        }

        PlayerDataManager.SaveGame(data);
    }

    // ------------------------------------------------------------------
    // 게임 종료 시 자동 저장
    // ------------------------------------------------------------------
    void OnApplicationQuit()
    {
        SaveCurrentData();
    }

    // ------------------------------------------------------------------
    // SaveData를 PlayerSpecs, PlayerProgression에 적용
    // ------------------------------------------------------------------
    void ApplyData(SaveData data)
    {
        // PlayerProgression 적용 (MAX 먼저, CURRENT 나중)
        playerProgression.MAX_LEVEL     = data.maxLevel;
        playerProgression.MAX_EXP       = data.maxExp;
        playerProgression.CURRENT_LEVEL = data.playerLevel;
        playerProgression.CURRENT_EXP   = data.currentExp;
        playerProgression.SKILL_POINT   = data.skillPoint;

        // PlayerSpecs 적용 (MAX 먼저, CURRENT 나중)
        playerSpecs.MAX_HP       = data.maxHp;
        playerSpecs.MAX_MP       = data.maxMp;
        playerSpecs.BASICDAMAGE  = data.basicDamage;
        playerSpecs.BASICDEFENSE = data.basicDefense;
        playerSpecs.CURRENT_HP   = data.currentHp;
        playerSpecs.CURRENT_MP   = data.currentMp;

        // ── 스킬 ──────────────────────────────────────────────────────
        if (data.skills != null && data.skills.Count > 0)
        {
            // .sav에 저장된 레벨로 덮어씌우기
            foreach (SkillSaveData savedSkill in data.skills)
            {
                Skill skill = Skill_Manager.Instance.GetAllSkills().Find(s => s.skillName == savedSkill.skillName);
                if (skill != null)
                    skill.CURRENT_LEVEL = savedSkill.currentLevel;
                Debug.Log("저장된 스킬레벨 = " +  savedSkill.currentLevel);
                Debug.Log("불러온 스킬레벨 = " + skill.CURRENT_LEVEL);
            }
        }
        // data.skills가 null이면 CSV 기본값(레벨 0) 그대로 유지

        // ── 인벤토리 ──────────────────────────────────────────────────
        if (UI_Inventory.inven != null)
        {
            UI_Inventory.inven.GOLD = data.gold;

            // itemId로 Item_Manager에서 원본 아이템 정보를 찾아 복사본 생성 후 인벤토리에 추가
            // AddItemInventory() 대신 리스트에 직접 추가하는 이유:
            // AddItemInventory()는 퀘스트 이벤트를 발생시켜 로드 중 퀘스트가 오작동할 수 있음
            if (data.inventory != null && data.inventory.Count > 0 && Item_Manager.Instance != null)
            {
                foreach (ItemSaveData itemData in data.inventory)
                {
                    // itemId로 CSV에서 불러온 원본 아이템 조회
                    Item original = Item_Manager.Instance.GetItemById(itemData.itemId);
                    if (original == null) continue;

                    // 원본을 복사하고 저장된 개수를 덮어씀
                    Item copy = new Item(original);
                    copy.currentCount = itemData.count;

                    // itemCategory에 따라 맞는 리스트에 직접 추가
                    switch (itemData.itemCategory)
                    {
                        case ItemType.Weapon:
                        case ItemType.Armor:      UI_Inventory.inven.EQUIPLIST.Add(copy);    break;
                        case ItemType.Consumable: UI_Inventory.inven.CONSUMELIST.Add(copy);  break;
                        case ItemType.Material:   UI_Inventory.inven.MATERIALLIST.Add(copy); break;
                    }
                }
            }
        }
    }
}
