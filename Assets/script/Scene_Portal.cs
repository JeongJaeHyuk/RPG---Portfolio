using UnityEngine;

public class Scene_Portal : MonoBehaviour
{
    SceneName nextScene;

    void Awake()
    {
        if (!System.Enum.TryParse(gameObject.name, out nextScene))
        {
            Debug.LogError($"[Scene_Portal] '{gameObject.name}'이 SceneName enum에 없습니다.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            LoadingData.LoadScene(nextScene);
        }
    }
}
