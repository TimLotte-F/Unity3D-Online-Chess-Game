using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int[] newX = { currentX - 1, currentX, currentX + 1 };
        int[] newY = { currentY - 1, currentY, currentY + 1 };

        foreach (int x in newX)
            foreach (int y in newY)
            {
                if (x < tileCountX && y < tileCountY && x >= 0 && y >= 0)
                {
                    Vector2Int newMove = new Vector2Int(x, y);
                    ChessPiece newPos = board[x, y];

                    if (newPos == null)
                        r.Add(newMove);

                    if (newPos != null && !IsFromSameTeam(newPos))
                        r.Add(newMove);
                }
            }
        return r;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        SpecialMove r = SpecialMove.None;
        int yPos = (team == 0) ? 0 : 7;
        // check the move list is it include the king, left and right rook.
        var kingMove = moveList.Find(m => m[0].x == 4 && m[0].y == yPos);
        var leftRook = moveList.Find(m => m[0].x == 0 && m[0].y == yPos);
        var rightRook = moveList.Find(m => m[0].x == 7 && m[0].y == yPos);

        if (kingMove == null && currentX == 4)
        {
            // Left & right rooks
            for (int i = 0; i < 2; i++)
            {
                ChessPiece piece = board[(i == 1) ? 0 : 7, yPos];

                if ((isRookAvailable(piece, (i == 1) ? leftRook : rightRook, team)) && (isAvailableMove(board, (i == 1) ? 3 : 6, piece.currentY)))
                {
                    availableMoves.Add(new Vector2Int((i == 1) ? 2 : 6, piece.currentY));
                    r = SpecialMove.Castling;
                }
            }
        }
        return r;
    }

    private bool isAvailableMove(ChessPiece[,] board, int x, int y)
    {
        for (int newX = x;
            (newX >= ((x == 3) ? 1 : 5) && newX <= ((x == 3) ? 3 : 6));
            newX--)
            if (board[newX, y] != null)
                return false;
        return true;
    }

    private bool isRookAvailable(ChessPiece piece, Vector2Int[] Rook, int teamNumber)
    {
        return (piece != null && piece.type == ChessPieceType.Rook && Rook == null && piece.team == teamNumber);
    }
}
