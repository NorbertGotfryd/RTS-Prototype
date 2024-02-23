using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public RectTransform selectionBox;
    public LayerMask unitLayerMask;

    private List<Unit> selectedUnits = new List<Unit>();
    private Vector2 startPosition;

    //components
    private Camera cam;
    private Player player;

    private void Awake()
    {
        //get the components
        cam = Camera.main;
        player = GetComponent<Player>();
    }

    private void Update()
    {
        //mouse down
        if (Input.GetMouseButtonDown(0))
        {
            ToggleSelectionVisual(false);
            //clear selected unit list
            selectedUnits = new List<Unit>();

            TrySelect(Input.mousePosition);
            startPosition = Input.mousePosition;
        }

        //mouse up
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox();
        }

        //mouse help down
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }
    }

    //called wghen we release the selection box
    private void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach(Unit unit in player.units)
        {
            //convert unit world position to screen position
            Vector3 screenPosition = cam.WorldToScreenPoint(unit.transform.position);

            //check if unit screen position is in slection box
            if (screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
            {
                selectedUnits.Add(unit);
                unit.ToggleSelectionVisual(true);
            }

        }
    }

    //called when we click on a unit
    private void TrySelect(Vector2 screenPosition)
    {
        //create ray from mouse postion
        Ray ray = cam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100, unitLayerMask))
        {
            Unit unit = hit.collider.GetComponent<Unit>();

            if (player.IsMyUnit(unit))
            {
                selectedUnits.Add(unit);
                unit.ToggleSelectionVisual(true);
            } 
        }
    }

    //called when we are creatinmg a selection box
    private void UpdateSelectionBox(Vector2 currentMousePosition)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        float width = currentMousePosition.x - startPosition.x;
        float height = currentMousePosition.y - startPosition.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = startPosition + new Vector2(width / 2, height / 2);
    }

    //removes all destroyed or missing units from the selected list
    public void RemoveNullUnitsFromSelection()
    {
        for (int x = 0; x < selectedUnits.Count; x++)
        {
            if (selectedUnits[x] == null)
                selectedUnits.RemoveAt(x);
        }
    }

    private void ToggleSelectionVisual(bool selected)
    {
        foreach(Unit unit in selectedUnits)
            unit.ToggleSelectionVisual(selected);
    }

    public bool HasUnitSelected()
    {
        return selectedUnits.Count > 0 ? true : false;
    }

    //covnetr selected unit List to array
    public Unit[] GetSelectedUnit()
    {
        return selectedUnits.ToArray();
    }
}
