using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    //calculate a unit formation a givn destination
    public static Vector3[] GetUnitGroupDestination(Vector3 movePosition, int numberUnits, float unitGap)
    {
        //vector3 array for final destinations
        Vector3[] destinations = new Vector3[numberUnits];

        //calculate rows and columns
        int rows = Mathf.RoundToInt(Mathf.Sqrt(numberUnits));
        int columns = Mathf.CeilToInt((float)numberUnits / (float)rows);

        int currentRow = 0;
        int currentColumn = 0;

        float width = ((float)rows - 1) * unitGap;
        float length = ((float)columns - 1) * unitGap;

        for (int x = 0; x < numberUnits; x++)
        {
            destinations[x] = movePosition + (new Vector3(currentRow, 0, currentColumn) * unitGap) - new Vector3(length / 2, 0, width / 2);
            currentColumn++;

            if(currentColumn == rows)
            {
                currentColumn = 0;
                currentRow++;                    
            }
        }

        return destinations;
    }

    public static Vector3[] GetUnitGroupDestinationsAroundResource(Vector3 resourcePosition, int unitsNumber)
    {
        Vector3[] destinations = new Vector3[unitsNumber];
        float unitDistanceGap = 360.0f / (float)unitsNumber;

        for (int x = 0; x < unitsNumber; x++)
        {
            float angle = unitDistanceGap * x;
            Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad)); //?
            destinations[x] = resourcePosition + direction;
        }
        return destinations;
    }

    public static Vector3 GetUnitDestinationAroundResource(Vector3 resourcePosition)
    {
        float angle = Random.Range(0, 360);
        Vector3 direction = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));

        return resourcePosition + direction;
    }
}
