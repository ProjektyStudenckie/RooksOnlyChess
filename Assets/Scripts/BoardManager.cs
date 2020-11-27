using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public bool rooksOnlyGame = true;

    public AudioSource moveSound, destroySound, promotionSound;

    public GameObject winPanel;
    public Text winText;

    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { set; get; }
    public Chessman[,] Chessmans { set; get; }
    private Chessman selectedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;  //offset to the center (1/2)

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessPiecesPrefabs;
    private List<GameObject> activeChessPieces;

    private Material previousMat;
    public Material selectedMat;

    public int[] EnPassantMove { set; get; }

    public bool isWhiteTurn = true;

    private void Start()
    {
        Instance = this;

        winPanel.SetActive(false);

        moveSound = GetComponent<AudioSource>();
        destroySound = GetComponent<AudioSource>();
        promotionSound = GetComponent<AudioSource>();

        SpawnAllChessPieces();
    }

    private void Update()
    {
        UpdateSelection();
        DrawChessBoard();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedChessman == null)
                {
                    // select the chessman
                    SelectChessman(selectionX, selectionY);
                }
                else
                {
                    // move the chessman
                    MoveChessman(selectionX, selectionY);
                    CheckIfEnd();
                }
            }
        }
    }

    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null)
            return;

        if (Chessmans[x, y].isWhite != isWhiteTurn)
            return;

        //make sure to not double click if has no moves
        bool hasAtLeastOneMove = false;
        allowedMoves = Chessmans[x, y].PossibleMove();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (allowedMoves[i, j])
                    hasAtLeastOneMove = true;

        if (!hasAtLeastOneMove)
            return;

        selectedChessman = Chessmans[x, y];
        previousMat = selectedChessman.GetComponent<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedChessman.GetComponent<MeshRenderer>().material = selectedMat;
        BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);
    }

    private void MoveChessman(int x, int y)
    {
        if (allowedMoves[x, y])
        {

            Chessman c = Chessmans[x, y];

            if (c != null && c.isWhite != isWhiteTurn)
            {
                //Capture a piece
                activeChessPieces.Remove(c.gameObject);
                Destroy(c.gameObject);
                destroySound.Play();
            }

            // enpassant move
            if (x == EnPassantMove[0] && y == EnPassantMove[1])
            {
                if (isWhiteTurn)
                    c = Chessmans[x, y - 1];
                else
                    c = Chessmans[x, y + 1];

                activeChessPieces.Remove(c.gameObject);
                Destroy(c.gameObject);
                destroySound.Play();
            }
            EnPassantMove[0] = -1;
            EnPassantMove[1] = -1;
            if (selectedChessman.GetType() == typeof(Pawn))
            {
                // promote for white rook
                if (y == 7)
                {
                    activeChessPieces.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessPieces(0, x, y);
                    selectedChessman = Chessmans[x, y];
                    promotionSound.Play();
                }
                // promote for black rook
                else if (y == 0)
                {
                    activeChessPieces.Remove(selectedChessman.gameObject);
                    Destroy(selectedChessman.gameObject);
                    SpawnChessPieces(2, x, y);
                    selectedChessman = Chessmans[x, y];
                    promotionSound.Play();
                }

                if (selectedChessman.CurrentY == 1 && y == 3)
                {
                    EnPassantMove[0] = x;
                    EnPassantMove[1] = y - 1;
                }
                else if (selectedChessman.CurrentY == 6 && y == 4)
                {
                    EnPassantMove[0] = x;
                    EnPassantMove[1] = y + 1;
                }
            }

            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            selectedChessman.transform.position = GetTileCenter(x, y);
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman;
            isWhiteTurn = !isWhiteTurn;
            moveSound.Play();
        }

        selectedChessman.GetComponent<MeshRenderer>().material = previousMat;
        BoardHighlights.Instance.HideHighlights();
        selectedChessman = null;
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
            return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void SpawnChessPieces(int index, int x, int y)
    {
        GameObject go = Instantiate(chessPiecesPrefabs[index], GetTileCenter(x, y), Quaternion.Euler(-90, 0, 0));
        go.transform.SetParent(transform);
        Chessmans[x, y] = go.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);
        activeChessPieces.Add(go);
    }

    private void SpawnAllChessPieces()
    {
        activeChessPieces = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        EnPassantMove = new int[2] { -1, -1};

        //Spawn the chessman!

        if(rooksOnlyGame)
        {
            for (int i = 0; i < 8; i++) { SpawnChessPieces(0, i, 0); }
            for (int i = 0; i < 8; i++) { SpawnChessPieces(0, i, 1); }

            for (int i = 0; i < 8; i++) { SpawnChessPieces(2, i, 7); }
            for (int i = 0; i < 8; i++) { SpawnChessPieces(2, i, 6); }
            return;
        }

        // White Rooks
        for (int i = 0; i < 8; i++) { SpawnChessPieces(0, i, 0); }

        // White Pawns
        for (int i = 0; i < 8; i++) { SpawnChessPieces(1, i, 1); }

        // Black Rooks
        for (int i = 0; i < 8; i++) { SpawnChessPieces(2, i, 7); }

        // Black Pawns
        for (int i = 0; i < 8; i++) { SpawnChessPieces(3, i, 6); }

    }

    // find position on a board
    private Vector3 GetTileCenter(int x, int y) 
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE + x) + TILE_OFFSET - 1;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        origin.y += 0.5f;
        return origin;
    }

    private void DrawChessBoard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;
        
        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine( start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        // Draw selection
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));

            Debug.DrawLine(
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    private void CheckIfEnd()
    {
        if(activeChessPieces.Count > 0)
        {
            if (activeChessPieces[0].name.ToUpper().StartsWith("WHITE"))
            {
                for (int i = 0; i < activeChessPieces.Count; i++)
                    if (activeChessPieces[i].name.ToUpper().StartsWith("BLACK"))
                        return;
                EndGame(true);
            }
            else
            {
                for (int i = 0; i < activeChessPieces.Count; i++)
                    if (activeChessPieces[i].name.ToUpper().StartsWith("WHITE"))
                        return;
                EndGame(false);
            }
        }
    }

    private void EndGame(bool whiteWins)
    {
        winPanel.SetActive(true);

        if (whiteWins)
            winText.text = "WHITE WINS!";
        else
            winText.text = "BLACK WINS!";

        foreach (GameObject go in activeChessPieces)
            Destroy(go);

        isWhiteTurn = true;
        BoardHighlights.Instance.HideHighlights();
        SpawnAllChessPieces();
    }
}
