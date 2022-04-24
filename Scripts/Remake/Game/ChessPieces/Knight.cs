using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        int[] newX = new int[8], newY = new int[8];

        for (int i = 0, z = 0; i < 8; z++, i++)
        {
            // generate the x & y axis for the checking order
            newX[i] = currentX + ((z % 2 == 0) ? 1 : -1);
            newY[i] = currentY + ((z < 2) ? 2 : -2);
            
            if (i > 3)
            {
                newX[i] = currentX + ((z % 2 == 0) ? 2 : -2);
                newY[i] = currentY + ((z < 6) ? 1 : -1);
            }

            int x = newX[i], y = newY[i];

            if (x >= 0 && y >= 0 && x < tileCountX && y < tileCountY)
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
}
