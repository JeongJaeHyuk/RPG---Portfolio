using UnityEngine;

// =====================================================================
// PlayerResourceManager : 플레이어 프리팹 정보를 보관하고 생성하는 싱글톤
// =====================================================================
// [역할]
// - 플레이어 프리팹 참조 보관
// - 씬 진입 시 지정된 위치에 플레이어 생성
// - PlayerManager에서 호출하여 사용
// =====================================================================
public class PlayerResourceManager : MonoBehaviour
{
    public static PlayerResourceManager Instance { get; private set; } = null;

    [Header("── 플레이어 프리팹 ──────────────")]
    [SerializeField] GameObject playerPrefab;   // 생성할 플레이어 프리팹

    [Header("── 스폰 위치 ──────────────────")]
    [SerializeField] Transform spawnPoint;      // 씬에 배치된 스폰 위치 오브젝트
    [SerializeField] Vector3 defaultSpawnPos;   // 스폰 오브젝트 없을 때 사용할 기본 위치

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ------------------------------------------------------------------
    // 플레이어 프리팹을 스폰 위치에 생성하고 반환
    // PlayerManager.Awake()에서 호출합니다
    // ------------------------------------------------------------------
    public GameObject SpawnPlayer()
    {
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : defaultSpawnPos;
        return Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }
}
