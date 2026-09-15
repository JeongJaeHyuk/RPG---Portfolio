using System.Collections;
using UnityEngine;

// 자식 여러 개(연출용/데미지용 등)를 한 번에 묶어서 위치만 잡아주는 컨테이너 - 컨테이너 자신은 hitCollider가 없다.
// BossEffectManager.PlayEffect()는 이 컨테이너만 찾아서 위치/회전을 맞춰주고, 자식 개별 제어(순차 재생 등)는
// Monster_State_Boss가 애니메이션 이벤트/코루틴 타이밍에 맞춰 직접 호출한다(BossEffectManager를 다시 거치지 않음).
public class Boss_Attack_Effect_Container : Boss_Attack_Effect
{
    // hitCollider가 없으므로 부모의 Play()를 그대로 쓰면 NullReferenceException - 자식들이 다 재생되고
    // 끝날 시간(lifeTime)만큼만 대기했다가 대기 상태로 복귀한다. 자식은 컨테이너와 함께 비활성화된다.
    protected override IEnumerator Play()
    {
        yield return new WaitForSeconds(lifeTime);
        BossEffectManager.instance.ReturnEffect(patternName, this);
    }
}
