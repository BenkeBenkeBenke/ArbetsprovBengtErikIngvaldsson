using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int width = 16;              //Board size
    public int height = 16;
    public int mineCount = 32;          //Tot antal mines i spelet.

    private Board board;                //Referenser anv�nds f�r att kunna uppdatera spelplanen
    private Cell[,] tileData;           //Array med all data f�r alla celler.

    private Cell cell;                  //Cell klickad av spelaren.(Kan inte anv�nda "tile" d� detta namn anv�nds av Unitys "tile" system.
    private Vector3Int cellPosition;    //Klickad cells position

    private bool lookGameInput;         //F�r att hindra spelaren fr�n att trycka p� fler tiles n�r en spelet �r �ver.
    public GameObject winUI;            //vinn game UI.
    public GameObject loseUI;           //f�rlora game UI.

    //Calls n�r n�got �ndras i inspektorn.
    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, (width * height)-1);      //F�r att inte det ska finnas fler minor �n spelplaner. M�ste iallafall finnas en tile som inte �r en mina.
    }

    private void Awake()
    {
        board = GetComponentInChildren<Board>();                    //Board scriptet ligger i children.
    }

    private void Start()
    {
        NewGame();  
    }

    //Jag gjorde denna funktion f�r att i framtiden kunna starta om spelet fr�n menyer mm...
    private void NewGame()
    {
        lookGameInput = false;                              //L�s up Input
        winUI.SetActive(false);                             //G�m UI.
        loseUI.SetActive(false);
        tileData = new Cell[width, height];                 //skapa tom cell data array

        CreateBoard();                                     //pass 1 skapa tomma celler.
        AddMines();                                        //pass 2 l�gger till mines(skriver �ver/�ndrar tomma rutor data)
        AddNumbers();                                      //pass 3 n�r vi vet vart minorna �r s� kan vi s�tta ut siffror.

        Camera.main.transform.position = new Vector3(width / 2, height / 2, -10); //Centrera cameran �ver spelplanen.
        board.Draw(tileData);                              //S�g �t Board scriptet att skapa boarden.
    }

    private void CreateBoard()
    {
        for (int x = 0; x < width; x++)                    //Iterera �ver arrayen f�r att skapa tiles.
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();                    //Skapa nytt/tomt cell script f�r den h�r tilen. 
                cell.position = new Vector3Int(x, y, 0);   //Fyll p� med data.
                cell.type = Cell.Type.Empty;               //F�rsta pass f�r att skapa alla rutorna empty(sen kommer fler passes med mines osv...)
                tileData[x, y] = cell;                     //�verf�r den nyskapade cellen till tileData arrayen som har koll p� alla tiles.
            }
        }
    }

    private void AddMines()
    {
        for (int i = 0; i < mineCount; i++)                 //Skapa mines
        {
            int x = Random.Range(0, width);                 //s�tt ut random mine
            int y = Random.Range(0, height);

            while (tileData[x,y].type == Cell.Type.Mine)    //Om tilen renda �r en mine s� beh�ver vi assing en annan tile.
            {
                /////////////////////////////// Antingen kollar man upp en ny random ruta
                x = Random.Range(0, width);
                y = Random.Range(0, height);

                ///////////////////////////////Eller s� kan man g�ra s�h�r:
                
                /*
                //Stegar till n�sta ruta
                x++;

                //Ifall vi �r utanf�r spelplanen
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

                /////////////////////////////TODO: s�tta en gr�ns f�r hur m�nga ggr den h�r while loopen f�r k�ras f�r att f�rhindra crach.
            }

            tileData[x, y].type = Cell.Type.Mine;              //Set tile data
            //tileData[x, y].revealed = true;                  //Tillf�lligt prova om dom placeras random
        }
    }

    private void AddNumbers()
    {
        for (int x = 0; x < width; x++)                     //Iterera �ver arrayen f�r att skapa siffror
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = tileData[x,y];                  //hitta tiledata f�r just den h�r tilen.
                if (cell.type == Cell.Type.Mine) continue;  //Inga tiles med mine har numer: hoppa �ver den h�r tilen.
                cell.number = CountMines(x,y);              //Samma h�r. Detta kr�ver mer logic s� jag delar upp stora problem till sm� och l�ser.
           
                if(cell.number > 0)                         //Om det finns mines i n�rheten
                {
                    cell.type = Cell.Type.Number;
                }

                //cell.revealed = true;                      //Tillf�llig test f�r att se hela spel planen.
                tileData[x, y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        int count = 0;                                              //S�tt till 0 till att b�rja med.       
        for(int offsetX = -1; offsetX <= 1; offsetX++)              //Kolla i kolumnen till h�ger om den h�r tilen. Sen samma column och sen v�nster column.
        {
            for(int offsetY = -1; offsetY <= 1; offsetY++)          //Kolla i raden ovanf�r om den h�r tilen. Sen samma rad och sen nedanf�r.
            {
                if(offsetX == 0 && offsetY == 0) continue;          //Om b�da offseten �r 0 s� �r vi p� samma ruta som vi st�r p� och det �r ingen mine.
                int x = cellX + offsetX;                            //L�gg ihop f�r att f� fram vilken tile som ska kollas
                int y = cellY + offsetY;
                if (GetCell(x,y).type == Cell.Type.Mine) count++;   //Anv�nd en hj�lp funktion f�r att se om det �r en mine s� l�gg till 1 p� countern som sen visar hur m�nga mines det finns i n�rheten
            }
        }
        return count;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) NewGame();                  //Tryck "Space" f�r att starta om.

        if (!lookGameInput)
        {
            if (Input.GetMouseButtonDown(1))                            //Plasera flaga
            {
                Flag();
            }
            if (Input.GetMouseButtonDown(0))                            //Klickat tile
            {
                Reveal();                                               //Gillar att h�lla update kort och anv�nda hj�lpfunktioner. D� �r det l�ttare att fels�ka.
            }
        }
    }

    private void Reveal()
    {
        GetCellClicked();                                                //Hj�lp funktion f�r att setta cell och cell position.
        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode();
                break;

            case Cell.Type.Empty:
                CheckNeighbors(cell);                                   //V�ldigt klurig och rolig hj�lp funktion. Troligt vis den sv�raste och den ni vill att jag ska visa hur jag t�nker.
                CheckWinCondition();
                break;

            default:
                cell.revealed = true;
                tileData[cellPosition.x, cellPosition.y] = cell;        //Uppdatera cell data arrayen.
                CheckWinCondition();
                break;

        }


        board.Draw(tileData);                                           //TileData arrayen har �ndrats. S� d� beh�vs boarden uppdateras.
    }
    private void Explode()
    {
        Debug.Log("Game over");
        lookGameInput = true;
        loseUI.SetActive(true);

        cell.revealed = true;
        cell.exploded = true;
        tileData[cell.position.x, cell.position.y] = cell;          //Save tileData.

        for (int x = 0; x < width; x++)                             //Checka igenom boarden f�r att hitta minor och reveal dem
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

    
    private void CheckNeighbors(Cell cellToCheck)                                                   //Loop f�r att hitta cluster av celler som sitter ihop och inte �r minor. Det �r nog den h�r funktionen ni vill att jag skulle g�ra i arbetsprovet. Det �r den klurigaste men ocks� den roligaste att t�nka ut hur den ska funka.
    {
        if (cellToCheck.revealed) return;                                                           //Om Checkad tile redan �r revelad : stop loop. Detta f�r att f�rhindra tex att samma tile kollas flera ggr.
        if (cellToCheck.type == Cell.Type.Mine || cellToCheck.type == Cell.Type.Invalid) return;    //Samma med mine och utanf�r spelplanen
        
        cellToCheck.revealed = true;                                                                //Visa tilen som ska kollas
        tileData[cellToCheck.position.x, cellToCheck.position.y] = cellToCheck;                     //Spara tilen som ska kollas

                                                                                                    //Om det �r en cell med siffra s� ska den fortfarande revelas men loopen ska inte forts�tta. Bara starta om loopen om det �r en tom ruta.
        if(cellToCheck.type == Cell.Type.Empty)                                                     //Starta om loopen f�r celler h�ger+v�nster+up+ner fr�n den nuvarande kollade cellen . Det g�r inget om det �r den sista cellen p� raden f�r detta tas om hand av return chechen ovan. 
        {
            CheckNeighbors(GetCell(cellToCheck.position.x +1,cellToCheck.position.y));              //H�ger
            CheckNeighbors(GetCell(cellToCheck.position.x -1, cellToCheck.position.y));             //V�nster
            CheckNeighbors(GetCell(cellToCheck.position.x, cellToCheck.position.y +1));             //Upp
            CheckNeighbors(GetCell(cellToCheck.position.x, cellToCheck.position.y -1));             //Ner OBS!!!Diagonalen kollas inte.
        }
    }

    private void Flag()                                                 //Logik f�r n�r spelaren flagar en tile
    {
        GetCellClicked();                                               //�teranv�nd hj�lp funktion f�r att s�tta cell och cell position.
        if (cell.type == Cell.Type.Invalid || cell.revealed) return;    //Om spelaren klickar utanf�r spelplanen eller om tilen redan �r reveald s� ska inget h�nda
        cell.flagged = !cell.flagged;                                   //toggle flagged bool.

        tileData[cellPosition.x, cellPosition.y] = cell;                //Save tileData
        board.Draw(tileData);                                           //Uppdatera spelplanen
    }

    private void GetCellClicked()                                       //Anv�ndbar hj�lp funktion
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cellPosition = board.tilemap.WorldToCell(worldPosition);
        cell = GetCell(cellPosition.x, cellPosition.y);
    }

    private Cell GetCell(int x, int y)                       //Hj�lp funktion f�r att hitta cell fr�n koordinater. Detta sker ofta s� en hj�lpfunktion g�r koden snyggare.
    {
        if (IsValidCell(x, y))
        {
            return tileData[x, y];
        }
        else
        {
            //return null;                                  //Kan inte return null d� detta �r en struct.
            return new Cell();                              //defult �r "invalid"
        }
    }

    private bool IsValidCell(int x, int y)                      //Samma h�r. Att kolla om koordinaterna �r inom tilemapen �r ocks� nice att ha i en hj�lpfunktion.
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
                if (cell.type != Cell.Type.Mine && !cell.revealed)       //Kolla att alla tiles som inte �r mine �r synliga..
                    return;
            }
        }

        Debug.Log("You win");                                           //Spelaren har gjort alla tiles f�rutom minor synliga och vunnit.
        lookGameInput = true;                                           //L�ser spelplanen och visar "Start new game" ui.
        winUI.SetActive(true);

        for (int x = 0; x < width; x++)                                 //Visa spelaren vart minorna �r.
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
