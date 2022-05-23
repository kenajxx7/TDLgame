using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAtacks;
    bool alreadyAtacked;

    public float sightRange, attackRange;
    bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        
        playerInSightRange  = (Vector3.Distance(transform.position, player.position) < sightRange);
        playerInAttackRange = (Vector3.Distance(transform.position, player.position) < attackRange);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange  && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange &&  playerInSightRange ) AttackPlayer();
        
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(new Vector3(transform.position.x, player.position.y, transform.position.z));

        if (playerInAttackRange && !alreadyAtacked)
        {
            alreadyAtacked = true;
            Invoke("ResetAttack", timeBetweenAtacks);
            Health.health -= 25;
            Debug.Log("Player atacked");
        }
    }

    private void ResetAttack()
    {
        alreadyAtacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}