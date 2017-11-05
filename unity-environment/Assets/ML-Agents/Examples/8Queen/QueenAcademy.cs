using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QueenAcademy : Academy
{
    public GameObject whiteCellPrefab;
    public GameObject blackCellPrefab;

    public int chessBoardSize = 8;
    public QueenChessBoard chessBoard;
    public QueenAgent queenPrefab;
    public Brain defaultBrain;
    public Text score;

    public List<QueenAgent> queens = new List<QueenAgent>();

    public override void InitializeAcademy()
    {
        chessBoard = new QueenChessBoard(chessBoardSize);

        for (int i = 0; i < chessBoardSize; i++)
        {
            var queen = Instantiate(queenPrefab) as QueenAgent;
            queen.InitializeQueen(i, this);
            queen.GiveBrain(defaultBrain);

            queens.Add(queen);

            for (int j = 0; j < chessBoardSize; j++)
            {
                Instantiate(((i % 2 == 0 && j % 2 == 1) || (i % 2 == 1 && j % 2 == 0))?whiteCellPrefab:blackCellPrefab, new Vector3(i, 0, j), Quaternion.identity);
            }
        }
    }

    public int seedConfiguration = 8493028;

    public override void AcademyReset()
    {
        List<int> freezeIndex = new List<int>();
        int freezeCount = Mathf.Clamp((chessBoardSize -2) - episodeCount / 500, 1, chessBoardSize - 2);

        while (freezeIndex.Count() < freezeCount)
        {
            int index = Random.Range(0, queens.Count());
            if (!freezeIndex.Contains(index))
                freezeIndex.Add(index);
        }

        if (episodeCount % 100 == 0)
        {
            seedConfiguration++;
        }

        Random.InitState(seedConfiguration);
        for (int i = 0; i < chessBoardSize; i++)
        {
            if (freezeIndex.Contains(i))
                queens[i].done = true;

            queens[i].MoveTo(Random.Range(0, chessBoardSize));
        }
    }

    public override void AcademyStep()
    {
        if (queens.All((a) => a.done))
            done = true;
        else
        {
            int threats = chessBoard.TotalThreatCount();

            if (chessBoard.TotalThreatCount() == 0)
            {
                foreach (var queen in queens)
                {
                    queen.win = true;
                }
            }  
            score.text = threats.ToString();
        }
    }
}
