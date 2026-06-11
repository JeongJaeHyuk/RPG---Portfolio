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

        // UI_Equip_Status는 Start()에서 플레이어를 찾지만,
        // ApplyData()가 Awake()에서 EquipWeapon을 호출하므로 지금 미리 주입
        if (UI_Equip_Status.Instance != null)
        {
            UI_Equip_Status.Instance.SetPlayerSpecs(playerSpecs);
        }

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
        if (data == null)
        {
            Debug.LogWarning("[Save] 세이브 파일 없음 — 저장 취소");
            return;
        }

        Debug.Log("===== [Save] 저장 시작 =====");

        // ── 스탯 ──────────────────────────────────────────────────────
        data.currentHp    = playerSpecs.CURRENT_HP;
        data.maxHp        = playerSpecs.MAX_HP;
        data.currentMp    = playerSpecs.CURRENT_MP;
        data.maxMp        = playerSpecs.MAX_MP;
        data.basicDamage  = playerSpecs.BASICDAMAGE;
        data.basicDefense = playerSpecs.BASICDEFENSE;
        Debug.Log($"[Save] HP:{data.currentHp}/{data.maxHp}  MP:{data.currentMp}/{data.maxMp}  ATK:{data.basicDamage}  DEF:{data.basicDefense}");

        // ── 레벨 / 경험치 ──────────────────────────────────────────────
        data.playerLevel = playerProgression.CURRENT_LEVEL;
        data.maxLevel    = playerProgression.MAX_LEVEL;
        data.currentExp  = playerProgression.CURRENT_EXP;
        data.maxExp      = playerProgression.MAX_EXP;
        data.skillPoint  = playerProgression.SKILL_POINT;
        Debug.Log($"[Save] Lv:{data.playerLevel}/{data.maxLevel}  EXP:{data.currentExp}/{data.maxExp}  SP:{data.skillPoint}");

        // ── 위치 ──────────────────────────────────────────────────────
        if (player != null)
        {
            data.posX = player.transform.position.x;
            data.posY = player.transform.position.y;
            data.posZ = player.transform.position.z;
        }
        Debug.Log($"[Save] 위치:({data.posX:F1}, {data.posY:F1}, {data.posZ:F1})");

        // ── 장착 아이템 ───────────────────────────────────────────────
        data.equippedWeaponId = -1;
        data.equippedHeadId   = -1;
        data.equippedChestId  = -1;

        Item savedWeapon = null;
        Item savedHead   = null;
        Item savedChest  = null;

        if (UI_Equip_Status.Instance != null)
        {
            savedWeapon = UI_Equip_Status.Instance.GetEquippedWeapon();
            savedHead   = UI_Equip_Status.Instance.GetEquippedArmor(ArmorType.Head);
            savedChest  = UI_Equip_Status.Instance.GetEquippedArmor(ArmorType.Chest);

            data.equippedWeaponId = savedWeapon != null ? savedWeapon.itemId : -1;
            data.equippedHeadId   = savedHead   != null ? savedHead.itemId   : -1;
            data.equippedChestId  = savedChest  != null ? savedChest.itemId  : -1;
        }
        Debug.Log($"[Save] 장착 — 무기ID:{data.equippedWeaponId}  머리ID:{data.equippedHeadId}  가슴ID:{data.equippedChestId}");

        // ── 골드 / 인벤토리 ───────────────────────────────────────────
        if (UI_Inventory.inven != null)
        {
            data.gold = UI_Inventory.inven.GOLD;
            Debug.Log($"[Save] 골드:{data.gold}");

            data.inventory.Clear();

            foreach (Item item in UI_Inventory.inven.EQUIPLIST)
            {
                data.inventory.Add(new ItemSaveData { itemId = item.itemId, count = item.currentCount, itemCategory = item.type });
                Debug.Log($"[Save]  인벤(장비) id:{item.itemId} {item.itemName} x{item.currentCount}");
            }
            foreach (Item item in UI_Inventory.inven.CONSUMELIST)
            {
                data.inventory.Add(new ItemSaveData { itemId = item.itemId, count = item.currentCount, itemCategory = item.type });
                Debug.Log($"[Save]  인벤(소비) id:{item.itemId} {item.itemName} x{item.currentCount}");
            }
            foreach (Item item in UI_Inventory.inven.MATERIALLIST)
            {
                data.inventory.Add(new ItemSaveData { itemId = item.itemId, count = item.currentCount, itemCategory = item.type });
                Debug.Log($"[Save]  인벤(재료) id:{item.itemId} {item.itemName} x{item.currentCount}");
            }

            // 장착 중인 아이템은 EQUIPLIST에서 빠져있으므로 명시적으로 추가
            if (savedWeapon != null)
            {
                data.inventory.Add(new ItemSaveData { itemId = savedWeapon.itemId, count = savedWeapon.currentCount, itemCategory = savedWeapon.type });
                Debug.Log($"[Save]  장착무기 인벤포함 id:{savedWeapon.itemId} {savedWeapon.itemName}");
            }
            if (savedHead != null)
            {
                data.inventory.Add(new ItemSaveData { itemId = savedHead.itemId, count = savedHead.currentCount, itemCategory = savedHead.type });
                Debug.Log($"[Save]  장착머리 인벤포함 id:{savedHead.itemId} {savedHead.itemName}");
            }
            if (savedChest != null)
            {
                data.inventory.Add(new ItemSaveData { itemId = savedChest.itemId, count = savedChest.currentCount, itemCategory = savedChest.type });
                Debug.Log($"[Save]  장착가슴 인벤포함 id:{savedChest.itemId} {savedChest.itemName}");
            }
        }
        else
        {
            Debug.LogWarning("[Save] UI_Inventory.inven == null — 인벤토리 저장 안됨");
        }

        // ── 스킬 ──────────────────────────────────────────────────────
        data.skills.Clear();
        foreach (Skill skill in Skill_Manager.Instance.GetAllSkills())
        {
            data.skills.Add(new SkillSaveData { skillName = skill.skillName, currentLevel = skill.CURRENT_LEVEL });
            Debug.Log($"[Save]  스킬:{skill.skillName}  Lv:{skill.CURRENT_LEVEL}");
        }

        PlayerDataManager.SaveGame(data);
        Debug.Log("===== [Save] 저장 완료 =====");
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
        Debug.Log("===== [Load] 로드 시작 =====");

        // PlayerProgression 적용 (MAX 먼저, CURRENT 나중)
        playerProgression.MAX_LEVEL     = data.maxLevel;
        playerProgression.MAX_EXP       = data.maxExp;
        playerProgression.CURRENT_LEVEL = data.playerLevel;
        playerProgression.CURRENT_EXP   = data.currentExp;
        playerProgression.SKILL_POINT   = data.skillPoint;
        Debug.Log($"[Load] Lv:{playerProgression.CURRENT_LEVEL}/{playerProgression.MAX_LEVEL}  EXP:{playerProgression.CURRENT_EXP}/{playerProgression.MAX_EXP}  SP:{playerProgression.SKILL_POINT}");

        // PlayerSpecs 적용 (MAX 먼저, CURRENT 나중)
        playerSpecs.MAX_HP       = data.maxHp;
        playerSpecs.MAX_MP       = data.maxMp;
        playerSpecs.BASICDAMAGE  = data.basicDamage;
        playerSpecs.BASICDEFENSE = data.basicDefense;
        playerSpecs.CURRENT_HP   = data.currentHp;
        playerSpecs.CURRENT_MP   = data.currentMp;
        Debug.Log($"[Load] HP:{playerSpecs.CURRENT_HP}/{playerSpecs.MAX_HP}  MP:{playerSpecs.CURRENT_MP}/{playerSpecs.MAX_MP}  ATK:{playerSpecs.BASICDAMAGE}  DEF:{playerSpecs.BASICDEFENSE}");

        // ── 스킬 ──────────────────────────────────────────────────────
        if (data.skills != null && data.skills.Count > 0)
        {
            foreach (SkillSaveData savedSkill in data.skills)
            {
                Skill skill = Skill_Manager.Instance.GetAllSkills().Find(s => s.skillName == savedSkill.skillName);
                if (skill != null)
                {
                    Skill_Database db = Skill_Manager.Instance.GetSkillData().Find(d => d.skill_Id == savedSkill.skillName);
                    if (db != null)
                    {
                        skill.MAX_LEVEL = db.maxLevel;
                    }
                    skill.CURRENT_LEVEL = savedSkill.currentLevel;
                    Debug.Log($"[Load]  스킬:{skill.skillName}  Lv:{skill.CURRENT_LEVEL}");
                }
                else
                {
                    Debug.LogWarning($"[Load]  스킬 못찾음:{savedSkill.skillName}");
                }
            }
        }
        else
        {
            Debug.LogWarning("[Load] 저장된 스킬 데이터 없음");
        }

        // ── 인벤토리 ──────────────────────────────────────────────────
        if (UI_Inventory.inven != null)
        {
            UI_Inventory.inven.GOLD = data.gold;
            Debug.Log($"[Load] 골드:{data.gold}");

            if (data.inventory != null && data.inventory.Count > 0 && Item_Manager.Instance != null)
            {
                foreach (ItemSaveData itemData in data.inventory)
                {
                    Item original = Item_Manager.Instance.GetItemById(itemData.itemId);
                    if (original == null)
                    {
                        Debug.LogWarning($"[Load]  아이템 못찾음 id:{itemData.itemId}");
                        continue;
                    }

                    Item copy = new Item(original);
                    copy.currentCount = itemData.count;

                    switch (itemData.itemCategory)
                    {
                        case ItemType.Weapon:
                        case ItemType.Armor:
                            UI_Inventory.inven.EQUIPLIST.Add(copy);
                            Debug.Log($"[Load]  인벤(장비) id:{copy.itemId} {copy.itemName} x{copy.currentCount}");
                            break;
                        case ItemType.Consumable:
                            UI_Inventory.inven.CONSUMELIST.Add(copy);
                            Debug.Log($"[Load]  인벤(소비) id:{copy.itemId} {copy.itemName} x{copy.currentCount}");
                            break;
                        case ItemType.Material:
                            UI_Inventory.inven.MATERIALLIST.Add(copy);
                            Debug.Log($"[Load]  인벤(재료) id:{copy.itemId} {copy.itemName} x{copy.currentCount}");
                            break;
                    }
                }
            }
            else
            {
                Debug.LogWarning("[Load] 저장된 인벤토리 없음");
            }
        }
        else
        {
            Debug.LogWarning("[Load] UI_Inventory.inven == null — 인벤토리 로드 안됨");
        }

        // ── 장착 아이템 복원 ──────────────────────────────────────────
        Debug.Log($"[Load] 장착복원 시도 — 무기ID:{data.equippedWeaponId}  머리ID:{data.equippedHeadId}  가슴ID:{data.equippedChestId}");
        if (UI_Equip_Status.Instance != null && UI_Inventory.inven != null)
        {
            if (data.equippedWeaponId != -1)
            {
                Item weapon = UI_Inventory.inven.EQUIPLIST.Find(i => i.itemId == data.equippedWeaponId);
                if (weapon != null)
                {
                    UI_Equip_Status.Instance.EquipWeapon(weapon);
                    Debug.Log($"[Load]  무기 장착됨:{weapon.itemName}");
                }
                else
                {
                    Debug.LogWarning($"[Load]  무기 EQUIPLIST에 없음 id:{data.equippedWeaponId}");
                }
            }
            if (data.equippedHeadId != -1)
            {
                Item head = UI_Inventory.inven.EQUIPLIST.Find(i => i.itemId == data.equippedHeadId);
                if (head != null)
                {
                    UI_Equip_Status.Instance.EquipArmor(head);
                    Debug.Log($"[Load]  머리 장착됨:{head.itemName}");
                }
                else
                {
                    Debug.LogWarning($"[Load]  머리 EQUIPLIST에 없음 id:{data.equippedHeadId}");
                }
            }
            if (data.equippedChestId != -1)
            {
                Item chest = UI_Inventory.inven.EQUIPLIST.Find(i => i.itemId == data.equippedChestId);
                if (chest != null)
                {
                    UI_Equip_Status.Instance.EquipArmor(chest);
                    Debug.Log($"[Load]  가슴 장착됨:{chest.itemName}");
                }
                else
                {
                    Debug.LogWarning($"[Load]  가슴 EQUIPLIST에 없음 id:{data.equippedChestId}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"[Load] 장착복원 불가 — UI_Equip_Status:{UI_Equip_Status.Instance != null}  UI_Inventory:{UI_Inventory.inven != null}");
        }

        Debug.Log("===== [Load] 로드 완료 =====");
    }
}
