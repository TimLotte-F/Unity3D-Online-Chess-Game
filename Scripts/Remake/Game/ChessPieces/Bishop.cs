using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int[] newXY = { currentX - 1, currentX + 1, currentY - 1, currentY + 1 };
        int[] orderList = { 0, 2, 1, 3, 0, 3, 1, 2 };

        for (int index = 0; index < 4; index++)
        {
            int x = newXY[orderList[index * 2]];
            int y = newXY[orderList[index * 2 + 1]];
            getAvailableLine(board, 
                tileCountX, tileCountY, 
                x, y, orderList[index * 2] % 2 != 0, orderList[index * 2 + 1] % 2 != 0, r);
        }
        return r;
    }

    private void getAvailableLine(ChessPiece[,] board, int tileCountX, int tileCountY, int x, int y, bool isXPlus, bool isYPlus, List<Vector2Int> r)
    {
        for (int newX = x, newY = y; (newX < tileCountX && newY < tileCountX && newX >= 0 && newY >= 0);)
        {
            Vector2Int newMove = new Vector2Int(newX, newY);
            ChessPiece newPos = board[newX, newY];

            if (newPos == null)
                r.Add(newMove);

            if (newPos != null)
            {
                if (!IsFromSameTeam(newPos))
                    r.Add(newMove);
                break;
            }
            newX = isXPlus ? newX + 1 : newX - 1;
            newY = isYPlus ? newY + 1 : newY - 1;
        }
    }
}
