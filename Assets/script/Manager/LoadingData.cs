using UnityEngine;
using UnityEngine.SceneManagement;

// =====================================================================
// LoadingData : 씬 전환 정보를 담는 싱글톤 (씬이 바뀌어도 파괴되지 않음)
// =====================================================================
// [역할]
// - 이동할 씬 이름을 저장하는 데이터 전달자
// - 실제 로딩 처리는 하지 않고 오직 정보 보관만 담당
//
// [사용 방법]
// 씬 이동이 필요한 곳에서 아래처럼 호출합니다
//   LoadingData.LoadScene(SceneName.TownScene);
//
// [씬 추가 방법]
// 1. SceneName enum에 새 씬 이름을 추가합니다
// 2. Unity Build Settings에 해당 씬을 등록합니다
// =====================================================================

// ------------------------------------------------------------------
// 프로젝트에 존재하는 씬 목록
// enum 이름과 Build Settings에 등록된 씬 파일명이 정확히 일치해야 합니다
// ------------------------------------------------------------------
public enum SceneName
{
    StartScene    = 0,
    LoadingScene  = 1,
    TownScene     = 2,
    OrcFieldScene = 3,
    RaidScene     = 4
}

public class LoadingData : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static LoadingData Instance { get; private set; } = null;

    // 다음에 이동할 씬 이름 (LoadingSceneManager가 읽어서 사용)
    public static string TargetSceneName { get; private set; }

    // 로딩씬 (Build Settings에 등록된 이름과 일치해야 함)
    private const string LOADING_SCENE_NAME = "LoadingScene";

    void Awake()
    {
        // 이미 인스턴스가 존재하면 중복 생성 방지
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 씬이 바뀌어도 이 오브젝트는 파괴되지 않음
        DontDestroyOnLoad(gameObject);
    }

    // ------------------------------------------------------------------
    // 씬 전환 요청
    // SceneName enum을 받아서 로딩씬으로 이동합니다
    // 예) LoadingData.LoadScene(SceneName.TownScene);
    // ------------------------------------------------------------------
    public static void LoadScene(SceneName sceneName)
    {
        // enum 이름을 그대로 씬 이름 문자열로 변환
        TargetSceneName = sceneName.ToString();
        SceneManager.LoadScene(LOADING_SCENE_NAME);
    }
}
