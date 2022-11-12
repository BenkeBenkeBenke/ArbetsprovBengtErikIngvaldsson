using UnityEngine;
public struct Cell  //Custom data structure. Varje tile behövde detta script för att samla tile data.
{
    public enum Type //För att beskriva vilken "tileData" tilen befinner sig i.
    {
        Invalid,    //Defult
        Empty,
        Mine,
        Number,
    }

    public Vector3Int position;             //Beskriver positionen i tilemapen. Koordinater. Tablemap hanterar bara int och vecktor3Int.
    public Type type;
    public int number;                      //Beskriver hur många mines runt den här tilen.
    public bool revealed;                   //Används tex vid test av för att se om spelaren klickat på ett kluster av "toma" tiles.
    public bool flagged;                    //För att se om den här rutan ska lämnas ifred vid cluster uträkning.
    public bool exploded;                   //Om spelaren har kclickat på en mina och förlorat.
}
