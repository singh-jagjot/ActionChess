using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ChessAI;

public class Board : MonoBehaviour
{
    [Header("ChessBoard Objects")]
    public Canvas gameInfo;

    public Material whiteTileMaterial;
    public Material blackTileMaterial;
    public Material tileHoverMaterial;
    public Material tileLegalMaterial;
    public Material tileDangerMaterial;
    public Material pieceLight;
    public Material pieceDark;
    public Material royalPieceLight;
    public Material royalPieceDark;
    public GameObject chessBoard;
    public GameObject pawn;
    public GameObject knight;
    public GameObject bishop;
    public GameObject rook;
    public GameObject queen;
    public GameObject king;

    [Header("ChessBoard Properties")]
    public float rotaionSpeed;
    public static ChessBoard cb = new ChessBoard();
    public static ChessAi ai = new ChessAi();

    private NavMeshAgent blackPieceAgent;
    private NavMeshAgent whitePieceAgent;

    private Transform previousHoveredTile = null;
    private Dictionary<int, GameObject> tileDict = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> pieceDict = new Dictionary<int, GameObject>();
    private HashSet<int> legalTiles = new HashSet<int>();
    private int sourceTile = -1;
    private int destinationTile = -1;
    private Animator whiteAnimator;
    private Animator blackAnimator;
    public static bool isBlackTurn = false;
    // private bool whiteTake = false;
    // private bool blackTake = false;
    private GameObject blackKing;
    private GameObject whiteKing;
    private int cpuDifficulty = 1;



