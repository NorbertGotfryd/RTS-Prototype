using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAI : MonoBehaviour
{
    public float checkRate = 1.0f;
    private ResourceSource[] resources;

    private Player player;
    private PlayerAI playerAI;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        resources = FindObjectsOfType<ResourceSource>();
        InvokeRepeating("Check", 0.0f, checkRate);
    }

    private void Check()
    {
        //create new unit if we have enought food
        if (player.food >= player.unitCost)
            player.CreateNewUnit();
    }

    //called when a new unit is created
    public void OnUnitCreated(Unit unit)
    {
       unit.GetComponent<UnitAI>().InitializeAI(this, unit);
    }

    public ResourceSource GetClosestResource(Vector3 position)
    {
        ResourceSource[] closest = new ResourceSource[3];
        float[] closestDistance = new float[3];

        foreach (ResourceSource resource in resources)
        {
            if (resource == null)
                continue;

            float distance = Vector3.Distance(position, resource.transform.position);

            for (int x = 0; x < closest.Length; x++)
            {
                if (closest[x] == null)
                {
                    closest[x] = resource;
                    closestDistance[x] = distance;
                    break;
                }
                else if (distance < closestDistance[x])
                {
                    closest[x] = resource;
                    closestDistance[x] = distance;
                    break;
                }
            }
        }
        return closest[Random.Range(0, closest.Length)];
    }
}
