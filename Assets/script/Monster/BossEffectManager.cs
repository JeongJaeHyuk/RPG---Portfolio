using System.Collections.Generic;
using UnityEngine;

public class BossEffectManager : MonoBehaviour
{
    public static BossEffectManager instance;

    const int constPoolSizePerType = 5;

    // 패턴 이름(자식 오브젝트 이름) 기준으로 이펙트 원본과 풀을 관리
    Dictionary<string, Boss_Attack_Effect> effectPrefabs = new Dictionary<string, Boss_Attack_Effect>();
    Dictionary<string, Queue<Boss_Attack_Effect>> effectQueues = new Dictionary<string, Queue<Boss_Attack_Effect>>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            CreateEffects();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 매니저 자식 오브젝트(패턴 이름)마다 원본 이펙트를 두고, 그 이름으로 풀 생성
    void CreateEffects()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform patternParent = transform.GetChild(i);
            string patternName = patternParent.name;

            Boss_Attack_Effect prefab = patternParent.GetComponentInChildren<Boss_Attack_Effect>(true);
            if (prefab == null)
            {
                Debug.LogWarning($"[BossEffectManager] '{patternName}' 아래에 Boss_Attack_Effect 원본이 없습니다.");
                continue;
            }

            effectPrefabs[patternName] = prefab;
            effectQueues[patternName] = new Queue<Boss_Attack_Effect>();

            for (int j = 0; j < constPoolSizePerType; j++)
            {
                effectQueues[patternName].Enqueue(CreateEffect(patternName, patternParent));
            }
        }
    }

    Boss_Attack_Effect CreateEffect(string _patternName, Transform _parent)
    {
        Boss_Attack_Effect effect = Instantiate(effectPrefabs[_patternName], _parent);
        effect.gameObject.SetActive(false);
        return effect;
    }

    // 보스가 스킬을 낼 때 호출 - _point 위치/방향으로 이펙트를 꺼내 부모를 해제하고 재생
    public void PlayEffect(string _patternName, Transform _point, float _bossDamage)
    {
        if (!effectQueues.ContainsKey(_patternName))
        {
            Debug.LogWarning($"[BossEffectManager] '{_patternName}' 패턴이 등록되어 있지 않습니다.");
            return;
        }

        Boss_Attack_Effect effect = effectQueues[_patternName].Count > 0
            ? effectQueues[_patternName].Dequeue()
            : CreateEffect(_patternName, transform.Find(_patternName));

        effect.transform.SetParent(null);
        effect.transform.SetPositionAndRotation(_point.position, _point.rotation);
        effect.gameObject.SetActive(true);
        effect.Activate(_patternName, _bossDamage);
    }

    // 이펙트가 스스로 재생을 끝내고 풀로 복귀할 때 호출
    public void ReturnEffect(string _patternName, Boss_Attack_Effect _effect)
    {
        _effect.gameObject.SetActive(false);
        _effect.transform.SetParent(transform.Find(_patternName));
        effectQueues[_patternName].Enqueue(_effect);
    }
}
