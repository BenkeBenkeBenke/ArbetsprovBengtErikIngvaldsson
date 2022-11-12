using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour  // F�r att skapa spelplanen.
{
    public Tilemap tilemap { get; private set; }    //S� att andra script kan l�sa tilemap data men bara det h�r scriptet kan "set" tile map data.

    public Tile tileUnknown;                        //Tillg�ngliga sprites/tiles att updatera boarden med.
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExplode;
    public Tile tileFlag;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;

    private void Awake()
    {   
        tilemap = GetComponent<Tilemap>();  //V�ldigt viktigt att detta script �r p� samma gameobject som tilemap.
    }

    public void Draw(Cell[,] tileData)      //Called fr�n Gamemanager. kr�ver en tileData array som input.
    {
        int width = tileData.GetLength(0);                      //Storleken p� "tileData Array"
        int height = tileData.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = tileData[x, y];                     //Extrakt cell data fr�n den aktuella kordinaten.
                tilemap.SetTile(cell.position, GetTile(cell));  //Skapa tilen
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.revealed)                  //kolla cell bool f�r att se vilken tile som ska ritas.
        {
            return GetRevealdTile(cell);
        }
        else if (cell.flagged)
        {
            return tileFlag;
        }
        else
        {
            return tileUnknown;
        }
    }

    private Tile GetRevealdTile(Cell cell)  //Logik f�r att se vad som �r under tilen.(Type type)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? tileExplode : tileMine; //mine eller exploded om spelaren har klickat p� tile.
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(Cell cell) //Numret beh�ver lite mer logik. Jag gillar att dela upp scripten s� h�r s� det blir tydligare vad som h�nder.
    {
        switch (cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
    }

}

