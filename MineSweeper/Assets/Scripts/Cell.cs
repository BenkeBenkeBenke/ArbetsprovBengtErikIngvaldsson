using UnityEngine;
public struct Cell  //Custom data structure. Varje tile beh�vde detta script f�r att samla tile data.
{
    public enum Type //F�r att beskriva vilken "tileData" tilen befinner sig i.
    {
        Invalid,    //Defult
        Empty,
        Mine,
        Number,
    }

    public Vector3Int position;             //Beskriver positionen i tilemapen. Koordinater. Tablemap hanterar bara int och vecktor3Int.
    public Type type;
    public int number;                      //Beskriver hur m�nga mines runt den h�r tilen.
    public bool revealed;                   //Anv�nds tex vid test av f�r att se om spelaren klickat p� ett kluster av "toma" tiles.
    public bool flagged;                    //F�r att se om den h�r rutan ska l�mnas ifred vid cluster utr�kning.
    public bool exploded;                   //Om spelaren har kclickat p� en mina och f�rlorat.
}
