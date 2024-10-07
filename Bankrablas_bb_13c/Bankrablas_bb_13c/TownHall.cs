
namespace Bankrablas_bb_13c
{
    /// <summary>
    /// A map egyik eleme. Pozicioja van csak. Ide kell eljutni a Sheriffnek a 5 goldja van.
    /// Ez is egy TownElement tehat rendelkezik mindennel ami ott elerheto bar nem hasznalja, viszont a
    /// tobbi elem szamara hasznos ez a kapcsolat.
    /// </summary>
    public class TownHall : TownElement, IHasPosition
    {
        (int, int) position;
        public (int, int) Position { get { return position; } set { position = value; } }

        public TownHall((int, int) position)
        {
            this.position = position;
        }

        public TownHall() { 
        
        }
        public override string ToString() => (" V ").PadRight(3);
    }
}
