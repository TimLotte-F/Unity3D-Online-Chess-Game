using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private Camera currentCamera;
    public bool isGameStart = false;

    public int BOARD_SIZE = 8;
    public GameObject[,] tiles;
    public ChessPiece[,] chessPieces;

    private Vector2Int currentHover;
    private float dragOffset = 1.5f;


    private ChessGameController chessController;
    private bool isWhiteTurn;
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deathWhites = new List<ChessPiece>();
    private List<ChessPiece> deathBlacks = new List<ChessPiece>();

    private SpecialMove specialMove;
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }
        if (!isGameStart)
        {
            return;
        }


        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover", "Highlight")))
        {
            // get the indexed of the tile ive hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // if we're hovering a tile after not hovering a tiles;
            if (currentHover == -Vector2Int.one)
            {
                FirstTimeHover(hitPosition);
            }

            // if we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                HoverTile(hitPosition);
            }

            // if we press down on the mouse
            if (Input.GetMouseButtonDown(0))
            {
                DraggingThePiece(hitPosition);
            }

            if (Input.GetMouseButtonUp(0) && currentlyDragging != null)
            {
                ReleaseDraggingPiece(hitPosition);
            }
        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                MoveOutTile();
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0))
            {
                CancelMove(currentlyDragging.currentX, currentlyDragging.currentY);
            }
        }

        if (currentlyDragging)
        {
            RaiseUpPiece(ray);
        }
    }
    public void Init()
    {
        BoardSetup.instance.GenerateAllTiles(this, BOARD_SIZE, BOARD_SIZE, "Tile");
        chessController = GetComponent<ChessGameController>();
        chessController.SetGameBoard(this);

        isWhiteTurn = true;
        isGameStart = true;
    }
    public void FirstTimeHover(Vector2Int hitPosition)
    {
        // First time in game without any hover activities 
        currentHover = hitPosition;
        tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");

    }

    public void HoverTile(Vector2Int hitPosition)
    {
        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer((ContainsValidMove(ref availableMoves, currentHover)) ? "Highlight" : "Tile");
        currentHover = hitPosition;
        tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");

    }
    public void DraggingThePiece(Vector2Int hitPosition)
    {
        ChessPiece cp = chessPieces[hitPosition.x, hitPosition.y];
        if (HasPiece(cp) && NetworkTurnManager.Instance.gameState == GameState.Playing)
        {
            // is it our trun?
            if ((cp.team == 0 && NetworkTurnManager.Instance._isWhiteTurn && NetworkTurnManager.Instance.isWhitePlayer())
                || (cp.team == 1 && !NetworkTurnManager.Instance._isWhiteTurn && NetworkTurnManager.Instance.isBlackPlayer()))
            {
                currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];

                // get a list of where ia can go, highlight tiles as well
                availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, BOARD_SIZE, BOARD_SIZE);
                // get a list of special moves as well
                specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);
                if (isKingUnderAttack() && cp.type == ChessPieceType.King && specialMove == SpecialMove.Castling)
                {
                    removeCastlingMoves(cp);
                }

                PreventCheck();

                HighlightTiles();
            }

        }
    }
    public void ReleaseDraggingPiece(Vector2Int hitPosition)
    {
        Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

        if (ContainsValidMove(ref availableMoves, new Vector2Int(hitPosition.x, hitPosition.y)))
        {
            MoveTo(previousPosition.x, previousPosition.y, hitPosition.x, hitPosition.y);
        }
        else
        {
            CancelMove(previousPosition.x, previousPosition.y);
        }

    }
    public void MoveOutTile()
    {

        tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer((ContainsValidMove(ref availableMoves, currentHover)) ? "Highlight" : "Tile");
        currentHover = -Vector2Int.one;

    }
    public void CancelMove(int previousPosition_X, int previousPosition_Y)
    {
        currentlyDragging.SetPosition(BoardSetup.instance.GetTileCenter(previousPosition_X, previousPosition_Y));
        ResetDragging();

    }
    public void RaiseUpPiece(Ray ray)
    {
        Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * BoardSetup.instance.yOffset);
        float distance = 0.0f;
        if (horizontalPlane.Raycast(ray, out distance))
        {
            currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
        }
    }


    public Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < BOARD_SIZE; x++)
            for (int y = 0; y < BOARD_SIZE; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one; // handle invalid 
    }

    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int pos)
    {
        for (int i = 0; i < moves.Count; i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }

    // highlight the tiles
    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");

    }
    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");

        availableMoves.Clear();
    }
    private void ResetDragging()
    {
        currentlyDragging = null;
        RemoveHighlightTiles();
    }
    private bool isNotValidPosition(ChessPiece OldChessPiece, ChessPiece NewChessPiece) => HasPiece(NewChessPiece) && OldChessPiece.IsFromSameTeam(NewChessPiece); // oldCp.team == newCp.team
    public ChessPieceType getChessPieceType(ChessPiece Piece) => HasPiece(Piece) ? Piece.type : ChessPieceType.None;
    public bool HasPiece(ChessPiece piece) => piece != null;
    public bool HasPiece(int x, int y) => chessPieces[x, y] != null;

    public void SaveLocalMoveList(int originalX, int originalY, int newX, int newY)
    {
        Vector2Int previousPosition = new Vector2Int(originalX, originalY);
        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(newX, newY) });  // save step
    }
    // send move
    public void MoveTo(int originalX, int originalY, int x, int y)
    {
        ChessPiece cp = chessPieces[originalX, originalY];
        ChessPiece ocp = chessPieces[x, y];
        // if this tile has the pieces
        if (isNotValidPosition(cp, ocp))
            return;
        print("sending movement");
        int movePieceType = (int)cp.type;
        int killPieceType = (int)getChessPieceType(ocp);
        chessController.MovePieceTo(movePieceType, killPieceType, originalX, originalY, x, y, specialMove: (int)specialMove); // kill -> moving -> save local move list
        NetworkTurnManager.Instance.OnMovingPiece(movePieceType, killPieceType, originalX, originalY, x, y, (int)specialMove);
        NetworkTurnManager.Instance.SwitchPlayerTurn();
        ResetDragging();

        return;
    }
    #region Special Moves
    public void ProcessSpecialMove()
    {
        Vector2Int[] lastMove = GetLastMove();
        int lastX = lastMove[1].x;
        int lastY = lastMove[1].y;

        if (specialMove == SpecialMove.EnPassant)
        {
            ChessPiece myPawn = getLastMovedPiece();

            var targetPawnPosition = moveList[moveList.Count - 2];
            ChessPiece enemyPiece = chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];

            if (myPawn.currentX == enemyPiece.currentX)
            {
                if (myPawn.currentY == enemyPiece.currentY - 1 || myPawn.currentY == enemyPiece.currentY + 1)
                    atePiece(enemyPiece, enemyPiece.team == 0);

                chessPieces[enemyPiece.currentX, enemyPiece.currentY] = null;
            }
        }

        if (specialMove == SpecialMove.Promotion && NetworkTurnManager.Instance.isMyturn())
        {
            ChessPiece targetPawn = getLastMovedPiece();

            if (targetPawn.type == ChessPieceType.Pawn && (targetPawn.currentY == 0 || targetPawn.currentY == 7))
            {
                // open the windows 
                PromotionWindow.instance.OpenWindow("Pawn's Promotion");
            }
        }

        if (specialMove == SpecialMove.Castling)
        {
            if (lastY == 0 || lastY == 7)
            {
                if (lastX == 2)
                    castlingMove(chessPieces, 0, lastY);
                if (lastX == 6)
                    castlingMove(chessPieces, 7, lastY);
            }
        }
    }
    private void castlingMove(ChessPiece[,] chessPieces, int oldX, int y)
    {
        int newX = 0;
        if (oldX == 0) newX = 3;
        if (oldX == 7) newX = 5;

        if (newX == 3 || newX == 5)
        {
            ChessPiece rook = chessPieces[oldX, y];
            chessPieces[newX, y] = rook; // [5,7] [3,7] [3,0] [5,0]
            chessController.PositionSinglePiece(newX, y);
            chessPieces[oldX, y] = null; //clean old position
        }

    }

    public void pawnPromotion(ChessPieceType type) // move to -> send Special Move (False) -> process Special -> show window -> select type -> send Message -> received message -> paw
    {
        ChessPiece targetPawn = getLastMovedPiece();

        ChessPiece newPiece = chessController.piecesCreator.SpawnSinglePieces(type, targetPawn.team);
        newPiece.transform.position = targetPawn.transform.position;

        Destroy(targetPawn.gameObject);
        chessPieces[targetPawn.currentX, targetPawn.currentY] = newPiece;
        chessController.PositionSinglePiece(targetPawn.currentX, targetPawn.currentY);

    }
    #endregion
    private void atePiece(ChessPiece ocp, List<ChessPiece> deathTeam, int x, int z)
    {
        Vector3 r = Vector3.forward;

        if (ocp.team != (int)TeamColor.Black) // if team number is black, the piece will be placed backward
            r = Vector3.back;

        if (ocp.type == ChessPieceType.King)
            ShowCheckMate(ocp.team == (int)TeamColor.White ? (int)TeamColor.Black : (int)TeamColor.White);

        deathTeam.Add(ocp);
        ocp.SetScale(Vector3.one * BoardSetup.instance.piecesDeathSize);
        ocp.SetPosition(BoardSetup.instance.ResetCoordinate(x, z, r, deathTeam.Count));
    }

    public void atePiece(ChessPiece ocp, bool isWhite)
    {
        if (!HasPiece(ocp)) return;
        // isWhite means that pieces is killed by which team.
        int x = isWhite ? -1 : 8;
        int y = isWhite ? 8 : -1;
        atePiece(ocp, isWhite ? deathWhites : deathBlacks, x, y);
    }

    private void SimulateMoveForSinglePiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing)
    {
        // save the current values, to reset after the function call
        int actualX = cp.currentX, actualY = cp.currentY;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        // going throngh all the oves, simualte them and check if we're in check
        for (int i = 0; i < moves.Count; i++)
        {
            int simX = moves[i].x, simY = moves[i].y;

            Vector2Int kingPositionThisSim = new Vector2Int(targetKing.currentX, targetKing.currentY);

            // did we simulate the king's move
            if (cp.type == ChessPieceType.King)
                kingPositionThisSim = new Vector2Int(simX, simY);

            ChessPiece[,] simulation = new ChessPiece[BOARD_SIZE, BOARD_SIZE];
            List<ChessPiece> simAttackingPieces = new List<ChessPiece>();

            for (int x = 0; x < BOARD_SIZE; x++)
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    if (HasPiece(chessPieces[x, y]))
                    {
                        simulation[x, y] = chessPieces[x, y];
                        if (simulation[x, y].team != cp.team)
                            simAttackingPieces.Add(simulation[x, y]);
                    }
                }

            // simulate tha move
            simulation[actualX, actualY] = null;
            cp.currentX = simX;
            cp.currentY = simY;
            simulation[simX, simY] = cp;

            // did one of the piece got taken down during this simulation
            var deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
            if (HasPiece(deadPiece))
                simAttackingPieces.Remove(deadPiece);

            // get all the simulated attacking pieces moves
            List<Vector2Int> simMoves = new List<Vector2Int>();
            for (int a = 0; a < simAttackingPieces.Count; a++)
            {
                var pieceMoves = simAttackingPieces[a].GetAvailableMoves(ref simulation, BOARD_SIZE, BOARD_SIZE);
                for (int b = 0; b < pieceMoves.Count; b++)
                    simMoves.Add(pieceMoves[b]);
            }

            // is the king in trouble? if so, remove the move
            if (ContainsValidMove(ref simMoves, kingPositionThisSim))
                movesToRemove.Add(moves[i]);

            // resotre the actual cp data
            cp.currentX = actualX;
            cp.currentY = actualY;
        }

        // remove from the current available move list
        for (int i = 0; i < movesToRemove.Count; i++)
            moves.Remove(movesToRemove[i]);
    }
    public bool CheckForCheckmate()
    {
        int targetTeam = (getLastMovedPiece().team == 0) ? 1 : 0;

        List<ChessPiece> attackingPieces = new List<ChessPiece>();
        List<ChessPiece> defendingPieces = new List<ChessPiece>();

        ChessPiece targetKing = null, anotherKing = null;

        for (int x = 0; x < BOARD_SIZE; x++)
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                ChessPiece piece = chessPieces[x, y];
                if (HasPiece(piece))
                {
                    if (piece.team == targetTeam)
                    {
                        defendingPieces.Add(piece);
                        if (piece.type == ChessPieceType.King)
                            targetKing = piece;
                    }
                    else
                    {
                        attackingPieces.Add(piece);
                        if (piece.type == ChessPieceType.King)
                            anotherKing = piece;

                    }
                }
            }

        // is the king attacked right now?
        List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();

        for (int i = 0; i < attackingPieces.Count; i++)
        {
            var pieceMoves = attackingPieces[i].GetAvailableMoves(ref chessPieces, BOARD_SIZE, BOARD_SIZE);

            for (int j = 0; j < pieceMoves.Count; j++)
                currentAvailableMoves.Add(pieceMoves[j]);
            SimulateMoveForSinglePiece(attackingPieces[i], ref currentAvailableMoves, anotherKing);
        }

        // are we in check right now?
        if (ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
        {
            List<Vector2Int> defendingMoves = new List<Vector2Int>();

            for (int i = 0; i < defendingPieces.Count; i++)
            {
                //king is under attack, can we move something to help him?
                defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, BOARD_SIZE, BOARD_SIZE);

                // since we're sending ref availableMoves, we will be deleting moves that are putting us in check.
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, targetKing);

                if (defendingMoves.Count > 0)
                    return false;
            }

            return true; // checkmate exit
        }
        return false;
    }
    public bool CheckStalemate()
    {
        bool isWhiteTurn = NetworkTurnManager.Instance._isWhiteTurn;
        return isStalemate(GetPieces(isWhiteTurn), GetKing(isWhiteTurn)) && !isKingUnderAttack();
    }
    private bool isStalemate(List<ChessPiece> defendingPieces, ChessPiece targetKing)
    {
        foreach (var piece in defendingPieces)
        {
            List<Vector2Int> defendingMoves = piece.GetAvailableMoves(ref chessPieces, BOARD_SIZE, BOARD_SIZE);
            SimulateMoveForSinglePiece(piece, ref defendingMoves, targetKing);
            if (defendingMoves.Count > 0)
                return false;
        }
        return true;
    }
    private bool isKingUnderAttack()
    {

        isWhiteTurn = NetworkTurnManager.Instance._isWhiteTurn;
        ChessPiece king = GetKing(isWhiteTurn);
        List<ChessPiece> enemyPieces = GetPieces(!isWhiteTurn); // check enemy team

        foreach (var piece in enemyPieces)
        {
            List<Vector2Int> pieceMoves = piece.GetAvailableMoves(ref chessPieces, BOARD_SIZE, BOARD_SIZE);
            if (pieceMoves.Contains(king.GetVector2Int_Position()))
            {
                return true;
            }
        }

        return false;
    }
    private void PreventCheck()
    {
        ChessPiece targetKing = null;

        for (int x = 0; x < BOARD_SIZE; x++)
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                ChessPiece cp = chessPieces[x, y];
                if (HasPiece(cp) && cp.type == ChessPieceType.King && cp.IsFromSameTeam(currentlyDragging))
                    targetKing = cp;
            }
        // since we're sending ref availableMoves, we will be deleting moves that are puttin us in check
        SimulateMoveForSinglePiece(currentlyDragging, ref availableMoves, targetKing);
    }
    private void removeCastlingMoves(ChessPiece currentking)
    {
        // remove the caslting move in King's available moves 
        specialMove = SpecialMove.None;

        for (int i = 0; i < 2; i++)
        {
            Vector2Int special = new Vector2Int((i == 1) ? 2 : 6, currentking.currentY);
            if (availableMoves.Contains(special))
                availableMoves.Remove(special);
        }
    }
    private List<ChessPiece> GetPieces(bool isWhite)
    {
        List<ChessPiece> pieces = new List<ChessPiece>();
        int teamNumber = isWhite ? 0 : 1;

        for (int x = 0; x < BOARD_SIZE; x++)
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                ChessPiece cp = chessPieces[x, y];
                if (HasPiece(cp) && cp.team == teamNumber)
                    pieces.Add(cp);
            }

        return pieces;
    }
    private ChessPiece GetKing(bool isWhite)
    {
        return GetPieces(isWhite).Find(x => (x.type == ChessPieceType.King));
    }
    private ChessPiece getLastMovedPiece()
    {
        Vector2Int[] lastMove = GetLastMove();
        int lastX = lastMove[1].x;
        int lastY = lastMove[1].y;

        return chessPieces[lastX, lastY];
    }

    private Vector2Int[] GetLastMove() => moveList[moveList.Count - 1];
    public TeamColor getLastMoveTeamColor()
    {
        return getLastMovedPiece().team == 0 ? TeamColor.White : TeamColor.Black;
    }

    public void SetSpecialMove(SpecialMove specialMove)
    {
        this.specialMove = specialMove;
    }

    #region Reset Game Info
    // UI control
    private void ShowCheckMate(int team)
    {
        // DisplayVictory(team);
    }

    // Game reset game object
    public void GameReset()
    {
        // fields reset
        ResetDragging();
        availableMoves.Clear();
        moveList.Clear();
        initDeathPiecesLists();

        isGameStart = true;
        isWhiteTurn = NetworkTurnManager.Instance._isWhiteTurn;
    }
    public void initDeathPiecesLists()
    {
        initPieceList(deathBlacks);
        initPieceList(deathWhites);
    }
    private void initPieceList(List<ChessPiece> deathPiecesList)
    {
        if (deathPiecesList.Count > 0)
        {
            deathPiecesList.ForEach(cp => Destroy(cp.gameObject));
            deathPiecesList.Clear();
        }
    }

    #endregion
}
