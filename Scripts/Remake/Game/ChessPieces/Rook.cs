using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int[] newX = { currentX - 1, currentX, currentX + 1 };
        int[] newY = { currentY - 1, currentY, currentY + 1 };
        int[] orderlist = { 1, 0, 1, 2, 0, 1, 2, 1 };

        for (int index = 0; index < 4; index++) {
            int x = newX[orderlist[index * 2]];
            int y = newY[orderlist[index * 2 + 1]];

            bool isX = orderlist[index * 2] != 1;
            bool isPlus = (isX ? orderlist[index * 2] : orderlist[index * 2 + 1]) == 2;

            getAvailableLine(board, tileCountX, tileCountY, x, y, isPlus, isX, r); 
        }

        return r; 
    
    }

    private void getAvailableLine(ChessPiece[,] board, int tileCountX, int tileCountY, int x, int y, bool isPlus, bool isX, List<Vector2Int> r)
    {
        for (int pos = isX? x:y ; ((pos < (isX? tileCountX: tileCountY)) && pos >= 0);)
        {
            Vector2Int newPos = isX? new Vector2Int(pos, y) : new Vector2Int(x, pos);
            ChessPiece piece = isX? board[pos, y] : board[x, pos];

            if (piece == null)
                r.Add(newPos);

            if (piece != null)
            {
                if (!IsFromSameTeam(piece))
                    r.Add(newPos);

                break;
            }
            pos = isPlus ? pos + 1 : pos - 1;
        }
    }
}

