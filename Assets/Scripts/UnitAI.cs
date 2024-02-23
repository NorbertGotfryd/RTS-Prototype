using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitAI : MonoBehaviour
{
    public float checkRate = 1.0f;
    public float nearbyEnemyAttackRange;

    public LayerMask unitLayerMask;

    private PlayerAI playerAI;
    private Unit unit;

    public void InitializeAI(PlayerAI playerAI, Unit unit)
    {
        this.playerAI = playerAI;
        this.unit = unit;
    }

    private void Start()
    {
        InvokeRepeating("Check", 0.0f, checkRate);
    }

    void Check()
    {
        //check if we have nerby enemies if yes then attack them
        if (unit.state != UnitState.Attack && unit.state != UnitState.MoveToEnemy)
        {
            Unit potentialEnemy = CheckForNearbyEnemies();

            if (potentialEnemy != null)
                unit.AttackUnit(potentialEnemy);
        }

        //if we are doing nothing then find a new resource
        if (unit.state == UnitState.Idle)
            FindNewResource();
       else if (unit.state == UnitState.MoveToResource && unit.currentResourceSource == null) //if we are moving to a resource which is destroyed then find a new one
            FindNewResource();
    }

    private void FindNewResource()
    {
        ResourceSource resourceToGet = playerAI.GetClosestResource(transform.position);

        if (resourceToGet != null)
            unit.GatherResource(resourceToGet, UnitMover.GetUnitDestinationAroundResource(resourceToGet.transform.position));
        else
            PersueEnemy();
    }

    private Unit CheckForNearbyEnemies()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, nearbyEnemyAttackRange, Vector3.up, unitLayerMask);

        GameObject closest = null;
        float closestDistance = 0.0f;

        for (int x = 0; x < hits.Length; x++)
        {
            //skip if this is us
            if (hits[x].collider.gameObject == gameObject)
                continue;

            //skip is this is teammate
            if (unit.player.IsMyUnit(hits[x].collider.GetComponent<Unit>()))
                continue;

            if (!closest || Vector3.Distance(transform.position, hits[x].transform.position) < closestDistance)
            {
                closest = hits[x].collider.gameObject;
                closestDistance = Vector3.Distance(transform.position, hits[x].transform.position);
            }
        }

        if (closest != null)
            return closest.GetComponent<Unit>();
        else
            return null;
    }

    //calles when there is no resources on the map and then chase after a random enemy
    void PersueEnemy()
    {
        Player enemyPlayer = GameManager.instance.GetRandomEnemyPlayer(unit.player);

        if (enemyPlayer.units.Count > 0)
            unit.AttackUnit(enemyPlayer.units[Random.Range(0, enemyPlayer.units.Count)]);
    }
}
