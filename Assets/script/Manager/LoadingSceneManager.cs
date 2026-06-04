using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// =====================================================================
// LoadingSceneManager : 로딩씬에서 실제 씬 로딩을 처리하는 스크립트
// =====================================================================
// [역할]
// - LoadingData에서 이동할 씬 이름을 받아와서 비동기 로딩 실행
// - 로딩 진행률을 슬라이더와 텍스트로 표시
// - 로딩 완료 후 자동으로 목표 씬으로 전환
//
// [Unity 에디터 설정 방법]
// 1. LoadingScene을 새로 만들고 이 스크립트를 빈 GameObject에 붙입니다
// 2. Canvas 아래에 Slider(progressBar)와 Text(progressText)를 만듭니다
// 3. 인스펙터에서 각 UI를 연결합니다
// =====================================================================
public class LoadingSceneManager : MonoBehaviour
{
    [Header("── 로딩 UI ──────────────────────")]
    [SerializeField] Slider progressBar;   // 로딩 진행률 슬라이더 (0~1)
    [SerializeField] Text progressText;    // 진행률 텍스트 (예: "75%")
    [SerializeField] Text loadingInfoText; // 로딩 상태 텍스트 (예: "캐릭터 로딩중...")

    void Start()
    {
        Debug.Log("[LoadingSceneManager] 로딩씬 진입");
        StartCoroutine(LoadTargetScene());
    }

    // ------------------------------------------------------------------
    // 비동기 씬 로딩 코루틴
    // 백그라운드에서 씬을 불러오면서 진행률을 UI에 반영합니다
    // ------------------------------------------------------------------
    IEnumerator LoadTargetScene()
    {
        // LoadingData에 저장된 이동할 씬 이름을 가져옴
        string targetScene = LoadingData.TargetSceneName;
        Debug.Log($"[LoadingSceneManager] 목표 씬 : {targetScene}");

        // 씬 이름이 비어있으면 로딩 진행 불가
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogError("[LoadingSceneManager] 씬 이름이 없습니다. LoadingData.LoadScene()을 통해 이동하세요.");
            yield break;
        }

        // 비동기 씬 로딩 시작
        // allowSceneActivation = false : 로딩이 완료되어도 바로 전환하지 않고 대기
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        Debug.Log($"[LoadingSceneManager] {targetScene} 비동기 로딩 시작");
        asyncLoad.allowSceneActivation = false;

        // 로딩 진행률 업데이트 (Unity는 allowSceneActivation=false 일 때 0.9까지만 채워짐)
        while (!asyncLoad.isDone)
        {
            // 실제 진행률을 0~1 사이로 변환 (0.9를 최대로 나눔)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // UI 업데이트
            if (progressBar != null)
                progressBar.value = progress;

            if (progressText != null)
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";

            // 진행률에 따라 로딩 상태 텍스트 변경
            // 나중에 완성후 진행속도가 너무빠를시 그냥 로딩정보말고 팁같은거 적는게 더좋아보임
            if (loadingInfoText != null)
            {
                if (progress < 0.25f)      loadingInfoText.text = "게임 데이터 로딩중...";
                else if (progress < 0.5f)  loadingInfoText.text = "캐릭터 로딩중...";
                else if (progress < 0.75f) loadingInfoText.text = "이미지 로딩중...";
                else if (progress < 0.9f)  loadingInfoText.text = "맵 로딩중...";
                else                       loadingInfoText.text = "최적화중...";
            }

            // 로딩 완료(90% 도달)되면 씬 전환 허용
            if (asyncLoad.progress >= 0.9f)
            {
                // 100% 표시 후 씬 전환
                if (progressBar != null) progressBar.value = 1f;
                if (progressText != null) progressText.text = "100%";

                yield return new WaitForSeconds(2f);
                Debug.Log($"[LoadingSceneManager] {targetScene} 로딩 완료 - 씬 전환");
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
