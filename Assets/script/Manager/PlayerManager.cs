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
    PlayerSpecs playerSpecs;
    PlayerProgression playerProgression;

    void Awake()
    {
        // 1. 플레이어 프리팹 생성
        GameObject player = PlayerResourceManager.Instance.SpawnPlayer();

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
    }
}
