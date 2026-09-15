using System.Collections;
using UnityEngine;

// 컨테이너(Boss_Attack_Effect_Container) 안에 계속 nested 상태로 남아있어야 하는 자식용 - 부모 클래스처럼
// 재생 끝나고 스스로 BossEffectManager에 반환(ReturnEffect)하지 않고, 자기 자신만 끈다. 실제 반환은
// 부모 컨테이너가 자신의 lifeTime이 끝날 때 통째로 처리한다(이때 자식도 같이 비활성화됨).
public class Boss_Attack_Effect_Child : Boss_Attack_Effect
{
    protected override IEnumerator Play()
    {
        hitCollider.enabled = true;
        yield return new WaitForSeconds(hitDuration);
        hitCollider.enabled = false;
        gameObject.SetActive(false);
    }
}
