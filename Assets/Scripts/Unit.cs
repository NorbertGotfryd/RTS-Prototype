using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum UnitState
{
    Idle,
    Move,
    MoveToResource,
    Gather,
    MoveToEnemy,
    Attack
}

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    public UnitState state;

    public int currentHp;
    public int maxHp;

    public int minAttackDamage;
    public int maxAttackDamage;

    public float attackRate;
    public float attackRange;
    private float lastAttackTime;

    public float pathUpdateRate = 1.0f;
    private float lastPathUpdateTime;

    public int gatherAmount;
    public float gatherRate;
    private float lastGatherTime;

    public ResourceSource currentResourceSource;
    private Unit currentEnemyTarget;

    [Header("Components")]
    public GameObject selectionVisual;
    private NavMeshAgent navAgent;
    public UnitHealthBar healthBar;

    public Player player;

    // events
    [System.Serializable]
    public class StateChangeEvent : UnityEvent<UnitState> { }
    public StateChangeEvent onStateChange;

    private void Start()
    {
        //get the components
        navAgent = GetComponent<NavMeshAgent>();

        SetState(UnitState.Idle);
    }

    private void Update()
    {
        switch (state)
        {
            case UnitState.Move:
                {
                    MoveUpdate();
                    break;
                }
            case UnitState.MoveToResource:
                {
                    MoveToResourceUpdate();
                    break;
                }
            case UnitState.Gather:
                {
                    GatherUpdate();
                    break;
                }
            case UnitState.MoveToEnemy:
                {
                    MoveToEnemy();
                    break;
                }
            case UnitState.Attack:
                {
                    AttackUpdate();
                    break;
                }
        }
    }

    private void MoveUpdate()
    {
        //calculating distance and if it is 0 then set state to Idle
        if (Vector3.Distance(transform.position, navAgent.destination) == 0.0f)
            SetState(UnitState.Idle);
    }

    private void MoveToResourceUpdate()
    {
        //if resource is destroy then set state to idle
        if (currentResourceSource == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        if (Vector3.Distance(transform.position, navAgent.destination) == 0.0f)
            SetState(UnitState.Gather);
    }

    private void GatherUpdate()
    {
        if (currentResourceSource == null)
        {
            //if resource is destroy then set state to idle
            SetState(UnitState.Idle);
            return;
        }

        LookAt(currentResourceSource.transform.position);

        if (Time.time - lastGatherTime > gatherRate)
        {
            lastGatherTime = Time.time;
            currentResourceSource.GatherResource(gatherAmount, player);
        }
    }

    private void SetState(UnitState toState)
    {
        //state = current state, toState = state we wanna go to
        state = toState;

        //calling the event
        if (onStateChange != null)
            onStateChange.Invoke(state);

        if (toState == UnitState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    //called every frame the MoveToEnemy state is active
    private void MoveToEnemy()
    {
        //if are target is dead go idle
        if (currentEnemyTarget == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        if (Time.time - lastPathUpdateTime > pathUpdateRate)
        {
            lastPathUpdateTime = Time.time;
            navAgent.isStopped = false;
            navAgent.SetDestination(currentEnemyTarget.transform.position);
        }

        if (Vector3.Distance(transform.position, currentEnemyTarget.transform.position) <= attackRange)
            SetState(UnitState.Attack);
    }

    //called evert frame the Attack state is active
    private void AttackUpdate()
    {
        //if are target is dead go idle
        if (currentEnemyTarget == null)
        {
            SetState(UnitState.Idle);
            return;
        }

        //if we are still moving, stop
        if (!navAgent.isStopped)
            navAgent.isStopped = true;

        //attack every "attack rate" seconds
        if (Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            currentEnemyTarget.TakeDamage(Random.Range(minAttackDamage, maxAttackDamage + 1)); //max value is excluvie (not included)
        }

        //look at enemy
        LookAt(currentEnemyTarget.transform.position);

        //if we are to  far away from enemy, move towards enemy
        if (Vector3.Distance(transform.position, currentEnemyTarget.transform.position) > attackRange)
        {
            SetState(UnitState.MoveToEnemy);
        }
    }

    //called when an enemy unit attack unit
    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
            Die();

        //update health bar
        healthBar.UpdateHealthBar(currentHp, maxHp);
    }

    private void Die()
    {
        player.units.Remove(this);

        GameManager.instance.UnitDeathCheck();

        Destroy(gameObject);
    }

    public void MoveToPosition(Vector3 position)
    {
        SetState(UnitState.Move);

        navAgent.isStopped = false;
        navAgent.SetDestination(position);
    }

    public void GatherResource(ResourceSource resource, Vector3 position)
    {
        currentResourceSource = resource;
        SetState(UnitState.MoveToResource);

        navAgent.isStopped = false;
        navAgent.SetDestination(position);
    }

    public void AttackUnit(Unit target)
    {
        currentEnemyTarget = target;
        SetState(UnitState.MoveToEnemy);
    }

    public void ToggleSelectionVisual(bool selected)
    {
        if (selectionVisual != null)
            selectionVisual.SetActive(selected);
    }

    void LookAt(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized; //calculating direction
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;    //calculating angle

        transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
