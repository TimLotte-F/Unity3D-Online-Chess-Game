using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 0) ? 1 : -1;
        int newY = currentY + direction;
        int[] newX = { currentX - 1, currentX + 1 };

        if (newY >= 0 && newY < tileCountY)
        {
            // two in front
            if (board[currentX, newY] == null)
            {
                if (team == 0 && currentY == 1 && board[currentX, currentY + (direction * 2)] == null)
                    r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
                if (team == 1 && currentY == 6 && board[currentX, currentY + (direction * 2)] == null)
                    r.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }

            //one in front if possible
            if (board[currentX, newY] == null)
                r.Add(new Vector2Int(currentX, newY));

            // kills move  
            foreach (int x in newX)
            {
                if (x >= 0 && x < tileCountX)
                {
                    ChessPiece newPos = board[x, newY];
                    addKillMove(newPos, r);
                }
            }
        }
        return r;
    }

    private void addKillMove(ChessPiece newPos, List<Vector2Int> r)
    {
        if (newPos != null && !IsFromSameTeam(newPos))
            r.Add(new Vector2Int(newPos.currentX, newPos.currentY));
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        int direction = (team == 0) ? 1 : -1;
        int newY = currentY + direction;
        int[] newX = { currentX - 1, currentX + 1 };

        if ((team == 0 && currentY == 6) || (team == 1 && currentY == 1))
            return SpecialMove.Promotion;

        // En Passant
        if (moveList.Count > 0)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            if (board[lastMove[1].x, lastMove[1].y].type == ChessPieceType.Pawn) {  // if the last piece moved was a pawn

                if ( (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2)                // if the last move was a +2 in either direction
                    && (board[lastMove[1].x, lastMove[1].y].team != team)           // if the move was from the other team
                    && (lastMove[1].y == currentY) )                                // if both pawns are on the same Y
                {
                    foreach (int x in newX) {
                        if (lastMove[1].x == x) {
                            availableMoves.Add(new Vector2Int(x, newY));
                            return SpecialMove.EnPassant;
                        }
                    }
                }
                
            }
        }

        return SpecialMove.None;
    }
}
