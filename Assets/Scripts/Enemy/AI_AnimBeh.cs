using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_AnimBeh : MonoBehaviour
{
    [SerializeField]
    AI_StateBehaviour stateBeh;
    private void Start()
    {
        if(stateBeh == null)
            stateBeh = GetComponentInParent<AI_StateBehaviour>();
    }
    public void DoMeleeAttack()
    {
        stateBeh.DoMeleeAttack();
    }
    public void DoRangedAttack()
    {
        stateBeh.DoRangedAttack();
    }

    [SerializeField]
    AudioClip[] footStepDirt;
    bool canFootStep = true;
    public void FootStepSFX()
    {
        if (canFootStep == true)
        {
            AudioManager.instance.PlayRandomOneShotSoundFXClip(footStepDirt, transform, 0.8f);
            canFootStep = false;
            StartCoroutine(WaitFootStep());
            //Debug.Log("Step");
        }
    }
    IEnumerator WaitFootStep()
    {
        yield return new WaitForSeconds(0.1f);
        canFootStep = true;
    }
}
