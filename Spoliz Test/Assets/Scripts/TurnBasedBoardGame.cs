using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class TurnBasedBoardGame : MonoBehaviourPunCallbacks
{
    private const int GRID_SIZE = 3;
    private int[,] board = new int[GRID_SIZE, GRID_SIZE];
    private int currentPlayer = 1;
    public TMP_Text statusText;
    public TMP_Text playerInfoText;
    public Button buttonPrefab;
    public Button LeaveGame;
    public Transform boardParent;
    private Button[,] boardButtons = new Button[GRID_SIZE, GRID_SIZE];
    bool gameOver;

    void Start()
    {
        LeaveGame.onClick.AddListener(()=> 
        {
            OnLeaveGame();
            EventHandler.LeaveRoom?.Invoke();
        });
        InitializeBoard();
        UpdateTurnText();
        UpdatePlayerInfo();
        EventHandler.UpdateGameState?.Invoke(gameOver);
    }
 
    private void InitializeBoard()
    {
        for (int x = 0; x < GRID_SIZE; x++)
        {
            for (int y = 0; y < GRID_SIZE; y++)
            {
                Button btn = Instantiate(buttonPrefab, boardParent);
                btn.name = "Cell_" + x + "_" + y;
                int xPos = x, yPos = y;
                btn.onClick.AddListener(() => MakeMove(xPos, yPos));
                boardButtons[x, y] = btn;
            }
        }
    }

    public void MakeMove(int x, int y)
    {
        if (gameOver || boardButtons[x, y] == null) return;

        if (!PhotonNetwork.InRoom || photonView == null)
        {
            Debug.LogError("PhotonView or Room not available!");
            return;
        }

        int playerActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        int expectedPlayer = (currentPlayer == 1) ? 1 : 2;

        if (playerActorNumber != expectedPlayer)
        {
            statusText.text = "Not your turn!";
            return;
        }

        if (board[x, y] == 0)
        {
            photonView.RPC("SyncMove", RpcTarget.AllBuffered, x, y, currentPlayer);
        }
        else
        {
            statusText.text = "Invalid move!";
        }
    }

    [PunRPC]
    void SyncMove(int x, int y, int player)
    {
        board[x, y] = player;
        boardButtons[x, y].GetComponentInChildren<TMP_Text>().text = (player == 1) ? "X" : "O";
        CheckWinCondition();
        SwitchTurn();
    }
    [PunRPC]
    void SyncWin(int Player)
    {
        if (currentPlayer == 1)
            statusText.text = "Player 2 Wins";
        else
            statusText.text = "Player 1 Wins";
        EventHandler.UpdateGameState?.Invoke(true);
    }

    private void SwitchTurn()
    {
        if (!gameOver)
        {
            currentPlayer = (currentPlayer == 1) ? 2 : 1;
            UpdateTurnText();
        }
    }

    private void UpdateTurnText()
    {
        statusText.text = "Player " + currentPlayer + "'s Turn";
    }

    private void CheckWinCondition()
    {
        for (int i = 0; i < GRID_SIZE; i++)
        {
            if (board[i, 0] != 0 && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
            {
                DeclareWinner(board[i, 0]);
                return;
            }
            if (board[0, i] != 0 && board[0, i] == board[1, i] && board[1, i] == board[2, i])
            {
                DeclareWinner(board[0, i]);
                return;
            }
        }
        if (board[0, 0] != 0 && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
        {
            DeclareWinner(board[0, 0]);
            return;
        }
        if (board[0, 2] != 0 && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
        {
            DeclareWinner(board[0, 2]);
            return;
        }
    }
    private void UpdatePlayerInfo()
    {
        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        playerInfoText.text = "You are Player " + playerNumber;
    }

    private void DeclareWinner(int player)
    {
        statusText.text = "Player " + player + " Wins!";
        gameOver = true;
    }
    public void OnLeaveGame() 
    {
        gameOver = true;
        photonView.RPC("SyncWin", RpcTarget.AllBuffered, currentPlayer);
        Debug.Log($"game over in gameplay {gameOver}");
      
    }
  
}
