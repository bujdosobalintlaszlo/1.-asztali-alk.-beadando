namespace Bankrablas_bb_13c
{
    /// <summary>
    /// Ez egy eleme a map-nak. Ebbol gyujt a Sheriff 5, ill. a Bandit is felszedheti. Fux mennyisegu van belole a mappon.
    /// Leszarmazottja a TownElementnek, ill. az IHasPositionnak is.
    /// </summary>
    public class Gold : TownElement, IHasPosition
    {
        (int, int) position;
        public (int, int) Position { get { return position; } set { position = value; } }

        public Gold((int, int) position)
        {
            this.position = position;
        }

        public Gold() { 
        
        }

        public override string ToString() => (" A ").PadRight(3);
        
    }
}
