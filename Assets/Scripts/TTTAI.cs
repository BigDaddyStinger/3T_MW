using System;
using System.Collections.Generic;
using UnityEngine;

public class TTTAI : MonoBehaviour
{
    private static readonly int[][] Lines = new int[][]
    {
        new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8},
        new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8},
        new[] {0,4,8}, new[] {2,4,6}
    };

    private static readonly int[] Corners = { 0, 2, 6, 8 };
    private const int Center = 4;

    public static int GetBestMove(char[] board, char ai = 'O', char human = 'X', System.Random rng = null)
    {
        if (board == null || board.Length != 9) throw new ArgumentException("Board must be length 9.");
        rng ??= new System.Random();

        for (int i = 0; i < 9; i++)
            if (board[i] == '\0') board[i] = ' ';

        int m = FindLineFinish(board, ai);
        if (m != -1) return m;

        m = FindLineFinish(board, human);
        if (m != -1) return m;

        if (IsBoardEmpty(board))
        {
            foreach (int c in Corners)
                return c; // any corner
        }

        if (OpponentControlsAnyCorner(board, human) && IsEmpty(board, Center))
            return Center;

        if (AIControlsAnyCorner(board, ai) && board[Center] != ai)
        {
            int edge = TakeEdgeAdjacentToOwnedCorner(board, ai);
            if (edge != -1) return edge;
        }

        if (IsEmpty(board, Center)) return Center;

        foreach (int c in Corners)
            if (IsEmpty(board, c)) return c;

        var empties = GetEmptyCells(board);
        return empties.Count > 0 ? empties[rng.Next(empties.Count)] : -1;
    }

    private static int FindLineFinish(char[] board, char player)
    {
        foreach (var line in Lines)
        {
            int pCount = 0;
            int emptyIdx = -1;
            foreach (int idx in line)
            {
                if (board[idx] == player) pCount++;
                else if (board[idx] == ' ') emptyIdx = idx;
            }
            if (pCount == 2 && emptyIdx != -1) return emptyIdx;
        }
        return -1;
    }

    private static bool IsBoardEmpty(char[] board)
    {
        for (int i = 0; i < 9; i++)
            if (board[i] != ' ') return false;
        return true;
    }

    private static bool IsEmpty(char[] board, int i) => board[i] == ' ';

    private static bool OpponentControlsAnyCorner(char[] board, char opp)
    {
        foreach (int c in Corners) if (board[c] == opp) return true;
        return false;
    }

    private static bool AIControlsAnyCorner(char[] board, char ai)
    {
        foreach (int c in Corners) if (board[c] == ai) return true;
        return false;
    }

    private static int TakeEdgeAdjacentToOwnedCorner(char[] board, char ai)
    {
        (int corner, int e1, int e2)[] map = {
            (0, 1, 3), (2, 1, 5), (6, 3, 7), (8, 5, 7)
        };

        foreach (var entry in map)
        {
            if (board[entry.corner] == ai)
            {
                if (board[entry.e1] == ' ') return entry.e1;
                if (board[entry.e2] == ' ') return entry.e2;
            }
        }
        return -1;
    }

    private static List<int> GetEmptyCells(char[] board)
    {
        var list = new List<int>(5);
        for (int i = 0; i < 9; i++)
            if (board[i] == ' ') list.Add(i);
        return list;
    }
}