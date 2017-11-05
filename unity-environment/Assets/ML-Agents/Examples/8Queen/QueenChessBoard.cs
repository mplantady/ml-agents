using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenChessBoard 
{   
    public int[] chessBoard;
    public int size;

    public QueenChessBoard(int s)
    {
        size = s;
        chessBoard = new int[size];
    }

    public int ThreatCount(int queenX, int queenY)
    {
        int threat = 0;
        for (int x = 0; x < chessBoard.Length; x++)
        {
            if (x == queenX)
                continue;

            if (chessBoard[x] == queenY)
                threat++;
            else if (chessBoard[x] == queenY + Math.Abs(x - queenX))
                threat++;
            else if (chessBoard[x] == queenY - Math.Abs(x - queenX))
                threat++;
        }

        return threat;
    }

    public void Move(int x, int y)
    {
        chessBoard[x] = y;
    }

    public int TotalThreatCount()
    {
        int total = 0;
        for (int x = 0; x < chessBoard.Length; x++)
        {
            total += ThreatCount(x, chessBoard[x]);
        }

        return total;
    }
}
