using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Basic_State_Death : AI_Base_State
{
    public Basic_State_Death(GameObject _npc, NavMeshAgent _agent, Animator _animator, GameObject _target, CharacterStats _npcStats, AI_StateBehaviour _npcStateBeh, AI_Movement _npcMovement, AI_Canvas _npcCanvas)
           : base(_npc, _agent, _animator, _target, _npcStats, _npcStateBeh, _npcMovement, _npcCanvas)
    {
        stateName = STATE.DEAD;
    }

    public override void Enter()
    {
        anim.CrossFade("Death", 0.2f, -1);

        npcStateBeh.Change_Anim_MoveX_Weight(0f, 0.1f);
        if (agent.isActiveAndEnabled)
        {
            agent.updateRotation = false;
            agent.isStopped = true;
            agent.enabled = false;
        }
        if (npcMovement.enabled == true)
        {
            npcMovement.canMove = false;
            npcMovement.canRotate = false;
            npcMovement.enabled = false;
        }
        if (npcCanvas.enabled)
        {
            npcCanvas.ActivateAlert(false);
            npcCanvas.ActivateDeathMark(false);
            npcCanvas.ActivateVisionSlider(false);
        }
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
