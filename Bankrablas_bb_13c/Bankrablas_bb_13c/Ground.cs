namespace Bankrablas_bb_13c
{
    /// <summary>
    /// Ez egy townelement ez az alap ugymond fold.
    /// </summary>
    public class Ground : TownElement, IHasPosition
    {
        (int, int) position;
        public (int, int) Position { get { return position; } set { position = value; } }
        public Ground((int,int) hely) { 
            this.position = hely;
        }

        public Ground() { 
        
        }
        public override string ToString() => ("  ").PadRight(3);
        
    }
}