    void Start()
    {
        transform.eulerAngles = new Vector3(0,90f,0);
        var ob = GameObject.Find("GameInfo");
        if(ob != null){
            gameInfo = ob.GetComponent<Canvas>();
        }
        // foreach (var item in cb.GetCurrentBoardDict())
        // {

        // }
        // Time.timeScale = 0f;
        // cb.SetCurrentFen("rnb1kbnr/pp1p2pp/3N4/2pP1p2/2PpQ3/8/PP2PPPP/R1q1KBNR b - - 99 50");
        foreach (var item in chessBoard.GetComponentsInChildren<Transform>())
        {
            // Debug.Log("AAA:"+item.name);
            if (item.name.Contains("Tile"))
            {
                tileDict.Add(GetTileNumberFromTileTransform(item), item.gameObject);
                pieceDict.Add(GetTileNumberFromTileTransform(item), RenderChessPiece(item));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // SetTileColor();
        // MoveChessPiecePlayer();
        if(blackKing == null){
            var gi = this.gameInfo.GetComponent<GameInfo>();
            gi.updateMsg("White Wins!");
            gi.gameOver();
        }
        if(whiteKing == null){
            var gi = this.gameInfo.GetComponent<GameInfo>();
            gi.updateMsg("Black Wins!");
            gi.gameOver();
        }
    }

    void FixedUpdate()
    {
        // if (blackPieceAgent != null && whitePieceAgent != null)
        // {
        //     // Debug.Log("A S:"+whitePieceAgent.isStopped);
        //     if (whitePieceAgent.remainingDistance < 0.5f)
        //     {
        //         whitePieceAgent.transform.rotation = Quaternion.RotateTowards(whitePieceAgent.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotaionSpeed);
        //         blackPieceAgent.isStopped = false;
        //     }
        //     if (blackPieceAgent.remainingDistance < 0.5f)
        //     {
        //         blackPieceAgent.transform.rotation = Quaternion.RotateTowards(blackPieceAgent.transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * rotaionSpeed);
        //     }


        // }
        if (whitePieceAgent != null && whitePieceAgent.remainingDistance < 0.5f)
        {
            whitePieceAgent.transform.rotation = Quaternion.RotateTowards(whitePieceAgent.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotaionSpeed);
            whiteAnimator.SetBool("doWalk", false);
            whitePieceAgent = null;
            isBlackTurn = true;
            // if(whiteTake){
            //     whiteAnimator.SetBool("DoAttack", true);
            // }
            // if(blackPieceAgent != null){
            // blackPieceAgent.isStopped = false;
            // }
        }
        if (blackPieceAgent != null && blackPieceAgent.remainingDistance < 0.5f)
        {
            blackPieceAgent.transform.rotation = Quaternion.RotateTowards(blackPieceAgent.transform.rotation, Quaternion.Euler(0, 180, 0), Time.deltaTime * rotaionSpeed);
            blackAnimator.SetBool("doWalk", false);
            blackPieceAgent =null;
            isBlackTurn = false;
        }
        SetTileColor();
        MoveChessPiecePlayer();
    }
    public static int GetTileNumberFromTileTransform(Transform tile)
    {if(tile == null){
        Debug.Log("Is is null");
    }
        return int.Parse(tile.name.Substring(tile.name.Length - 2));
    }
    private GameObject RenderChessPiece(Transform tile)
    {
        int tileNumber = GetTileNumberFromTileTransform(tile);
        var rotation = Quaternion.identity;
        var pieceMaterial = pieceLight;
        var royalPieceMaterial = royalPieceLight;
        GameObject p = null;

        if (cb.IsBlackPiece(tileNumber))
        {
            rotation = Quaternion.Euler(0, 180, 0);
            pieceMaterial = pieceDark;
            royalPieceMaterial = royalPieceDark;
        }

        if (ChessPieces.IsPawn(cb.GetCurrentBoardDict()[tileNumber]))
        {
            p = Instantiate(pawn, tile.transform.GetComponent<Collider>().bounds.center, rotation);
            p.SetActive(true);
            p.GetComponentInChildren<Renderer>().material = pieceMaterial;
        }
        else if (ChessPieces.IsBishop(cb.GetCurrentBoardDict()[tileNumber]))
        {
            p = Instantiate(bishop, tile.transform.GetComponent<Collider>().bounds.center, rotation);
            p.SetActive(true);
            p.GetComponentInChildren<Renderer>().material = pieceMaterial;
        }
        else if (ChessPieces.IsKnight(cb.GetCurrentBoardDict()[tileNumber]))
        {
            p = Instantiate(knight, tile.transform.GetComponent<Collider>().bounds.center, rotation);
            p.SetActive(true);
            p.GetComponentInChildren<Renderer>().material = pieceMaterial;
        }
        else if (ChessPieces.IsRook(cb.GetCurrentBoardDict()[tileNumber]))
        {
            p = Instantiate(rook, tile.transform.GetComponent<Collider>().bounds.center, rotation);
            p.SetActive(true);
            p.GetComponentInChildren<Renderer>().material = pieceMaterial;
        }
        else if (ChessPieces.IsQueen(cb.GetCurrentBoardDict()[tileNumber]))
        {
            p = Instantiate(queen, tile.transform.GetComponent<Collider>().bounds.center, rotation);
            p.SetActive(true);

            foreach (var item in p.GetComponentsInChildren<Renderer>())
            {
                item.material = royalPieceMaterial;
            }

        }
        else if (ChessPieces.IsKing(cb.GetCurrentBoardDict()[tileNumber]))
        {
            p = Instantiate(king, tile.transform.GetComponent<Collider>().bounds.center, rotation);
            p.SetActive(true);
            foreach (var item in p.GetComponentsInChildren<Renderer>())
            {
                item.material = royalPieceMaterial;
            }

            if (cb.IsBlackPiece(tileNumber))
            {
                blackKing = p;
            }else
            {
                whiteKing = p;
            }
        }

        return p;
    }

    private void SetTileColor()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, 1 << 6))
        {
            // if (hit.transform.name.Contains("Tile"))
            // {
            int tileNumber = GetTileNumberFromTileTransform(hit.transform);
            if (previousHoveredTile != null && previousHoveredTile != hit.transform)
            {
                previousHoveredTile.GetComponent<Renderer>().material = previousHoveredTile.name.Contains("Black") ? blackTileMaterial : whiteTileMaterial;
            }

            if (!legalTiles.Contains(tileNumber))
            {
                hit.transform.GetComponent<Renderer>().material = tileHoverMaterial;
                previousHoveredTile = hit.transform;
            }

            if (Input.GetAxis("Fire1") > 0)
            {
                if (cb.IsEmptySquare(tileNumber) && !legalTiles.Contains(tileNumber))
                {
                    sourceTile = -1;
                    destinationTile = -1;
                    foreach (var tileNo in legalTiles)
                    {
                        tileDict[tileNo].GetComponent<Renderer>().material = tileDict[tileNo].name.Contains("Black") ? blackTileMaterial : whiteTileMaterial;
                    }
                    legalTiles.Clear();
                }
                else if (sourceTile > -1 && legalTiles.Contains(tileNumber))
                {
                    destinationTile = tileNumber;
                }
                else
                {
                    sourceTile = tileNumber;
                    destinationTile = -1;
                    foreach (var tileNo in legalTiles)
                    {
                        tileDict[tileNo].GetComponent<Renderer>().material = tileDict[tileNo].name.Contains("Black") ? blackTileMaterial : whiteTileMaterial;
                    }
                    legalTiles.Clear();
                    foreach (var tileNo in cb.GetLegalMoves(tileNumber))
                    {
                        legalTiles.Add(tileNo);
                        if (!cb.IsEmptySquare(tileNo))
                        {
                            tileDict[tileNo].GetComponent<Renderer>().material = tileDangerMaterial;
                        }
                        else
                        {
                            tileDict[tileNo].GetComponent<Renderer>().material = tileLegalMaterial;
                        }
                    }
                }
            }

            // }
        }
    }

    // private GameObject GetChessPieceOnTile(int tileNo)
    // {
    //     // RaycastHit hit;
    //     // GameObject gameObject = null;
    //     // if (Physics.Raycast(tileDict[tileNo].GetComponent<Collider>().bounds.center, Vector3.up, out hit, 5))
    //     // {
    //     //     // Debug.Log(tileNo + ":" + hit.transform.name + "\n");
    //     //     gameObject = hit.transform.gameObject;
    //     // }
    //     return tileDict[tileNo];
    // }

    private void MoveChessPiecePlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, 1 << 6))
        {
            // if(Input.GetAxis("Fire1") > 0){
            //     Debug.Log("ST: "+sourceTile+"  DT: "+destinationTile);
            // }
            if (!isBlackTurn && Input.GetAxis("Fire1") > 0 && sourceTile > -1 && destinationTile > -1)
            {
                var go = pieceDict[sourceTile];
                whitePieceAgent = go.GetComponent<NavMeshAgent>();
                whiteAnimator = go.GetComponent<Animator>();
                // if (cb.IsCapture(sourceTile, destinationTile))
                // {
                //     whiteTake = true;
                // }

                whitePieceAgent.acceleration = 1f;
                whitePieceAgent.destination = tileDict[destinationTile].GetComponent<Collider>().bounds.center;
                whiteAnimator.SetBool("doWalk", true);
                cb.PlayerMove(sourceTile, destinationTile);
                UpdatePieceDict(sourceTile, destinationTile);
                // blackPieceAgent.isStopped = true;
                sourceTile = destinationTile = -1;
            }
            if(isBlackTurn){
                var bm = ai.GetBlackNextMove(cb, cpuDifficulty);
                var go = pieceDict[bm[0]];
                blackPieceAgent = go.GetComponent<NavMeshAgent>();
                blackAnimator = go.GetComponent<Animator>();

                // if (cb.IsCapture(bm[0], bm[1]))
                // {
                //     blackTake = true;
                //     // AttackPiece(pieceDict[bm[0]], pieceDict[bm[1]]);
                // }

                blackPieceAgent.acceleration = 1f;
                blackPieceAgent.destination = tileDict[bm[1]].GetComponent<Collider>().bounds.center;
                blackAnimator.SetBool("doWalk", true);
                cb.PlayerMove(bm[0], bm[1]);
                UpdatePieceDict(bm[0], bm[1]);
                isBlackTurn = false;
            }
            
        }
    }

    private void AttackPiece(GameObject attacker, GameObject attacked)
    {

        // attacked.SetActive(false);
    }

    private void UpdatePieceDict(int sourceTile, int destinationTile)
    {
        pieceDict[destinationTile] = pieceDict[sourceTile];
        pieceDict[sourceTile] = null;
    }

    public ChessBoard GetChessBoard(){
        return cb;
    }
    public Dictionary<int, GameObject> GetTileDict(){
        return tileDict;
    }
    public Dictionary<int, GameObject> GetPieceDict(){
        return pieceDict;
    }

    public void updateDifficulty(float val){
        this.cpuDifficulty = ((int)val);
        Debug.Log("Diffculty:" + cpuDifficulty);
    }
}
