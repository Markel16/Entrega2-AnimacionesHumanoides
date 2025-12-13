using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RagdollManager : MonoBehaviour
{
    public KeyCode enableKey = KeyCode.R;  
    public KeyCode disableKey = KeyCode.T; 

    Animator animator;
    CharacterController controller;

    Rigidbody[] ragdollBodies;
    Collider[] ragdollColliders;

    public Transform hips; 

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        SetRagdollState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(enableKey))
            EnableRagdoll();

        if (Input.GetKeyDown(disableKey))
            DisableRagdoll();
    }

    public void EnableRagdoll()
    {
        if (animator) animator.enabled = false;
        if (controller) controller.enabled = false;

        SetRagdollState(true);
        Debug.Log("Ragdoll si");
    }

    public void DisableRagdoll()
    {
        if (hips != null)
            transform.position = hips.position;

        SetRagdollState(false);

        if (animator) animator.enabled = true;
        if (controller) controller.enabled = true;

        Debug.Log("Ragdoll no");
    }


    void SetRagdollState(bool active)
    {
        foreach (var rb in ragdollBodies)
        {
            if (rb.transform == transform) continue; 
            rb.isKinematic = !active;
        }

        foreach (var col in ragdollColliders)
        {
            if (col is CharacterController) continue;
            col.enabled = active;
        }
    }
}
