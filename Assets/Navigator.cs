using UnityEngine;
using System.Collections;

public class Navigator : MonoBehaviour {

    NavMeshAgent agent;
    Targeter targeter;
    Animator animator;

	void Awake () {
        agent = GetComponent<NavMeshAgent>();
        targeter = GetComponent<Targeter>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetFloat("Distance", agent.remainingDistance);
    }

    public void NavigateTo(Vector3 position)
    {
        agent.SetDestination(position); 
        targeter.target = null;
        animator.SetBool("Attack", false);
    }
}
