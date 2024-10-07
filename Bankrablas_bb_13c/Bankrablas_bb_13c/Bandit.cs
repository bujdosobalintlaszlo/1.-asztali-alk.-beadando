
using System;
using System.Collections.Generic;

    namespace Bankrablas_bb_13c
    {
    /// <summary>
    /// A banita osztalya. Itt vannak megvalositva a banditahoz kapcsolodo eljarasok,fuggvnyek.
    /// Ezek:Mozgas,harcolas a sheriffel, megjelenites, kepessegek, halal lekezelese es arany atadasa.
    /// Ez az osztaly leszarmazottja a TownElementnek ezaltal eleri a map-et ami alapjan tajekozodik, ill. harcol.
    /// Az IHasPositon biztositja azt hogy legyen pozicioja.
    /// </summary>
        public class Bandit : TownElement,IHasPosition
    {

            //valtozok
            int healthPower = 100;
            public int HealthPower { get { return healthPower; } set { healthPower = value; } }
            (int, int) position;
            public (int, int) Position { get { return position; } set { position = value; } }
            int damage { get { return rand.Next(4, 16); } }
            int goldCounter = 0;
            List<(int,int)> directions = new List<(int, int)>() { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
            //-------------

            public Bandit((int, int) position)
            {
                this.position = position;
            }
            
            /// <summary>
            /// ToString override displayhez
            /// </summary>
            /// <returns></returns>
            public override string ToString() => (" B ").PadRight(3);
            
            
            /// <summary>
            /// Bandita mozgasa ha van a kozelben bandita akkor harcol ha nincs random lep
            /// </summary>
            public void BanditMove()
            {
                if (IsSheriffNearby())
                {
                    AttackSheriff();
                    return; 
                }
                var validMoves = GetValidMoves();
                if (validMoves.Count > 0)
                {
                    var (dx, dy) = validMoves[rand.Next(validMoves.Count)];
                    Step(dx, dy);
                }
            }
            
            /// <summary>
            /// Megmondja, hogy a bandita altal latott 3*3-as rangeban van-e sheriff
            /// </summary>
            /// <returns>Van/nincs</returns>
            bool IsSheriffNearby()
            {
                foreach (var (dx, dy) in directions)
                {
                    var newPosition = (position.Item1 + dx, position.Item2 + dy);
                    if (IsInBounds(newPosition) && IsSheriff(newPosition))
                    {
                        return true;
                    }
                }
                return false;
            }
            
            /// <summary>
            /// A harc lebonyolítása:
            /// Megkapjuk a sheriff poziciojat tovabb adjuk a mapban mar peldanyositott Sheriffet a Duelnak.
            /// </summary>
            void AttackSheriff()
            {
                foreach (var (dx, dy) in directions)
                {
                    var sheriffPosition = (position.Item1 + dx, position.Item2 + dy);
                    if (IsInBounds(sheriffPosition) && IsSheriff(sheriffPosition))
                    {
                        Sheriff sheriff = (Sheriff)map[sheriffPosition.Item1][sheriffPosition.Item2];
                        Duel(sheriff);
                        return;
                    }
                }
            }

        /// <summary>
        /// Osszeszedi a valid lepeseket ha van gold afele egyebkent random
        /// </summary>
        /// <returns>Valid lepesek koordinataji</returns>
        public List<(int, int)> GetValidMoves()
        {
            var validMoves = FindGoldMoves();
            if (validMoves.Count == 0)
            {
                validMoves = FindFreeMoves();
            }

            return validMoves;
        }

        /// <summary>
        /// Goldra lepes
        /// </summary>
        /// <returns>Goldralepes koordinataji</returns>
        private List<(int, int)> FindGoldMoves()
        {
            var goldMoves = new List<(int, int)>();

            foreach (var (dx, dy) in directions)
            {
                var newPosition = (position.Item1 + dx, position.Item2 + dy);
                if (IsValidGoldMove(newPosition))
                {
                    goldMoves.Add((dx, dy));
                }
            }

            return goldMoves;
        }

        /// <summary>
        /// Lehetseges lepesek listaja (validalt lepesek IsValidFreeMove-val)
        /// </summary>
        /// <returns>Lehetseges lepesek listaja</returns>
        private List<(int, int)> FindFreeMoves()
        {
            var freeMoves = new List<(int, int)>();

            foreach (var (dx, dy) in directions)
            {
                var newPosition = (position.Item1 + dx, position.Item2 + dy);
                if (IsValidFreeMove(newPosition))
                {
                    freeMoves.Add((dx, dy));
                }
            }

            return freeMoves;
        }
        /// <summary>
        /// Ellenoriz hogy az adott poziciora lephet-e a bandita
        /// </summary>
        /// <param name="position">egy pozicio a 3*3 rangebol</param>
        /// <returns>jo/nem jo</returns>
        private bool IsValidGoldMove((int, int) position) => IsInBounds(position) && IsGoldPosition(position);


        /// <summary>
        /// Ellenoriz hogy az adott poziciora lephet-e a bandita
        /// </summary>
        /// <param name="position">egy pozicio a 3*3 rangebol</param>
        /// <returns>jo/nem jo</returns>
        private bool IsValidFreeMove((int, int) position) => IsInBounds(position) && IsPositionFree(position);
        
        /// <summary>
        /// Megnezi hogy az adott mezo gold-e
        /// </summary>
        /// <param name="position">A bandita 3*3 rangebol mezo</param>
        /// <returns>igen/nem</returns>
        bool IsGoldPosition((int x, int y) position) => map[position.x][position.y] is Gold;

        /// <summary>
        /// Megnezi hogy az adott mezo sheriff-e
        /// </summary>
        /// <param name="position">A bandita 3*3 rangebol mezo</param>
        /// <returns>igen/nem</returns>
        bool IsSheriff((int x, int y) position)
        {
            TownElement newCell = map[position.x][position.y];
            return newCell is Sheriff;
        }

        /// <summary>
        /// Megomondja hogy az adott pozicio szabad-e (nincs rajta semmmi olyan objektum ami gatolna az odalepest)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        bool IsPositionFree((int x, int y) position)
        {
            TownElement newCell = map[position.x][position.y];
            return !(newCell is Bandit || newCell is Sheriff || newCell is Barricade || newCell is Whiskey || newCell is TownHall);
        }

        /// <summary>
        /// Egy lepes lebonyolitasa. Ha hataron kivuli vagy tiltott mezon van a koordinata nem lep ra. Ha jo akkor mozog.
        /// </summary>
        /// <param name="deltaX">x koordinata</param>
        /// <param name="deltaY">y koordinata</param>
        void Step(int deltaX, int deltaY)
        {
            var newPosition = (position.Item1 + deltaX, position.Item2 + deltaY);

            if (!IsInBounds(newPosition))
            {
                return;
            }

            if (!IsPositionFree(newPosition))
            {
                return;
            }

            Move(newPosition);
        }

        /// <summary>
        /// Checkeli hogy hataron belul van-e a koordinata
        /// </summary>
        /// <param name="position">A mezo amire lepne</param>
        /// <returns>belul van/kivul van</returns>
        bool IsInBounds((int x, int y) position) => position.x >= 0 && position.x < map.Count && position.y >= 0 && position.y < map[0].Count;
        
        /// <summary>
        /// Lepes csereli a lepet mezot onmagara, az elozot pedig groundra. Ha goldra lep akkor felszedi az aranyat es eltarolja
        /// a goldCounterbe.
        /// </summary>
        /// <param name="newPosition">A pozicio amire lep</param>
        void Move((int x, int y) newPosition)
        {
            if (map[newPosition.x][newPosition.y] is Gold) 
            {
                goldCounter++;
            }
            map[position.Item1][position.Item2] = new Ground(position);
            map[newPosition.x][newPosition.y] = this;
            position = newPosition;
        }

        /// <summary>
        /// Maga a harc. Addig megy amig meg nem hal vagy a sheriff el nem menekul.
        /// </summary>
        /// <param name="s">A sheriff akivel harcol</param>
        void Duel(Sheriff s)
        {
        do
        {
            s.HealthPower -= damage;
            } while (s.HealthPower > 0 && !IsSheriffNearby());
            IsAlive(s);
            return;
        }

        /// <summary>
        /// Ha meghal a bandita a sheriff megszerzi az aranyat.
        /// </summary>
        /// <param name="s">A sheriff akivel harcol</param>
        void IsAlive(Sheriff s) 
        {
            if (healthPower <= 0)
            {
                map[position.Item1][position.Item2] = new Ground((position.Item1, position.Item2));
                s.goldCounter += goldCounter;
            }
        }
        }
    }
