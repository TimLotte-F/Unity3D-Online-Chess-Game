using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int[] newX = { currentX - 1, currentX, currentX + 1 };
        int[] newY = { currentY - 1, currentY, currentY + 1 };
        int[] orderlist = { 1, 0, 1, 2, 0, 1, 2, 1 };
        int[] orderlist2 = { 0, 0, 2, 2, 0, 2, 2, 0 };

        for (int index = 0; index < 4; index++)
        {
            int x1 = newX[orderlist[index * 2]], y1 = newY[orderlist[index * 2 + 1]];
            int x2 = newX[orderlist2[index * 2]], y2 = newY[orderlist2[index * 2 + 1]];

            bool isX = orderlist[index * 2] != 1;
            bool isPlus = ((isX ? orderlist[index * 2] : orderlist[index * 2 + 1]) == 2);

            bool isXPlus = orderlist2[index * 2] == 2;
            bool isYPlus = orderlist2[index * 2 + 1] == 2;

            getAvailableLine(board, tileCountX, tileCountY, x1, y1, isPlus, isX, r);
            getAvailableLine2(board, tileCountX, tileCountY, x2, y2, isXPlus, isYPlus, r);

        }

        return r;
    }

    private void getAvailableLine(ChessPiece[,] board, int tileCountX, int tileCountY, int x, int y, bool isPlus, bool isX, List<Vector2Int> r)
    {
        for (int pos = isX ? x : y; ((pos < (isX ? tileCountX : tileCountY)) && pos >= 0);)
        {
            Vector2Int newMove = isX ? new Vector2Int(pos, y) : new Vector2Int(x, pos);
            ChessPiece newPos = isX ? board[pos, y] : board[x, pos];

            if (newPos == null)
                r.Add(newMove);

            if (newPos != null)
            {
                if (!IsFromSameTeam(newPos))
                    r.Add(newMove);

                break;
            }
            pos = isPlus ? pos + 1 : pos - 1;
        }
    }
    private void getAvailableLine2(ChessPiece[,] board, int tileCountX, int tileCountY, int x, int y, bool isXPlus, bool isYPlus, List<Vector2Int> r)
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
