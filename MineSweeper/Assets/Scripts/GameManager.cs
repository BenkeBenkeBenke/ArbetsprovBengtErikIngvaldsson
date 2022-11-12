using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int width = 16;              //Board size
    public int height = 16;
    public int mineCount = 32;          //Tot antal mines i spelet.

    private Board board;                //Referenser används för att kunna uppdatera spelplanen
    private Cell[,] tileData;           //Array med all data för alla celler.

    private Cell cell;                  //Cell klickad av spelaren.(Kan inte använda "tile" då detta namn används av Unitys "tile" system.
    private Vector3Int cellPosition;    //Klickad cells position

    private bool lookGameInput;         //För att hindra spelaren från att trycka på fler tiles när en spelet är över.
    public GameObject winUI;            //vinn game UI.
    public GameObject loseUI;           //förlora game UI.

    //Calls när något ändras i inspektorn.
    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, (width * height)-1);      //För att inte det ska finnas fler minor än spelplaner. Måste iallafall finnas en tile som inte är en mina.
    }

    private void Awake()
    {
        board = GetComponentInChildren<Board>();                    //Board scriptet ligger i children.
    }

    private void Start()
    {
        NewGame();  
    }

    //Jag gjorde denna funktion för att i framtiden kunna starta om spelet från menyer mm...
    private void NewGame()
    {
        lookGameInput = false;                              //Lås up Input
        winUI.SetActive(false);                             //Göm UI.
        loseUI.SetActive(false);
        tileData = new Cell[width, height];                 //skapa tom cell data array

        CreateBoard();                                     //pass 1 skapa tomma celler.
        AddMines();                                        //pass 2 lägger till mines(skriver över/ändrar tomma rutor data)
        AddNumbers();                                      //pass 3 när vi vet vart minorna är så kan vi sätta ut siffror.

        Camera.main.transform.position = new Vector3(width / 2, height / 2, -10); //Centrera cameran över spelplanen.
        board.Draw(tileData);                              //Säg åt Board scriptet att skapa boarden.
    }

    private void CreateBoard()
    {
        for (int x = 0; x < width; x++)                    //Iterera över arrayen för att skapa tiles.
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();                    //Skapa nytt/tomt cell script för den här tilen. 
                cell.position = new Vector3Int(x, y, 0);   //Fyll på med data.
                cell.type = Cell.Type.Empty;               //Första pass för att skapa alla rutorna empty(sen kommer fler passes med mines osv...)
                tileData[x, y] = cell;                     //Överför den nyskapade cellen till tileData arrayen som har koll på alla tiles.
            }
        }
    }

    private void AddMines()
    {
        for (int i = 0; i < mineCount; i++)                 //Skapa mines
        {
            int x = Random.Range(0, width);                 //sätt ut random mine
            int y = Random.Range(0, height);

            while (tileData[x,y].type == Cell.Type.Mine)    //Om tilen renda är en mine så behöver vi assing en annan tile.
            {
                /////////////////////////////// Antingen kollar man upp en ny random ruta
                x = Random.Range(0, width);
                y = Random.Range(0, height);

                ///////////////////////////////Eller så kan man göra såhär:
                
                /*
                //Stegar till nästa ruta
                x++;

                //Ifall vi är utanför spelplanen
                if(x >= width)
                {
                    x = 0;
                    y++;
                    if(y >= height)
                    {
                        y = 0;
                    }
                }
                */

                /////////////////////////////TODO: sätta en gräns för hur många ggr den hör while loopen får köras för att förhindra crach.
            }

            tileData[x, y].type = Cell.Type.Mine;              //Set tile data
            //tileData[x, y].revealed = true;                  //Tillfälligt prova om dom placeras random
        }
    }

    private void AddNumbers()
    {
        for (int x = 0; x < width; x++)                     //Iterera över arrayen för att skapa siffror
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = tileData[x,y];                  //hitta tiledata för just den här tilen.
                if (cell.type == Cell.Type.Mine) continue;  //Inga tiles med mine har numer: hoppa över den här tilen.
                cell.number = CountMines(x,y);              //Samma här. Detta kräver mer logic så jag delar upp stora problem till små och löser.
           
                if(cell.number > 0)                         //Om det finns mines i närheten
                {
                    cell.type = Cell.Type.Number;
                }

                //cell.revealed = true;                      //Tillfällig test för att se hela spel planen.
                tileData[x, y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        int count = 0;                                              //Sätt till 0 till att börja med.       
        for(int offsetX = -1; offsetX <= 1; offsetX++)              //Kolla i kolumnen till höger om den här tilen. Sen samma column och sen vänster column.
        {
            for(int offsetY = -1; offsetY <= 1; offsetY++)          //Kolla i raden ovanför om den här tilen. Sen samma rad och sen nedanför.
            {
                if(offsetX == 0 && offsetY == 0) continue;          //Om båda offseten är 0 så är vi på samma ruta som vi står på och det är ingen mine.
                int x = cellX + offsetX;                            //Lägg ihop för att få fram vilken tile som ska kollas
                int y = cellY + offsetY;
                if (GetCell(x,y).type == Cell.Type.Mine) count++;   //Använd en hjälp funktion för att se om det är en mine så lägg till 1 på countern som sen visar hur många mines det finns i närheten
            }
        }
        return count;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) NewGame();                  //Tryck "Space" för att starta om.

        if (!lookGameInput)
        {
            if (Input.GetMouseButtonDown(1))                            //Plasera flaga
            {
                Flag();
            }
            if (Input.GetMouseButtonDown(0))                            //Klickat tile
            {
                Reveal();                                               //Gillar att hålla update kort och använda hjälpfunktioner. Då är det lättare att felsöka.
            }
        }
    }

    private void Reveal()
    {
        GetCellClicked();                                                //Hjälp funktion för att setta cell och cell position.
        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode();
                break;

            case Cell.Type.Empty:
                CheckNeighbors(cell);                                   //Väldigt klurig och rolig hjälp funktion. Troligt vis den svåraste och den ni vill att jag ska visa hur jag tänker.
                CheckWinCondition();
                break;

            default:
                cell.revealed = true;
                tileData[cellPosition.x, cellPosition.y] = cell;        //Uppdatera cell data arrayen.
                CheckWinCondition();
                break;

        }


        board.Draw(tileData);                                           //TileData arrayen har ändrats. Så då behövs boarden uppdateras.
    }
    private void Explode()
    {
        Debug.Log("Game over");
        lookGameInput = true;
        loseUI.SetActive(true);

        cell.revealed = true;
        cell.exploded = true;
        tileData[cell.position.x, cell.position.y] = cell;          //Save tileData.

        for (int x = 0; x < width; x++)                             //Checka igenom boarden för att hitta minor och reveal dem
        {
            for (int y = 0; y < height; y++)
            {
                cell = tileData[x, y];                              //Load den aktuella cellens state
                if(cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    tileData[x, y] = cell;                          //Save tielData
                }
            }
        }
    }

    
    private void CheckNeighbors(Cell cellToCheck)                                                   //Loop för att hitta cluster av celler som sitter ihop och inte är minor. Det är nog den här funktionen ni vill att jag skulle göra i arbetsprovet. Det är den klurigaste men också den roligaste att tänka ut hur den ska funka.
    {
        if (cellToCheck.revealed) return;                                                           //Om Checkad tile redan är revelad : stop loop. Detta för att förhindra tex att samma tile kollas flera ggr.
        if (cellToCheck.type == Cell.Type.Mine || cellToCheck.type == Cell.Type.Invalid) return;    //Samma med mine och utanför spelplanen
        
        cellToCheck.revealed = true;                                                                //Visa tilen som ska kollas
        tileData[cellToCheck.position.x, cellToCheck.position.y] = cellToCheck;                     //Spara tilen som ska kollas

                                                                                                    //Om det är en cell med siffra så ska den fortfarande revelas men loopen ska inte fortsätta. Bara starta om loopen om det är en tom ruta.
        if(cellToCheck.type == Cell.Type.Empty)                                                     //Starta om loopen för celler höger+vänster+up+ner från den nuvarande kollade cellen . Det gör inget om det är den sista cellen på raden för detta tas om hand av return chechen ovan. 
        {
            CheckNeighbors(GetCell(cellToCheck.position.x +1,cellToCheck.position.y));              //Höger
            CheckNeighbors(GetCell(cellToCheck.position.x -1, cellToCheck.position.y));             //Vänster
            CheckNeighbors(GetCell(cellToCheck.position.x, cellToCheck.position.y +1));             //Upp
            CheckNeighbors(GetCell(cellToCheck.position.x, cellToCheck.position.y -1));             //Ner OBS!!!Diagonalen kollas inte.
        }
    }

    private void Flag()                                                 //Logik för när spelaren flagar en tile
    {
        GetCellClicked();                                               //Återanvänd hjälp funktion för att sätta cell och cell position.
        if (cell.type == Cell.Type.Invalid || cell.revealed) return;    //Om spelaren klickar utanför spelplanen eller om tilen redan är reveald så ska inget hända
        cell.flagged = !cell.flagged;                                   //toggle flagged bool.

        tileData[cellPosition.x, cellPosition.y] = cell;                //Save tileData
        board.Draw(tileData);                                           //Uppdatera spelplanen
    }

    private void GetCellClicked()                                       //Användbar hjälp funktion
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cellPosition = board.tilemap.WorldToCell(worldPosition);
        cell = GetCell(cellPosition.x, cellPosition.y);
    }

    private Cell GetCell(int x, int y)                       //Hjälp funktion för att hitta cell från koordinater. Detta sker ofta så en hjälpfunktion gör koden snyggare.
    {
        if (IsValidCell(x, y))
        {
            return tileData[x, y];
        }
        else
        {
            //return null;                                  //Kan inte return null då detta är en struct.
            return new Cell();                              //defult är "invalid"
        }
    }

    private bool IsValidCell(int x, int y)                      //Samma här. Att kolla om koordinaterna är inom tilemapen är också nice att ha i en hjälpfunktion.
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = tileData[x, y];
                if (cell.type != Cell.Type.Mine && !cell.revealed)       //Kolla att alla tiles som inte är mine är synliga..
                    return;
            }
        }

        Debug.Log("You win");                                           //Spelaren har gjort alla tiles förutom minor synliga och vunnit.
        lookGameInput = true;                                           //Låser spelplanen och visar "Start new game" ui.
        winUI.SetActive(true);

        for (int x = 0; x < width; x++)                                 //Visa spelaren vart minorna är.
        {
            for (int y = 0; y < height; y++)
            {
                cell = tileData[x, y];                                  //Load den aktuella cellens tileData
                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    tileData[x, y] = cell;                              //Save tileData
                }
            }
        }
    }
}
