using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitStateBubble : MonoBehaviour
{
    public Image stateBubble;

    public Sprite idleState;
    public Sprite attackState;
    public Sprite gatherState;

    public void OnStateChange(UnitState state)
    {
        stateBubble.enabled = true;

        switch (state)
        {
            case UnitState.Idle:
                {
                    stateBubble.sprite = idleState;
                    break;
                }
            case UnitState.Attack:
                {
                    stateBubble.sprite = attackState;
                    break;
                }
            case UnitState.Gather:
                {
                    stateBubble.sprite = gatherState;
                    break;
                }
            default:
                {
                    stateBubble.enabled = false;
                    break;
                }

        }
    }
}
