using System.Collections.Generic;
using UnityEngine;

public class BossEffectManager : MonoBehaviour
{
    public static BossEffectManager instance;

    // 패턴 이름(자식 오브젝트 이름) 기준으로 이펙트 오브젝트를 하나씩 관리 - 보스가 하나뿐이고 패턴도 한 번에 하나씩만
    // 쓰기 때문에 복제(Instantiate) 없이 항상 같은 오브젝트를 재사용한다. (검풍처럼 동시에 여러 개가 필요한 패턴은
    // 이름 하나를 여러 개 등록하는 게 아니라, 그 오브젝트 하나를 여러 자식을 담은 컨테이너로 만들어서 순차 발사한다.)
    Dictionary<string, Boss_Attack_Effect> effects = new Dictionary<string, Boss_Attack_Effect>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            RegisterEffects();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 매니저 자식 오브젝트(패턴 이름)마다 있는 이펙트를 딕셔너리에 등록하고 꺼둠 - 복제하지 않음
    void RegisterEffects()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            // 자식 트랜스정보가져오기
            Transform patternParent = transform.GetChild(i);
            // 그 오브젝트이름그대로 string에 대입
            string patternName = patternParent.name;
            // 그리고 자식의 자식오브젝트 즉 파티클오브젝트정보가져와서 boo_Attadck_effect가 잇는지 확인하고 있으면 이펙트변수에 대입
            Boss_Attack_Effect effect = patternParent.GetComponentInChildren<Boss_Attack_Effect>(true);
            if (effect == null)
            {
                Debug.LogWarning($"[BossEffectManager] '{patternName}' 아래에 Boss_Attack_Effect가 없습니다.");
                continue;
            }
            // 이펙트딕셔너리에 현재위치자식의이름을 키로 설정하고 = 이펙트를 저장
            effects[patternName] = effect;
            // 이펙트를 끄기
            effect.gameObject.SetActive(false);
        }
    }

    // 보스가 스킬을 낼 때 호출 - _point의 자식으로 붙여서 재생 (좌표만 복사하면 브레스처럼 재생 중 본이 움직이는
    // 패턴은 이펙트가 따라가지 못해서 - 자식으로 붙여두면 _point가 애니메이션으로 움직여도 같이 따라감)
    // 실행시킨 인스턴스를 반환한다 - 검풍처럼 이후에 같은 인스턴스한테 "다음 블레이드 켜라" 등을 계속 말 걸어야 할 때 씀
    public Boss_Attack_Effect PlayEffect(string _patternName, Transform _point, float _bossDamage)
    {
        if (!effects.ContainsKey(_patternName))
        {
            Debug.LogWarning($"[BossEffectManager] '{_patternName}' 패턴이 등록되어 있지 않습니다.");
            return null;
        }

        Boss_Attack_Effect effect = effects[_patternName];
        effect.transform.SetParent(_point);
        effect.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        effect.gameObject.SetActive(true);
        effect.Activate(_patternName, _bossDamage);
        return effect;
    }

    // 이펙트가 스스로 재생을 끝내고 대기 상태로 돌아갈 때 호출 - 매니저 밑 제자리로 부모 복귀
    public void ReturnEffect(string _patternName, Boss_Attack_Effect _effect)
    {
        _effect.gameObject.SetActive(false);
        _effect.transform.SetParent(transform.Find(_patternName));
    }
}
