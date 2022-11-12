using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour  // För att skapa spelplanen.
{
    public Tilemap tilemap { get; private set; }    //Så att andra script kan läsa tilemap data men bara det här scriptet kan "set" tile map data.

    public Tile tileUnknown;                        //Tillgängliga sprites/tiles att updatera boarden med.
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
        tilemap = GetComponent<Tilemap>();  //Väldigt viktigt att detta script är på samma gameobject som tilemap.
    }

    public void Draw(Cell[,] tileData)      //Called från Gamemanager. kräver en tileData array som input.
    {
        int width = tileData.GetLength(0);                      //Storleken på "tileData Array"
        int height = tileData.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = tileData[x, y];                     //Extrakt cell data från den aktuella kordinaten.
                tilemap.SetTile(cell.position, GetTile(cell));  //Skapa tilen
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.revealed)                  //kolla cell bool för att se vilken tile som ska ritas.
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

    private Tile GetRevealdTile(Cell cell)  //Logik för att se vad som är under tilen.(Type type)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? tileExplode : tileMine; //mine eller exploded om spelaren har klickat på tile.
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(Cell cell) //Numret behöver lite mer logik. Jag gillar att dela upp scripten så här så det blir tydligare vad som händer.
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

