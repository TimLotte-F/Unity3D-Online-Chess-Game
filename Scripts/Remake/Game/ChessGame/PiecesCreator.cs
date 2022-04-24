using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecesCreator : MonoBehaviour
{
    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;
    public ChessPiece SpawnSinglePieces(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        return cp;
    }
    public void SpawnPairPieces(ChessPiece[,] cp, int x, int y, int type)
    {
        cp[x, y] = SpawnSinglePieces((ChessPieceType)type, (int)TeamColor.White);
        cp[x, 7 - y] = SpawnSinglePieces((ChessPieceType)type, (int)TeamColor.Black);
    }
    public void SpawnAllPieces(Board board)
    {
        int[] pieceTypes = { 2, 3, 4, 5, 6, 4, 3, 2 };
        ChessPiece[,] chessPieces = new ChessPiece[board.BOARD_SIZE, board.BOARD_SIZE];

        for (int x = 0; x < pieceTypes.Length; x++)
        {
            SpawnPairPieces(chessPieces, x, (int)TeamColor.Black, 1); // type 1 is a Pawn.
            SpawnPairPieces(chessPieces, x, (int)TeamColor.White, pieceTypes[x]);
        }
        board.chessPieces = chessPieces;
    }


}
