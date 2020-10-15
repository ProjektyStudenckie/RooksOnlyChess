using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;  //offset to the center (1/2)

    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessPiecesPrefabs;
    private List<GameObject> activeChessPieces;

    private void Start()
    {
        SpawnAllChessPieces();
    }

    private void Update()
    {
        UpdateSelection();
        DrawChessBoard();
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

    private void SpawnChessPieces(int index, Vector3 position)
    {
        GameObject go = Instantiate(chessPiecesPrefabs[index], position, Quaternion.Euler(-90, 0, 0)) as GameObject;
        go.transform.SetParent(transform);
        activeChessPieces.Add(go);
    }

    private void SpawnAllChessPieces()
    {
        activeChessPieces = new List<GameObject>();

        //Spawn the chessman!

        // White Rooks
        for (int i = 0; i < 8; i++) { SpawnChessPieces(0, GetTileCenter(i, 0)); }

        // White Pawns
        for (int i = 0; i < 8; i++) { SpawnChessPieces(1, GetTileCenter(i, 1)); }

        // Black Rooks
        for (int i = 0; i < 8; i++) { SpawnChessPieces(2, GetTileCenter(i, 7)); }

        // Black Pawns
        for (int i = 0; i < 8; i++) { SpawnChessPieces(3, GetTileCenter(i, 6)); }

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
}
