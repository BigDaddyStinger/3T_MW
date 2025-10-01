using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public Button[] cellButtons;
    public TMP_Text[] cellLabels;
    public Button makeMoveButton;
    public Button newGameButton;
    public TMP_Text statusText;

    public char humanMark = 'X';
    public char aiMark = 'O';

    private char[] board = new char[9];
    private char currentTurn;
    private System.Random rng;

    private static readonly int[][] Lines = new int[][]
    {
        new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8}, // rows
        new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8}, // cols
        new[] {0,4,8}, new[] {2,4,6}                 // diagonals
    };

    private void Awake()
    {
        rng = new System.Random();
        InitUIHooks();
        NewGame();
    }

    private void InitUIHooks()
    {
        if (makeMoveButton) makeMoveButton.onClick.AddListener(OnMakeMoveButton);
        if (newGameButton) newGameButton.onClick.AddListener(NewGame);
    }

    public void NewGame()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i] = ' ';
            if (cellLabels != null && i < cellLabels.Length && cellLabels[i] != null)
                cellLabels[i].text = "";
            if (cellButtons != null && i < cellButtons.Length && cellButtons[i] != null)
                cellButtons[i].interactable = true;
        }

        currentTurn = humanMark;
        SetStatus($"{currentTurn} to move");
        SetBoardInteractable(true);
    }

    private void SetStatus(string s)
    {
        if (statusText) statusText.text = s;
    }

    private void SetBoardInteractable(bool canClick)
    {
        if (cellButtons == null) return;
        foreach (var b in cellButtons)
            if (b) b.interactable = canClick && b.GetComponentInChildren<TMP_Text>().text == "";
    }

    public void OnCellClicked(int index)
    {
        if (!IsGameActive()) return;
        if (board[index] != ' ') return;
        if (currentTurn != humanMark) return;

        PlaceMark(index, humanMark);

        if (!IsGameActive())
            return;

        //OnMakeMoveButton();
    }

    public void OnMakeMoveButton()
    {
        if (!IsGameActive()) return;

        char aiForThisTurn = currentTurn;
        char other = (aiForThisTurn == 'X') ? 'O' : 'X';

        int move = TTTAI.GetBestMove(board, aiForThisTurn, other, rng);
        if (move >= 0)
        {
            PlaceMark(move, aiForThisTurn);
        }
    }

    private void PlaceMark(int index, char mark)
    {
        board[index] = mark;

        if (cellLabels != null && index < cellLabels.Length && cellLabels[index] != null)
            cellLabels[index].text = mark.ToString();

        if (cellButtons != null && index < cellButtons.Length && cellButtons[index] != null)
            cellButtons[index].interactable = false;

        if (HasWon(mark))
        {
            SetStatus($"{mark} wins!");
            EndGame();
            return;
        }
        if (IsBoardFull())
        {
            SetStatus("Tie!");
            EndGame();
            return;
        }

        currentTurn = (currentTurn == 'X') ? 'O' : 'X';
        SetStatus($"{currentTurn} to move");
        RefreshInteractable();
    }

    private void RefreshInteractable()
    {
        if (cellButtons == null) return;
        for (int i = 0; i < 9; i++)
        {
            if (cellButtons[i] == null) continue;
            bool empty = (board[i] == ' ');

            cellButtons[i].interactable = empty && IsGameActive();
        }
    }

    private bool HasWon(char mark)
    {
        foreach (var line in Lines)
        {
            if (board[line[0]] == mark && board[line[1]] == mark && board[line[2]] == mark)
                return true;
        }
        return false;
    }

    private bool IsBoardFull()
    {
        for (int i = 0; i < 9; i++) if (board[i] == ' ') return false;
        return true;
    }

    private bool IsGameActive()
    {
        return !HasWon('X') && !HasWon('O') && !IsBoardFull();
    }

    private void EndGame()
    {
        SetBoardInteractable(false);
    }
}
