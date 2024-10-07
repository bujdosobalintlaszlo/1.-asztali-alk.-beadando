namespace Bankrablas_bb_13c
{
    /// <summary>
    /// A whiskey egy TownElement ami Heal kepesseggel rendelkezik a sheriffre nezve.
    /// </summary>
    public class Whiskey : TownElement, IHasPosition
    {
        (int, int) position;
        public (int, int) Position { get { return position; } set { position = value; } }

        public Whiskey((int, int) positiom)
        {
            this.position = positiom;
        }

        public Whiskey()
        {
        }

        public override string ToString() => (" W ").PadRight(3);
        
        /// <summary>
        /// Whiskey random helyre valo ujrageneralasa
        /// </summary>
        /// <returns>A whiskey uj helye</returns>
        public (int, int) WhiskeyRegen()
        {
            int i = rand.Next(0, 25);
            int j = rand.Next(0, 25);
            do
            {
                i = rand.Next(0, 25);
                j = rand.Next(0, 25);
            } while (!(map[i][j] is Ground));
            return (i, j);
        }

        /// <summary>
        /// Sheriff healelese
        /// </summary>
        /// <param name="s">Sheriff</param>
        public void Heal(Sheriff s)
        {
            if (s.HealthPower + 50 <= 100)
            {
                s.HealthPower += 50;
            }
            else
            {
                s.HealthPower = 100;
            }
        }

    }
}
