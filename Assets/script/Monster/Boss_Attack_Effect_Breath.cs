using System.Collections;
using UnityEngine;

public class Boss_Attack_Effect_Breath : Boss_Attack_Effect
{
    // 브레스처럼 콜라이더를 반복적으로 켰다 끄는 패턴
    [SerializeField] float tickOnTime = 0.3f;  // 한 번 켜져있는 시간
    [SerializeField] float tickOffTime = 0.2f; // 한 번 꺼져있는 시간
    [SerializeField] int tickCount = 4;        // 반복 횟수

    protected override IEnumerator Play()
    {
        for (int i = 0; i < tickCount; i++)
        {
            hitCollider.enabled = true;
            yield return new WaitForSeconds(tickOnTime);
            hitCollider.enabled = false;
            yield return new WaitForSeconds(tickOffTime);
        }

        BossEffectManager.instance.ReturnEffect(patternName, this);
    }
}
