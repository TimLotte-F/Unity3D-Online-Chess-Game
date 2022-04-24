
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using Photon.Pun;
using System.Linq;
using System.Collections;

public class NetworkTurnManager : PunTurnManager, IPunTurnManagerCallbacks
{

    #region Public Variables

    [Tooltip("the duration time of turn")]
    public float TurnTime = 120f;

    [Tooltip("Player Team color")]
    public TeamColor localPlayerType = TeamColor.None;
    public TeamColor currentPlayer = TeamColor.None;
    public bool _isWhiteTurn = true;

    public GameState gameState = GameState.None;


    public struct step
    {
        public int movedType;
        public int killType;

        public int xFrom;
        public int yFrom;
        public int xTo;
        public int yTo;

        public string SpecialMove;

        public step(int _moveId, int _killId, int _xFrom, int _yFrom, int _xTo, int _yTo, string specialMove)
        {
            movedType = _moveId;
            killType = _killId;
            xFrom = _xFrom;
            yFrom = _yFrom;
            xTo = _xTo;
            yTo = _yTo;
            SpecialMove = specialMove;
        }
    }

    [Tooltip("Save the chess pieces steps")]
    public List<step> _steps = new List<step>();

    [Tooltip("Chess Game Controller")]
    public ChessGameController chessGameController;

    public static NetworkTurnManager Instance;

    #endregion

    #region Private Variables

    [Header("Game UIs")]
    [Tooltip("Game Ui View")]
    [SerializeField]
    private RectTransform GameUiView;

    [Tooltip("Button Canvas Group")]
    [SerializeField]
    private CanvasGroup ButtonCanvasGroup;

    [Tooltip("Disconnected Panel")]
    [SerializeField]
    private RectTransform DisconnectedPanel;

    [Tooltip("Request Panel")]
    [SerializeField]
    private RectTransform RequestPanel;

    [Header("Local player")]
    [Tooltip("Local player text")]
    [SerializeField]
    private TextMeshProUGUI LocalPlayerNameText;
    [Tooltip("LocalPlayer Time Text, Local Score Text, Local Turn Text")]
    [SerializeField]
    private TextMeshProUGUI LocalPlayerTimeText, LocalScoreText, LocalTurnText;
    [Tooltip("Local GameStatus Text")]
    [SerializeField]
    private TextMeshProUGUI LocalGameStatusText;

    [Header("Remote Player")]
    [Tooltip("Remote Player Name Text")]
    [SerializeField]
    private TextMeshProUGUI RemotePlayerNameText;

    [Tooltip("Remote Player Time Text, Remote Score Text, Remote Turn Text")]
    [SerializeField]
    private TextMeshProUGUI RemotePlayerTimeText, RemoteScoreText, RemoteTurnText;

    [Tooltip("Remote GameStatus Text")]
    [SerializeField]
    private TextMeshProUGUI RemoteGameStatusText;


    [Header("Picture sprite")]
    [Tooltip("Win Or Loss Image")]
    [SerializeField]
    private Image WinOrLossImage;

    [Tooltip("Sprite Win")]
    [SerializeField]
    private Sprite SpriteWin;

    [Tooltip("Sprite Lose")]
    [SerializeField]
    private Sprite SpriteLose;

    [Tooltip("Sprite Draw")]
    [SerializeField]
    private Sprite SpriteDraw;

    [Tooltip("Next turn")]
    [SerializeField]
    private Sprite Next;

    private ResultType result = ResultType.None;    // result

    private PunTurnManager turnManager; // pun turn manager

    private bool remoteSelection;   // remote player's Selection

    private bool IsShowingResults;	// same  mean as is finished the game

    // handling these two players are playing chess game in whole processing.
    private Player local;
    private Player remote;

    PhotonHandler photonHandler;

    #endregion

    public enum ResultType  // enum for result type
    {
        None = 0,
        Draw,
        LocalWin,
        LocalLoss
    }

    #region Mono Callbacks 


