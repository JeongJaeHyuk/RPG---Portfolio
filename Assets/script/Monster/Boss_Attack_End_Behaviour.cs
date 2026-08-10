using UnityEngine;

// 공격 State(ComboAttack/OverheadSlash/Slash Attack/Leaping Strike/Breath) 5개 전부에 붙일 것.
// 애니메이션 이벤트로 ExitAttack()을 부르면 State가 실제로 안 끝났는데 코드만 먼저 "공격 끝남" 처리되어
// Monster_Move가 애니메이션 꼬리와 겹쳐서 미끄러지듯 이동하는 문제가 있었음 - OnStateExit은 State를
// 실제로 벗어나는 순간에 맞춰 호출되어 이 문제가 없다
public class Boss_Attack_End_Behaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 임시 주석 처리 - Exit Time 조정만으로 미끄러짐이 해결되는지 먼저 테스트하기 위함
        //Monster_State_Boss bossState = animator.GetComponent<Monster_State_Boss>();
        //if (bossState != null)
        //{
        //    bossState.ExitAttack();
        //}
    }
}
