using System.IO;
using UnityEngine;

// =====================================================================
// PlayerLoadCSV : CSV 파일에서 플레이어 기본 데이터를 읽어오는 싱글톤
// =====================================================================
// [역할]
// - 플레이어 기본 스펙, 인벤토리, 스킬 등 CSV 데이터를 읽어서 반환
// - CreateSave() 호출 시 이 클래스에서 기본값을 받아 SaveData에 저장
//
// [새 게임 순서]
// 1. StartMenu.OnConfirmNewGame() → 새 게임 생성 요청
// 2. PlayerDataManager.CreateSave() → 세이브 파일 생성 시작
// 3. PlayerLoadCSV.Instance.LoadDefaultSpec() → CSV 기본값 읽기 (PlayerDefaultData.csv)
// 4. PlayerDataManager.WriteSaveFile() → SaveData를 암호화해서 .sav 파일로 저장
// 5. LoadingData.LoadScene(SceneName.TownScene) → 로딩씬 거쳐 타운씬 이동
// 6. 타운씬 GameManager.Awake() → PlayerDataManager.LoadGame()으로 .sav 읽어서 PlayerSpecs, PlayerProgression에 적용
//
// [불러오기 순서]
// 1. StartMenu.SelectSaveAndLoad() → 세이브 슬롯 선택
// 2. PlayerDataManager.SelectSave() → CurrentSaveName에 세이브 이름 저장
// 3. LoadingData.LoadScene(SceneName.TownScene) → 로딩씬 거쳐 타운씬 이동
// 4. 타운씬 GameManager.Awake() → PlayerDataManager.LoadGame()으로 .sav 읽어서 PlayerSpecs, PlayerProgression에 적용
//
// [함수 목록]
// - LoadDefaultSpec() : 플레이어 기본 스펙 CSV를 읽어서 SaveData에 채워 반환
// =====================================================================
public class PlayerLoadCSV : MonoBehaviour
{
    public static PlayerLoadCSV Instance { get; private set; } = null;

    string playerDefaultDataPath => Application.dataPath + "/Resources/PlayerDataPath/PlayerDefaultData.csv";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ------------------------------------------------------------------
    // 플레이어 기본 스펙 CSV를 읽어서 SaveData에 채워 반환
    // CreateSave()에서 호출하여 새 게임의 초기값을 설정합니다
    // ------------------------------------------------------------------
    public SaveData LoadDefaultSpec()
    {
        SaveData data = new SaveData();

        if (!File.Exists(playerDefaultDataPath))
        {
            Debug.LogError("PlayerDefaultData.csv 파일이 존재하지 않습니다 : " + playerDefaultDataPath);
            return data;
        }

        using (StreamReader sr = new StreamReader(playerDefaultDataPath))
        {
            sr.ReadLine(); // 첫 번째 줄(헤더) 건너뛰기

            string line = sr.ReadLine();
            if (string.IsNullOrEmpty(line)) return data;

            string[] col = line.Split(',');

            data.playerLevel  = int.Parse(col[0].Trim());
            data.maxLevel     = int.Parse(col[1].Trim());
            data.maxHp        = float.Parse(col[2].Trim());
            data.currentHp    = float.Parse(col[3].Trim());
            data.maxMp        = float.Parse(col[4].Trim());
            data.currentMp    = float.Parse(col[5].Trim());
            data.basicDamage  = float.Parse(col[6].Trim());
            data.basicDefense = float.Parse(col[7].Trim());
            data.maxExp       = float.Parse(col[8].Trim());
            data.currentExp   = float.Parse(col[9].Trim());
            data.skillPoint   = int.Parse(col[10].Trim());
        }

        return data;
    }
}
