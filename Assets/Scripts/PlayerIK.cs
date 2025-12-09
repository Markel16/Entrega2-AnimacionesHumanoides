using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    [Header("Activación general de IK")]
    public bool ikEnabled = true;

    [Header("Targets")]
    public Transform lookTarget;       
    public Transform leftHandTarget;   
    public Transform rightHandTarget;  

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || !ikEnabled)
        {
            ResetIK();
            return;
        }

        
        bool isHolding = animator.GetBool("IsHolding");

        
        if (isHolding && lookTarget != null)
        {
            
            animator.SetLookAtWeight(1f, 0.3f, 0.7f, 1f, 0.5f);
            animator.SetLookAtPosition(lookTarget.position);
        }
        else
        {
            animator.SetLookAtWeight(0f);
        }

        
        if (isHolding)
        {
            if (leftHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }

            if (rightHandTarget != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }
        }
        else
        {
            
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
        }
    }

    void ResetIK()
    {
        animator.SetLookAtWeight(0f);
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
    }
}
