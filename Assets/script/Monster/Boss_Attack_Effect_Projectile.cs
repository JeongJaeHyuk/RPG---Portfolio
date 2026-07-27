using System.Collections;
using UnityEngine;

public class Boss_Attack_Effect_Projectile : Boss_Attack_Effect
{
    // 검풍처럼 콜라이더 자체가 앞으로 이동해야 하는 패턴
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float travelTime = 1f;

    protected override IEnumerator Play()
    {
        hitCollider.enabled = true;

        float elapsed = 0f;
        while (elapsed < travelTime)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        hitCollider.enabled = false;
        BossEffectManager.instance.ReturnEffect(patternName, this);
    }
}