    public void Awake()
    {
        if (turnManager == null)
        {
            this.turnManager = this.gameObject.AddComponent<PunTurnManager>();  // add the component for pun turn manager
            this.turnManager.TurnManagerListener = this;    // listening all callback events 
            this.turnManager.TurnDuration = TurnTime;       // init the turn durationtime.
        }


        if (chessGameController == null)
            chessGameController = GameObject.FindObjectOfType<Board>().GetComponent<ChessGameController>();

        Instance = this;
        RefreshUIViews(); // refresh the in-game UI Views
        if (photonHandler == null)
            photonHandler = gameObject.AddComponent<PhotonHandler>();
        DisconnectedPanel.gameObject.SetActive(false);
    }

    private void RefreshUIViews()
    {
        if (GameUiView.gameObject.activeSelf)
            GameUiView.gameObject.SetActive(PhotonNetwork.InRoom);
        ButtonCanvasGroup.interactable = PhotonNetwork.CurrentRoom != null ? PhotonNetwork.CurrentRoom.PlayerCount > 1 : false;
        UpdatePlayerScoreTexts();
    }

    private void UpdateLocalPlayerInfo(string PlayerName, string PlayerStatus)
    {
        LocalPlayerNameText.text = "Player: " + PlayerName;
        LocalGameStatusText.text = PlayerStatus;
        LocalScoreText.text = "Scores: " + local.GetScore().ToString("D2");
        LocalTurnText.text = "Turn: " + PhotonNetwork.CurrentRoom.GetTurn().ToString();
    }

    private void UpdateRemotePlayerInfo(string PlayerName, string PlayerStatus)
    {
        RemotePlayerNameText.text = "Player: " + PlayerName;
        RemoteGameStatusText.text = PlayerStatus;
        RemoteScoreText.text = "Scores: " + remote.GetScore().ToString("D2");
        RemoteTurnText.text = "Turn: " + PhotonNetwork.CurrentRoom.GetTurn().ToString();
    }

    public void Update()
    {
        // check the are we lost the connection
        if (this.DisconnectedPanel == null)
        {
            Destroy(this.gameObject);
        }

        if (!PhotonNetwork.InRoom || turnManager == null)  // if not in room, stop the update
        {
            return;
        }

        // if pun is connected or is connecting, the reconnect panel would be disabled.
        if (PhotonNetwork.IsConnected && this.DisconnectedPanel.gameObject.activeSelf)
        {
            this.DisconnectedPanel.gameObject.SetActive(false);
        }

        if (!PhotonNetwork.IsConnected && !this.DisconnectedPanel.gameObject.activeSelf)
        {
            this.DisconnectedPanel.gameObject.SetActive(true);
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            if (this.turnManager.IsOver)
            {
                return; // end turn
            }

            if (this.turnManager.Turn > 0 && !IsShowingResults)
            {
                if (Input.GetKeyUp(KeyCode.K))
                {
                    this.turnManager.SendMove("Hello World!", false);

                }
                string remainTime = this.turnManager.RemainingSecondsInTurn.ToString("F1") + " Second";
                if (_isWhiteTurn)
                {
                    if (isWhitePlayer())
                    {
                        LocalPlayerTimeText.text = remainTime;
                        RemotePlayerTimeText.text = "00:00";
                    }
                    if (isBlackPlayer())
                    {
                        RemotePlayerTimeText.text = remainTime;
                        LocalPlayerTimeText.text = "00:00";
                    }
                }
                else
                {
                    if (isWhitePlayer())
                    {
                        RemotePlayerTimeText.text = remainTime;
                        LocalPlayerTimeText.text = "00:00";
                    }
                    if (isBlackPlayer())
                    {
                        LocalPlayerTimeText.text = remainTime;
                        RemotePlayerTimeText.text = "00:00";
                    }
                }
            }
        }

    }
    #endregion

    #region public methods
    public void Judge()
    {
        if (_steps.Count > 0) // prevent empty step in list
        {
            if (chessGameController.getBoard().CheckForCheckmate())
            {
                JudgeWinner();
            }

            if (chessGameController.getBoard().CheckStalemate())
            {
                SetResultDrawAndTexts("StaleMate Draw!");
            }
        }

        if (_steps.Count == 0)
        {
            SetResultDrawAndTexts("No Any Moves, Both Draw!");

        }
    }


