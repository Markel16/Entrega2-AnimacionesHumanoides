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

    [Header("IK de piernas")]
    public bool feetIkEnabled = true;
    public LayerMask groundMask = 0;   
    public float footRayDistance = 1.5f;
    public float footOffsetY = 0.05f;  


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
        
        if (feetIkEnabled && !isHolding)  
        {
            HandleFootIK(AvatarIKGoal.LeftFoot);
            HandleFootIK(AvatarIKGoal.RightFoot);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0f);
            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0f);
        }

    }

    void HandleFootIK(AvatarIKGoal foot)
    {
       
        Vector3 footPos = animator.GetIKPosition(foot);
        Vector3 origin = footPos + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, footRayDistance, groundMask))
        {
            Vector3 targetPos = hit.point + Vector3.up * footOffsetY;
            Quaternion targetRot = Quaternion.LookRotation(
                Vector3.ProjectOnPlane(transform.forward, hit.normal),
                hit.normal
            );

            animator.SetIKPositionWeight(foot, 1f);
            animator.SetIKRotationWeight(foot, 1f);
            animator.SetIKPosition(foot, targetPos);
            animator.SetIKRotation(foot, targetRot);
        }
        else
        {
            animator.SetIKPositionWeight(foot, 0f);
            animator.SetIKRotationWeight(foot, 0f);
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
