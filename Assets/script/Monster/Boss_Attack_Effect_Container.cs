using System.Collections;
using UnityEngine;

// 검풍처럼 서로 다른 각도의 파티클 여러 개(자식)를 한 번에 묶어서 위치만 잡아주는 컨테이너.
// 데미지 판정은 이 컨테이너가 아니라 자식들(Boss_Attack_Effect_Projectile 등)이 각자 담당한다.
// BossEffectManager.PlayEffect()는 이 컨테이너만 찾아서 위치/회전을 맞춰주고, 자식 개별 발사는
// Monster_State_Boss가 애니메이션 이벤트 타이밍에 맞춰 직접 호출한다(BossEffectManager를 다시 거치지 않음).
public class Boss_Attack_Effect_Container : Boss_Attack_Effect
{
    // hitCollider가 없으므로 부모의 Play()를 그대로 쓰면 NullReferenceException - 자식들이 다 발사되고
    // 끝날 시간만큼만 대기했다가 대기 상태로 복귀한다.
    protected override IEnumerator Play()
    {
        yield return new WaitForSeconds(lifeTime);
        BossEffectManager.instance.ReturnEffect(patternName, this);
    }
}