    public void JudgeWinner()
    {
        if (getLastMoveTeamColor() == TeamColor.Black) // in Black's turn
        {
            if (isWhitePlayer())
            {
                result = ResultType.LocalLoss;
            }

            if (isBlackPlayer())
            {
                result = ResultType.LocalWin;
            }
            LocalGameStatusText.text = "Black Win !";
        }

        if (getLastMoveTeamColor() == TeamColor.White)
        {
            if (isBlackPlayer())
            {
                result = ResultType.LocalLoss;
            }

            if (isWhitePlayer())
            {
                result = ResultType.LocalWin;
            }
            LocalGameStatusText.text = "White Win !";
        }
    }

    #endregion

    #region TurnManager Callbacks
    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins() turn: " + turn);
        this.LocalTurnText.text = "Turn: " + (this.turnManager.Turn).ToString();
        RemoteTurnText.text = LocalTurnText.text;
        this.WinOrLossImage.gameObject.SetActive(false); // invisible the win or loss image
        IsShowingResults = false;   // disable the result
        ButtonCanvasGroup.interactable = true; // switch the buttons?
    }
    public void OnTurnCompleted(int turn)
    {
        this.Judge();   // check the result
        this.UpdateScores();    // update scores
        this.OnEndTurn();	// finish this turn
    }



    public void OnPlayerMove(Player player, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + player + " turn: " + turn + " action: " + move);
        string strMove = move.ToString();

        if (strMove.Contains("_"))
        {

            if (!player.IsLocal)
            {
                string[] strArr = strMove.Split(char.Parse("_"));
                if (int.Parse(strArr[6]) == (int)SpecialMove.Promotion)
                {
                    chessGameController.MovePieceTo(
                        int.Parse(strArr[0]), int.Parse(strArr[1]),
                        int.Parse(strArr[2]), int.Parse(strArr[3]),
                        int.Parse(strArr[4]), int.Parse(strArr[5])
                        );

                }
            }
        }
        else
        {
            switch (strMove)
            {
                case "Restart":
                    if (!player.IsLocal)
                        PopRequest("Your opponent want to Restart the game");
                    break;
                case "RestartYes":
                    Restart();
                    break;
                case "RestartNo":
                    LocalGameStatusText.text = "Your oppenent reject your request";
                    break;
            }
        }
    }

    public void OnPlayerFinished(Player player, int turn, object move)
    {
        Debug.Log("OnTurnFinished: " + player.ToString() + " turn: " + turn + " action: " + move);
        string tmpStr = move.ToString();

        if (tmpStr.Contains("_"))
        {
            if (!player.IsLocal) // update the opponent move.
            {
                string[] strArr = tmpStr.Split(char.Parse("_"));
                if (strArr[0] == "+Promotion")
                {
                    // format: [0]: "+Promotion", [1]: PieceNewType;
                    chessGameController.getBoard().pawnPromotion((ChessPieceType)(int.Parse(strArr[1])));
                }
                else
                {
                    chessGameController.MovePieceTo(
                        int.Parse(strArr[0]), int.Parse(strArr[1]),
                        int.Parse(strArr[2]), int.Parse(strArr[3]),
                        int.Parse(strArr[4]), int.Parse(strArr[5]),
                        int.Parse(strArr[6])
                        );

                }
                SwitchPlayerTurn();

            }
        }
        else
        {
            switch (tmpStr)
            {
                case "BlackDefeat":
                    RemoteGameStatusText.text = "Black lose!";
                    if (isWhitePlayer())
                    {
                        result = ResultType.LocalWin;
                    }
                    break;
                case "WhiteDefeat":
                    RemoteGameStatusText.text = "White lose!";
                    if (isBlackPlayer())
                    {
                        result = ResultType.LocalWin;
                    }
                    break;
                case "Draw":
                    result = ResultType.Draw;
                    break;
                default:
                    break;
            }
        }
    }

    public void OnTurnTimeEnds(int turn)
    {
        if (!IsShowingResults)
        {

            if (_isWhiteTurn)
            {
                // White over time
                if (isWhitePlayer())
                {
                    result = ResultType.LocalLoss;
                    LocalGameStatusText.text = "Your time is over";
                    RemoteGameStatusText.text = "Victory!";
                }
                if (isBlackPlayer())
                {
                    result = ResultType.LocalWin;
                    LocalGameStatusText.text = "Your opponent's time is over";
                    RemoteGameStatusText.text = "Defeat!";
                }
            }
            else
            {
                // Black over time
                if (isWhitePlayer())
                {
                    result = ResultType.LocalWin;
                    LocalGameStatusText.text = "Your opponent's time is over";
                    RemoteGameStatusText.text = "Defeat!";
                }
                if (isBlackPlayer())
                {
                    result = ResultType.LocalLoss;
                    LocalGameStatusText.text = "Your time is over";
                    RemoteGameStatusText.text = "Victory!";
                }

            }
            OnTurnCompleted(-1);
        }
    }
    #region Core game play methods
    public void StartTurn()
    {
        gameState = GameState.Playing;
        if (PhotonNetwork.IsMasterClient)
        {
            this.turnManager.BeginTurn();
            print("Begin turn: " + this.turnManager.Turn);
        }
        this.turnManager.isTurnStarted = true;

        if (_isWhiteTurn)
        {

            if (isWhitePlayer())
            {
                LocalGameStatusText.text = "Your Turn!";
                RemoteGameStatusText.text = "Waiting...";
            }
            if (isBlackPlayer())
            {
                LocalGameStatusText.text = "Waiting your opponent!";
                RemoteGameStatusText.text = "Thinking...";
            }
            return;

        }
        else
        {
            if (isWhitePlayer())
            {
                LocalGameStatusText.text = "Waiting your opponent!";
                RemoteGameStatusText.text = "Thinking...";
            }
            if (isBlackPlayer())
            {
                LocalGameStatusText.text = "Your Turn!";
                RemoteGameStatusText.text = "Waiting...";
            }
            return;
        }
    }
    private void OnEndTurn()
    {
        ButtonCanvasGroup.interactable = false; // disable buttons switch
        IsShowingResults = true;
        this.turnManager.isTurnStarted = false;
        switch (result) // show the result image
        {
            case ResultType.None:
                this.StartTurn();
                break;
            case ResultType.Draw:
                this.WinOrLossImage.sprite = this.SpriteDraw;
                break;
            case ResultType.LocalWin:
                this.WinOrLossImage.sprite = this.SpriteWin;
                break;
            case ResultType.LocalLoss:
                this.WinOrLossImage.sprite = this.SpriteLose;
                break;
        }
        if (result != ResultType.None)
        {
            this.WinOrLossImage.gameObject.SetActive(true);
            chessGameController.getBoard().isGameStart = false;
            gameState = GameState.Finished;
        }
        else
        {
            ButtonCanvasGroup.interactable = true;
        }

    }
    #endregion

    public void EndGame()
    {
        Debug.Log("EndGame");
        Application.Quit();
    }
    #endregion
    #region button event handle
    public void OnClickConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
        photonHandler.StopFallbackSendAckThread(); // this is used in the demo to timeout in background!
    }
    public void OnClickReConnectAndRejoin()
    {
        PhotonNetwork.ReconnectAndRejoin();
        photonHandler.StopFallbackSendAckThread();  // this is used in the demo to timeout in background!
    }
    public void OnDefeat()
    {
        result = ResultType.LocalLoss;
        if (isBlackPlayer())
        {
            this.turnManager.SendMove("BlackDefeat", true);
        }
        if (isWhitePlayer())
        {
            this.turnManager.SendMove("WhiteDefeat", true);
        }
    }

    #endregion
    private void UpdateScores()
    {
        if (this.result == ResultType.LocalWin)
        {
            PhotonNetwork.LocalPlayer.AddScore(1);
            UpdatePlayerScoreTexts();
        }
    }
    private void UpdatePlayerScoreTexts()
    {
        if (local != null)
        {
            // ToString("D2") formate should be: "00"
            UpdateLocalPlayerInfo(local.NickName, LocalGameStatusText.text);
        }
        if (remote != null)
        {
            UpdateRemotePlayerInfo(remote.NickName, LocalGameStatusText.text);
        }

    }
    void PopRequest(string title)
    {
        if (RequestPanel == null && RequestPanel.gameObject.activeSelf)
        {
            return;
        }
        else
        {
            Debug.Log("PopRequest()");
            RequestPanel.gameObject.SetActive(true);
            // RequestPanel.transform.Find("Title/Text").GetComponent<Text>().text = title;
        }

    }


    #region Pun Call backs
    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom() via networkTurnManager.cs");
        base.OnLeftRoom();
        RefreshUIViews();
        chessGameController.InitChessInBoard();
        gameState = GameState.None;

    }

    public override void OnJoinedRoom()
    {
        local = PhotonNetwork.LocalPlayer;
        remote = PhotonNetwork.LocalPlayer.GetNext();
        gameState = GameState.Room;


        if (remote != null)
        {
            RemoteGameStatusText.text = "Matched sucessfully!";
            // the format should be: "name        00"
            this.RemotePlayerNameText.text = remote.NickName;
        }
        else
        {
            this.RemotePlayerNameText.text = "Matching";
            LocalGameStatusText.text = "Waiting another one...";
            RemoteGameStatusText.text = "Matching";
        }

        RefreshUIViews();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        this.DisconnectedPanel.gameObject.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Debug.Log("Other player arrived");
        RemoteGameStatusText.text = "Welcome, " + newPlayer.NickName + " joined this game!";
        if (remote == null)
        {
            remote = newPlayer;
        }
        RefreshUIViews();
        // play();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Debug.Log("Other player disconnected! isInactive: " + otherPlayer.IsInactive);
        RemoteGameStatusText.text = "Player: " + otherPlayer.NickName + " has left this room.";
        if (!isPlayingGame())
        {

            if (!otherPlayer.IsLocal && !IsShowingResults && _steps.Count > 0)
            {
                result = ResultType.LocalWin;
                UpdateScores();
            }
        }
        if (remote != null)
            remote = null;
        RefreshUIViews();
    }


    #endregion

    #region Controll the Players And UI
    public void CreateLocalPlayer()
    {
        // Before call Play() method
        // when local player has the customeProperties about the team -> local.CustomProperties[PlayerProperties.Team]

        TeamColor LocalTeamColor = (TeamColor)getPlayerTeamColor(local);
        if (LocalTeamColor != TeamColor.None)
        {
            localPlayerType = LocalTeamColor;
            string teamString = isWhitePlayer() ? "White" : "Black";
            string PlayerName = string.Format("{0} :({1})", local.NickName, teamString);
            string LocalGameStatus = string.Format("You is {0} team...", teamString);
            UpdateLocalPlayerInfo(PlayerName, LocalGameStatus);
        }
        else
        {
            Debug.Log("Team color is None, cannot create the Local Player");
        }

    }

    void Play()
    {
        print("Play() methods");

        InitGameBoard();

        // init UI info
        GameUIController.Instance.OnStartGame((CameraAngel)((int)localPlayerType)); // change the camera view
        LocalGameStatusText.text = "Start Game!";
        RemoteGameStatusText.text = "Start Game!";
        UpdatePlayerScoreTexts();
        RefreshUIViews();

        StartCoroutine("MatchSuccessfully");
    }
    IEnumerator MatchSuccessfully()
    {
        yield return new WaitForSeconds(2.5f);
        this.StartTurn();
    }
    public void SwitchPlayerTurn()
    {
        _isWhiteTurn = !_isWhiteTurn;
        currentPlayer = _isWhiteTurn ? TeamColor.White : TeamColor.Black;
    }
    public void StartGame()
    {
        GameUiView.gameObject.SetActive(true);
        // Create Local player -> check the conditions -> setup the time and Board -> Start the Game;
        if (isFullPlayers() && isZeroTurn())
        {
            // when two players, triggers play() methods
            SetRoundTime();
            CreateLocalPlayer();
            Play();
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount > 2) // For guest
        {
            localPlayerType = TeamColor.None;
            LocalGameStatusText.text = "The game has started..";
        }
    }
    private void Restart()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            this.turnManager.RestartTurn();
            _isWhiteTurn = true;

        }
        localPlayerType = (TeamColor)getPlayerTeamColor(local);
        currentPlayer = TeamColor.White;
        // chessGameController.RestartGame();
        CleanAllStep();
        RefreshUIViews();
    }
    public void InitGameBoard()
    {
        gameState = GameState.Init;
        // init player info
        _isWhiteTurn = true;
        result = ResultType.None;
        currentPlayer = TeamColor.White;
        CleanAllStep();

        chessGameController.RestartGame();
    }

    public void OnClickExitButton()
    {
        if (PhotonNetwork.InRoom)
        {
            // OnDefeat();
            PhotonNetwork.LeaveRoom(false);
            Debug.Log("On Click the exit button in Game");
        }
    }
    #endregion
    public void SaveStep(int movePieceTypeNumber, int killPieceTypeNumber, int fromX, int fromY, int toX, int toY, string specialMove)
    {
        step tmpStep = new step();

        tmpStep.movedType = movePieceTypeNumber;
        tmpStep.killType = killPieceTypeNumber;
        tmpStep.xFrom = fromX;
        tmpStep.yFrom = fromY;
        tmpStep.xTo = toX;
        tmpStep.yTo = toY;
        tmpStep.SpecialMove = specialMove;

        _steps.Add(tmpStep);

    }

    public void CleanAllStep()
    {
        if (_steps != null)
            _steps.Clear();
    }
    public TeamColor getLastMoveTeamColor()
    {
        int x = _steps.Last().xTo,
            y = _steps.Last().yTo;

        return chessGameController.getBoard().chessPieces[x, y].team == 0 ? TeamColor.White : TeamColor.Black;
    }

    public bool isZeroTurn() => this.turnManager.Turn == 0;
    public bool isFullPlayers() => PhotonNetwork.CurrentRoom.PlayerCount == 2;

    public bool isMyturn() => localPlayerType == currentPlayer;
    public bool isWhitePlayer() => localPlayerType == TeamColor.White;
    public bool isBlackPlayer() => localPlayerType == TeamColor.Black;
    public bool isPlayingGame()
    {
        return gameState == GameState.Init || gameState == GameState.Playing;
    }
    public int getPlayerTeamColor(Player player)
    {
        if (player != null || player.CustomProperties.ContainsKey(PlayerProperties.Team))
        {
            return (int)player.CustomProperties[PlayerProperties.Team];
        }
        return (int)TeamColor.None;
    }
    public int getRoundTimeInRoom() => (int)PhotonNetwork.CurrentRoom.CustomProperties[RoomProperty.RoundTime] * 60;
    public void SetRoundTime()
    {
        // Before start game, setup the round time for each turn
        // (x) * 60 seconds, x is according to Room Property
        this.turnManager.TurnDuration = getRoundTimeInRoom();
    }
    public void SetResultDrawAndTexts(string messageText)
    {
        if (isWhitePlayer() || isBlackPlayer())
        {
            result = ResultType.Draw;
            LocalGameStatusText.text = messageText;
            RemoteGameStatusText.text = messageText;
            return;
        }
    }
    #region Handling the send move Messages

    public void OnMovingPiece(int movePieceType, int killPieceType, int originalX, int originalY, int x, int y, int specialMove)
    {
        bool isSpecialMove = specialMove != (int)SpecialMove.None;
        bool isPromotionEvent = (specialMove == (int)SpecialMove.Promotion);

        string tmpStr = getMessageFormat(movePieceType, killPieceType, originalX, originalY, x, y);

        tmpStr += "_" + specialMove.ToString();
        if (isSpecialMove)
        {
            this.turnManager.SendMove(tmpStr, !isPromotionEvent);
        }
        else
        {
            this.turnManager.SendMove(tmpStr, true);
        }

    }

    public void OnPromotionPiece(string PieceNewType)
    {
        string tmpStr = string.Format("{0}_{1}", "+Promotion", PieceNewType);
        this.turnManager.SendMove(tmpStr, true);
    }

    public string getMessageFormat(int movePieceType, int killPieceType, int originalX, int originalY, int x, int y)
    {
        string tmpStr = string.Format("{0}_{1}_{2}_{3}_{4}_{5}",
               movePieceType.ToString(), killPieceType.ToString(),
               originalX.ToString(), originalY.ToString(),
               x.ToString(), y.ToString());

        return tmpStr;
    }
    #endregion



}
