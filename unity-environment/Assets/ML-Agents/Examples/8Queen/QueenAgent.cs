using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenAgent : Agent
{
    public QueenChessBoard board;
    public int agentX;
    public int agentY;
    public bool win;

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();

        // define a value for each cell
        for (int i = 0; i < board.size; i++)
        {
            state.Add((agentX == i) ? 1 : -1);
        }
     
        for (int i = 0; i < board.size; i++)
        {
            state.Add((agentY == i) ? 1 : -1);
        }

        for (int i = 0; i < board.size; i++)
        {
            state.Add(Mathf.Clamp01(board.ThreatCount(agentX, i) / (float)board.size));
        }

        for (int i = 0; i < board.size; i++)
        {
            state.Add(board.chessBoard[i] / (float)board.size);
        }

        return state;
    }

    public override void AgentStep(float[] act)
    {
        if (win)
        {
            reward = 10;
            done = true;
            return;
        }

        if (act[0] >= 0)
        {
            MoveTo((int)act[0]);

            int totalThreat = board.ThreatCount(agentX, (int)act[0]);

            switch (totalThreat)
            {
                case 0:
                    reward += 0.01f;
                    break;
                default:
                    reward -= totalThreat * 0.01f;
                    break;
                    
            }
        }
    }

    public override void AgentReset()
    {
        win = false;
    }

    public void InitializeQueen(int x, QueenAcademy a)
    {
        board = a.chessBoard;
        agentX = x;
    }

    public void MoveTo(int y)
    {
        agentY = y;
        transform.position = new Vector3(agentX, transform.position.y, agentY);
        board.Move(agentX, agentY);
    }

    public void OnDrawGizmos()
    {
        if (done)
            return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < board.size; i++)
        {
            var start = new Vector3(transform.position.x + i * 0.05f - 0.2f, 1.9f, transform.position.z);

            Gizmos.DrawLine(start, start + Vector3.up * board.ThreatCount(agentX, i) * 0.1f);
        }
    }
}
