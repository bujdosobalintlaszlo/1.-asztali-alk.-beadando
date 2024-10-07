namespace Bankrablas_bb_13c
{
    /// <summary>
    /// Barikad aminek pozicioja van csak es egy tostring overrideja a displayeleshez
    /// </summary>
    public class Barricade : TownElement,IHasPosition
    {
        (int, int) position;
        public (int,int) Position { get { return position; } set { position = value; } }

        public Barricade((int, int) position)
        {
            this.position = position;
        }

        public Barricade() { 
        }

        public override string ToString() => (" X ").PadRight(3);
    }
}
