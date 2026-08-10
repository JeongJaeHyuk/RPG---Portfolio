using UnityEngine;

// "Shout"(등장 연출) State에만 붙일 것 - "Shout 0"(전투 중 예비동작)은 같은 클립을 쓰지만
// 이건 State 단위로 걸리는 StateMachineBehaviour라서 전투 중엔 호출되지 않는다
public class Boss_Intro_Behaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Monster_State_Boss bossState = animator.GetComponent<Monster_State_Boss>();
        if (bossState != null)
        {
            bossState.StartCombat();
        }
    }
}
