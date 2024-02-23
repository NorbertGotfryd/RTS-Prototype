using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCommander : MonoBehaviour
{
    public GameObject selectionMarkerPrefab;
    public LayerMask layerMask;
    public float unitGap;

    //components
    private UnitSelection unitSelection;
    private Camera cam;

    private void Awake()
    {
        unitSelection = GetComponent<UnitSelection>();
        cam = Camera.main;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1) && unitSelection.HasUnitSelected())
        {
            //shoot a raycast from our mouse to see wahat we hit
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Unit[] selectedUnits = unitSelection.GetSelectedUnit();

            //shoot the raycast
            if(Physics.Raycast(ray, out hit, 100, layerMask))
            {
                unitSelection.RemoveNullUnitsFromSelection();

                //are we clicking on the ground?
                if (hit.collider.CompareTag("Ground"))
                {
                    UnitsMoveToPosition(hit.point, selectedUnits);
                    CreateSelectionmarker(hit.point, false);
                }
                //are we clicking on the recource?
                else if (hit.collider.CompareTag("Resources"))
                {
                    UnitsGatherRecource(hit.collider.GetComponent<ResourceSource>(), selectedUnits);
                    CreateSelectionmarker(hit.collider.transform.position, true);
                }
                //are we clicking on the enemy unit?
                else if (hit.collider.CompareTag("Unit"))
                {
                    Unit enemy = hit.collider.gameObject.GetComponent<Unit>();
                    if (!Player.me.IsMyUnit(enemy))
                    {
                        UnitsAttackEnemy(enemy, selectedUnits);
                        CreateSelectionmarker(enemy.transform.position, false);
                    }
                }
            }
        }
    }

    private void UnitsMoveToPosition (Vector3 movePosition, Unit[] units)
    {
        Vector3[] destinations = UnitMover.GetUnitGroupDestination(movePosition, units.Length, unitGap);

        for (int x = 0; x < units.Length; x++)
        {
            units[x].MoveToPosition(destinations[x]);
        }
    }

    private void UnitsGatherRecource(ResourceSource resource, Unit[] units)
    {
        if(units.Length == 1)
        {
            units[0].GatherResource(resource, UnitMover.GetUnitDestinationAroundResource(resource.transform.position));
        }
        else
        {
            Vector3[] destinations = UnitMover.GetUnitGroupDestinationsAroundResource(resource.transform.position, units.Length);

            for (int x = 0; x < units.Length; x++)
            {
                units[x].GatherResource(resource, destinations[x]);
            }
        }
    }

    //called when we command units to attack an enemy
    private void UnitsAttackEnemy(Unit target, Unit[] units)
    {
        for (int x = 0; x < units.Length; x++)
            units[x].AttackUnit(target);
    }

    //create a new selection marker visual at the given position
    private void CreateSelectionmarker(Vector3 position, bool large)
    {
        GameObject marker = Instantiate(selectionMarkerPrefab, new Vector3(position.x, 0.01f, position.z), Quaternion.identity);

        if (large)
            marker.transform.localScale = Vector3.one * 3;
    }
}
