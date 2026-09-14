using System.Collections;
using UnityEngine;

public class Boss_Attack_Effect_Projectile : Boss_Attack_Effect
{
    // 검풍처럼 콜라이더 자체가 앞으로 이동해야 하는 패턴
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float travelTime = 1f;

    Transform homeParent;
    Vector3 homeLocalPosition;
    Quaternion homeLocalRotation;
    bool homeCached; // 처음 발사될 때 원래 부모/로컬 위치/회전(부채꼴 각도)을 기억해뒀다가 발사 끝나면 그 자리로 되돌아옴

    protected override IEnumerator Play()
    {
        if (!homeCached)
        {
            homeParent = transform.parent;
            homeLocalPosition = transform.localPosition;
            homeLocalRotation = transform.localRotation;
            homeCached = true;
        }

        // 발사 시작 - 컨테이너(부모)에서 분리해야 스윙 중 컨테이너가 움직여도 이미 날아가는 블레이드 궤적이 안 꼬임
        transform.SetParent(null);
        hitCollider.enabled = true;

        float elapsed = 0f;
        while (elapsed < travelTime)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }

        hitCollider.enabled = false;
        gameObject.SetActive(false);
        // 검풍 컨테이너의 자식으로, 원래 위치/각도 그대로 복귀 (ReturnEffect를 쓰면 BossEffectManager 밑으로
        // 부모가 바뀌어버려서 다음 발사 때 못 찾게 되므로 직접 원래 부모로 되돌려놓는다)
        transform.SetParent(homeParent);
        transform.SetLocalPositionAndRotation(homeLocalPosition, homeLocalRotation);
    }
}
