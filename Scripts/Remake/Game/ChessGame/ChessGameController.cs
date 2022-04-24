
using UnityEngine;

public class ChessGameController : MonoBehaviour
{
    private Board board;
    public bool isWhiteTurn;

    public PiecesCreator piecesCreator;
    private void Awake()
    {
        if (piecesCreator == null)
            piecesCreator = GetComponent<PiecesCreator>();
    }
    public void SetGameBoard(Board board) => this.board = board;


    // positioning
    public void PositionAllPieces()
    {
        if (board != null)
        {
            for (int x = 0; x < board.BOARD_SIZE; x++)
                for (int y = 0; y < board.BOARD_SIZE; y++)
                    if (board.chessPieces[x, y] != null)
                        PositionSinglePiece(x, y, true);
        }
    }

    public void PositionSinglePiece(int x, int y, bool force = false)
    {
        board.chessPieces[x, y].currentX = x;
        board.chessPieces[x, y].currentY = y;
        board.chessPieces[x, y].SetPosition(BoardSetup.instance.GetTileCenter(x, y), force);
    }

    public void InitChessInBoard()
    {
        if (board.chessPieces != null)
        {
            // Clean up
            for (int x = 0; x < board.BOARD_SIZE; x++)
            {
                for (int y = 0; y < board.BOARD_SIZE; y++)
                {
                    if (board.chessPieces[x, y] != null)
                        Destroy(board.chessPieces[x, y].gameObject);
                    board.chessPieces[x, y] = null;
                }
            }
            board.initDeathPiecesLists();
        }
        else
        {
            // Debug.Log("The board or chessPieces is null, cannot clean up.");
        }
    }

    public void InitPieces()
    {
        if (board == null)
        {
            Debug.Log("The board is null, cannot init pieces.");
        }
        else
        {
            piecesCreator.SpawnAllPieces(board);
            PositionAllPieces();
        }
    }

    public void RestartGame()
    {
        InitChessInBoard();
        InitPieces();
        ResetChessBoardInfo();
    }

    public void ResetChessBoardInfo() => board.GameReset(); // it is using on reset gameobject for chess game rather than being on networking
    public Board getBoard() => this.board;

    #region Handling the Pieces of Movement on Network

    public void MovePieceTo(int movePieceType, int killPieceType, int originalX, int originalY, int x, int y, int specialMove = (int)SpecialMove.None)
    {
        ChessPiece ocp = board.chessPieces[x, y];
        killPiece(killPieceType, ocp);
        MovingPiece(originalX, originalY, x, y);
        board.SaveLocalMoveList(originalX, originalY, x, y); // updated move list
        NetworkTurnManager.Instance.SaveStep(movePieceType, killPieceType, originalX, originalY, x, y, specialMove.ToString());
        if (specialMove != (int)SpecialMove.None)
        {
            MovingSpecialMove((SpecialMove)specialMove);
        }

    }
    public void MovingSpecialMove(SpecialMove specialMove)
    {

        board.SetSpecialMove(specialMove);
        board.ProcessSpecialMove(); // processing Special move 

    }

    public void MovingPiece(int previousX, int previousY, int newX, int newY)
    {
        board.chessPieces[newX, newY] = board.chessPieces[previousX, previousY];
        board.chessPieces[previousX, previousY] = null;
        PositionSinglePiece(newX, newY);
    }

    public void killPiece(int killPieceType, ChessPiece ocp)
    {
        if (killPieceType != (int)ChessPieceType.None)
            board.atePiece(ocp, ocp.team == (int)TeamColor.White);
    }
    #endregion
}
