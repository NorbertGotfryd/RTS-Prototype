using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player[] players;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    //return a random enemy player
    public Player GetRandomEnemyPlayer(Player me)
    {
        Player randomPlayer = players[Random.Range(0, players.Length)];

        while(randomPlayer == me)
        {
            randomPlayer = players[Random.Range(0, players.Length)];
        }

        return randomPlayer;
    }

    //calles when a unit dies, check to see if thete is one remaining player
    public void UnitDeathCheck()
    {
        int remainingPlayers = 0;
        Player winner = null;

        for (int x = 0; x < players.Length; x++)
        {
            if(players[x].units.Count > 0)
            {
                remainingPlayers++;
                winner = players[x];
            }
        }

        //if there is mose than 1 remaining player, return
        if (remainingPlayers != 1)
            return;

        EndScreenUI.instance.SetEndScreen(winner.isMe);
    }
}
