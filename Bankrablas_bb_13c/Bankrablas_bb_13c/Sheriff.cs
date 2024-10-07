using System;
using System.Collections.Generic;
using System.Linq;

namespace Bankrablas_bb_13c
{
    public class Sheriff : TownElement, IHasPosition
    {
        //valtozok
        int healthPower = 100;
        public int HealthPower { get { return healthPower; } set { healthPower = value; } }
        static int damage = rand.Next(20, 35);
        public int Damage { get { return damage; } }
        (int, int) position;
        public (int, int) Position { get { return position; } set { position = value; } }
        public int goldCounter = 0;
        List<(int, int)> steps = new List<(int, int)>();
        int innerStepCounter = 0;
        public Bandit targettedBandit = null;
        public int stepCounter = 0;
        public bool isFighting = false;
        //---------------
        public Sheriff((int, int) position)
        {
            this.position = position;
        }

        public Sheriff()
        {
        }

        public override string ToString() => (" S ").PadRight(3);
        
        /// <summary>
        /// Sheriff mozgas logikaja
        /// </summary>
        public void SheriffMozog()
        {
            if (steps.Count == 0)
            {
                if (!PalyaContainsVarosHaza() && goldCounter == 5)
                {
                    allas = Allapot.Win;
                }
                else if (healthPower <= 0) {
                    allas = Allapot.Lose;
                }
                else if (healthPower < 60 && healthPower > 0)
                {
                    HandleLowHealthActions();
                } 
                else if (healthPower >= 60) {
                    HandleGeneralActions();
                }
            }
            else {
                AreStepsDone();
            }
        }

        /// <summary>
        /// Ha hp-ja 60 alatti ez alapjan cselekszik
        /// </summary>
        void HandleLowHealthActions() {
            if (ContainsWhiskey())
            {
                steps = FindWhiskeyPath();
                Step(steps[innerStepCounter]);
                innerStepCounter++;
            }
            else if (ContainsVarosHaza() && goldCounter == 5)
            {
                steps = FindTownHallPath();
                Step(steps[innerStepCounter]);
                innerStepCounter++;
            }
            else
            {
                MoveTowardsMostlyUnrevealed();
            }
        }

        /// <summary>
        /// Ha a hp 60 felett van ez alapjan lep
        /// </summary>
        void HandleGeneralActions() {
            if (HasSurroundingBandit())
            {
                CheckForDuel();
            }
            else if (ContainsArany() && goldCounter < 5)
            {
                steps = FindClosestAranyPath();
                Step(steps[innerStepCounter]);
                innerStepCounter++;
            }
            else if (ContainsVarosHaza() && goldCounter == 5)
            {
                steps = FindTownHallPath();
                Step(steps[innerStepCounter]);
                innerStepCounter++;
            }
            else if (!ContainsArany() && RevTeruletIsFull() && ContainsBandit())
            {
                StepTowardsBandit();
            }
            else
            {
                MoveTowardsMostlyUnrevealed();
            }
        }

        /// <summary>
        /// ha nincs targetelt bandita akkor megkeresi a legkozelebbihez az utvonalat, ha van targetelt akkor lepes fele/ellenorzes h elkapta-e mar
        /// </summary>
        void StepTowardsBandit() {
            if (targettedBandit == null)
            {
                (int, int)? banditPos = FindClosestBandit();
                if (banditPos != null)
                {
                    targettedBandit = ((Bandit)map[banditPos.Value.Item1][banditPos.Value.Item2]);
                    steps = FindClosestBanditPath();
                    Step(steps[innerStepCounter]);
                    innerStepCounter++;
                }
            }
            else
            {
                IsBanditCaughth();
            }
        }

        /// <summary>
        /// Megnezi hogy elkapta-e a banditat ha nem incrementeli az innerStepCounter-t ha igen reseteli a stepset,innerStepCounter es a targetelt banditet
        /// </summary>
        void IsBanditCaughth() {
            if (steps.Count < innerStepCounter)
            {
                Step(steps[innerStepCounter]);
                innerStepCounter++;
            }
            else
            {
                steps = new List<(int, int)>();
                innerStepCounter = 0;
                targettedBandit = null;
            }
        }

        /// <summary>
        /// Ha lelepte a lepeseket a steps-bol akkor reseteli a listat es innerStepCounter-t, ha nem akkor incrementeli az innerStepCounter-t
        /// </summary>
        void AreStepsDone() {
            if (steps.Count < innerStepCounter)
            {
                Step(steps[innerStepCounter]);
                innerStepCounter++;
            }
            else
            {
                steps = new List<(int, int)>();
                innerStepCounter = 0;
            }
        }

        /// <summary>
        /// A* kereses a banditara
        /// </summary>
        /// <returns>A lepesek koordinataji</returns>
        List<(int, int)> FindClosestBanditPath() {

            (int, int)? bandit = GetAValidPosNextToBandit();
            if (!bandit.HasValue)
            {
                return new List<(int, int)>();
            }
            return AStarPathfinding(bandit.Value);
        }

        /// <summary>
        /// Ez azert van mert banditra nem lehet lepni igy egy mellette levo valid mezot keres amit kovethet
        /// </summary>
        /// <returns></returns>
        (int, int)? GetAValidPosNextToBandit() {
            (int, int)? bandit = (targettedBandit.Position.Item1, targettedBandit.Position.Item2);
            int i = 0;
            while (IsValidPosition(bandit.Value.Item1, bandit.Value.Item2))
            {
                bandit = (bandit.Value.Item1 + steps[i].Item1, bandit.Value.Item2 + steps[i].Item2);
                i++;
            }
            return bandit.Value;
        }

        /// <summary>
        /// Megtalalja a pillanatnyi legkozelebbi banditat(akkor amikor targeteli onnantol azt koveti amig meg nem oli vagy el nem menkul)
        /// </summary>
        /// <returns>pozicio ha van</returns>
        (int, int)? FindClosestBandit()
        {
            (int, int)? closestArany = null;
            int closestDistance = int.MaxValue;
            foreach (var (x, y) in revArea)
            {
                if (IsValidPosition(x, y) && map[x][y] is Bandit)
                {
                    int dsitance = Heuristic(Position, (x, y));
                    if (dsitance < closestDistance)
                    {
                        closestDistance = dsitance;
                        closestArany = (x, y);
                    }
                }
            }
            return closestArany;
        }

        /// <summary>
        /// Megnezi hogy van-e revTeruletben bandita
        /// </summary>
        /// <returns>van/nincs</returns>
        bool ContainsBandit()
        {
            foreach (var (x, y) in revArea)
            {
                if (x >= 0 && x < map.Count && y >= 0 && y < map[x].Count)
                {
                    if (map[x][y] is Bandit)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Tele van-e a revTerulet
        /// </summary>
        /// <returns>true/false</returns>
        bool RevTeruletIsFull() => revArea.Count == 625;

        /// <summary>
        /// Megnezi hogy van-e a Seriff 3*3ban bandita
        /// </summary>
        /// <returns>true/false</returns>
        bool HasSurroundingBandit()
        {
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i == 0 && j == 0) continue;
                    int newRow = Position.Item1 + i;
                    int newCol = Position.Item2 + j;

                    if (newRow >= 0 && newRow < 25 && newCol >= 0 && newCol < 25)
                    {
                        if (map[newRow][newCol] is Bandit)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// van-e whiskey a revTreuletben
        /// </summary>
        /// <returns>true/false</returns>
        bool ContainsWhiskey()
        {
            foreach (var (x, y) in revArea)
            {
                if (x >= 0 && x < MapSize && y >= 0 && y < MapSize)
                {
                    if (map[x][y] is Whiskey)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// van-e arany a revTeruletben
        /// </summary>
        /// <returns>true/false</returns>
        bool ContainsArany()
        {
            foreach (var (x, y) in revArea)
            {
                if (x >= 0 && x < MapSize && y >= 0 && y < MapSize)
                {
                    if (map[x][y] is Gold)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Megtalalja a legkozelebbi aranyat
        /// </summary>
        /// <returns>az arany koordinataja ha van (int,int)</returns>
        (int, int)? FindClosestArany()
        {
            (int, int)? closestArany = null;
            int closestDistance = int.MaxValue;
            foreach (var (x, y) in revArea)
            {
                if (IsValidPosition(x, y) && map[x][y] is Gold)
                {
                    int dsitance = Heuristic(Position, (x, y));
                    if (dsitance < closestDistance)
                    {
                        closestDistance = dsitance;
                        closestArany = (x, y);
                    }
                }
            }
            return closestArany;
        }

        /// <summary>
        /// Megnezi hogy van-e varoshaza
        /// </summary>
        /// <returns>true/false</returns>
        bool ContainsVarosHaza()
        {
            foreach (var (x, y) in revArea)
            {
                if (x >= 0 && x < MapSize && y >= 0 && y < MapSize)
                {
                    if (map[x][y] is TownHall)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Megnezi hogy a palyan van-e varoshaza
        /// </summary>
        /// <returns>van/nincs</returns>
        bool PalyaContainsVarosHaza()
        {
            for (int i = 0; i < MapSize; ++i)
            {
                for (int j = 0; j < MapSize; ++j)
                {
                    if (map[i][j] is TownHall)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Visszaadja a legoptimalisabb utvonalat (A* search)
        /// </summary>
        /// <returns>List<(int,int utvonal)></returns>
        public List<(int, int)> FindTownHallPath()
        {
            (int, int)? thPos = FindVarosHaza();
            if (!thPos.HasValue)
            {
                return new List<(int, int)>();
            }
            var target = thPos.Value;
            List<(int, int)> l = AStarPathfinding(target);
            return l;
        }

        /// <summary>
        /// Megkeresi a varoshazat ha van
        /// </summary>
        /// <returns>varoshaza koordinata(int,int)</returns>
        (int, int)? FindVarosHaza()
        {
            (int, int)? whPos = null;
            foreach (var (x, y) in revArea)
            {
                if (IsValidPosition(x, y) && map[x][y] is TownHall)
                {
                    whPos = (x, y);

                }
            }
            return whPos;
        }
        /// <summary>
        /// Megtalalja a legjobb utat a legkozelebbi aranyhoz (A* search)
        /// </summary>
        /// <returns>utvonal a legkozelebbi aranyhoz (List<(int,int)>)</returns>
        public List<(int, int)> FindClosestAranyPath()
        {
            (int, int)? closestArany = FindClosestArany();
            if (!closestArany.HasValue)
            {
                return new List<(int, int)>();
            }
            var target = closestArany.Value;
            return AStarPathfinding(target);
        }
        /// <summary>
        /// Visszadja az ut koordinatajit a whiskeyhez
        /// </summary>
        /// <returns>List<(int,int)></returns>
        public List<(int, int)> FindWhiskeyPath()
        {
            (int, int)? closestWhiskey = FindClosestWhiskey();
            if (!closestWhiskey.HasValue)
            {
                return new List<(int, int)>();
            }
            var target = closestWhiskey.Value;
            return AStarPathfinding(target);
        }

        /// <summary>
        /// visszaadja a legkozelebbi whiskey koordinatajat ha van ilyen
        /// </summary>
        /// <returns>(int,int) koordinata</returns>
        private (int, int)? FindClosestWhiskey()
        {
            (int, int)? closestWhiskey = null;
            int closestDistance = int.MaxValue;
            foreach (var (x, y) in revArea)
            {
                if (IsValidPosition(x, y) && map[x][y] is Whiskey)
                {
                    int distance = Heuristic(Position, (x, y));
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestWhiskey = (x, y);
                    }
                }
            }

            return closestWhiskey;
        }

        /// <summary>
        /// hataron belul van-e a poz.
        /// </summary>
        /// <param name="x">x koordinata</param>
        /// <param name="y">y koordinata</param>
        /// <returns>true/false</returns>
        private bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < MapSize && y >= 0 && y < MapSize;
        }

        /// <summary>
        /// A* kereses
        /// </summary>
        /// <param name="targetPosition">cel</param>
        /// <returns>List<(int,int)> optimalis utvonal (meg sheriff poz benne van)</returns>
        private List<(int, int)> AStarPathfinding((int, int) targetPosition)
        {
            var start = Position;
            var openSet = new HashSet<PathNode>();
            var closedSet = new HashSet<PathNode>();
            var startNode = new PathNode(start, null, 0, Heuristic(start, targetPosition));

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(n => n.F).First();

                if (currentNode.Position == targetPosition)
                {
                    return ReconstructPath(currentNode);
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbor in GetValidNeighbors(currentNode.Position))
                {
                    if (closedSet.Any(n => n.Position == neighbor))
                    {
                        continue;
                    }

                    var gCost = currentNode.G + 1;
                    var neighborNode = new PathNode(neighbor, currentNode, gCost, Heuristic(neighbor, targetPosition));

                    if (openSet.Any(n => n.Position == neighbor && n.G <= gCost))
                    {
                        continue;
                    }

                    openSet.Add(neighborNode);
                }
            }

            return new List<(int, int)>();
        }

        /// <summary>
        /// manhattan tavolsag szamitas - A* kereseshez es legkozelebbi whiskeyhez,Aranyhoz
        /// </summary>
        /// <param name="position">kiindulas</param>
        /// <param name="target">celpont</param>
        /// <returns>tavolsag(cost)</returns>
        private int Heuristic((int, int) position, (int, int) target)
        {
            return Math.Abs(position.Item1 - target.Item1) + Math.Abs(position.Item2 - target.Item2);
        }

        /// <summary>
        /// korbe checkelei a 3*3 hogy hol van akadaly es jarhato-e
        /// </summary>
        /// <param name="position">elem pozicio</param>
        /// <returns>List<(int,int)> szomszedos elemek koordinataji</returns>
        private List<(int, int)> GetValidNeighbors((int, int) position)
        {
            var neighbors = new List<(int, int)>();
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i == 0 && j == 0) continue;

                    var newRow = position.Item1 + i;
                    var newCol = position.Item2 + j;

                    if (IsValidPosition(newRow, newCol) && IsWalkable(newRow, newCol))
                    {
                        neighbors.Add((newRow, newCol));
                    }
                }
            }
            return neighbors;
        }

        /// <summary>
        /// Forbidden elem-e (varoshaza csak addig amig goldcounter < 5)
        /// </summary>
        /// <param name="x">x koordinata</param>
        /// <param name="y">y koordinata</param>
        /// <returns>true/false</returns>
        private bool IsWalkable(int x, int y)
        {
            if (map[x][y] is Barricade || map[x][y] is Bandit)
            {
                return false;
            }
            if (map[x][y] is TownHall && goldCounter < 5)
            {
                return false;
            }

            return true;
        }


        ///<summary> 
        ///Újraépíti az utat a celcsomoponttol a kezdocsomopontig kiszedi a kezdo erteket(maga a sheriff).
        /// </summary>
        /// <param name="currentNode">tarolja a costokat es, adott elem helyet es parent nodeot</param>
        /// <returns>List<(int,int)> az utvonal amibol kivan szedve a sheriff koordinataja</returns>
        private List<(int, int)> ReconstructPath(PathNode currentNode)
        {
            var path = new List<(int, int)>();

            while (currentNode != null)
            {
                path.Add(currentNode.Position);
                currentNode = currentNode.Parent;
            }

            path.Reverse();

            if (path.Count > 0)
            {
                path.RemoveAt(0);
            }

            return path;
        }

        /// <summary>
        /// Megtallalja a sheriff 3*3ban a banditat
        /// </summary>
        /// <returns>A 3*3ban levo Bandita objektum</returns>
        Bandit GetBandit()
        {
            Bandit bandit = null;

            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i == 0 && j == 0) continue;

                    int newRow = Position.Item1 + i;
                    int newCol = Position.Item2 + j;

                    if (newRow >= 0 && newRow < 25 && newCol >= 0 && newCol < 25)
                    {
                        if (map[newRow][newCol] is Bandit foundBandit)
                        {
                            bandit = foundBandit;
                            break;
                        }
                    }
                }
                if (bandit != null)
                {
                    break;
                }
            }
            return bandit;
        }

        /// <summary>
        /// Hacolas
        /// </summary>
        /// <param name="b">A szomszédos Bandita</param>
        void Duel(Bandit b)
        {
            isFighting = true;
            while (b.HealthPower > 0)
            {
                if (healthPower < 70)
                {
                    isFighting = false;
                    break;
                }
                b.HealthPower -= damage;
            }
            isFighting = false;
            return;
        }

        /// <summary>
        /// Ha nincs fighteban es van korulotte(3*3ban) bandita akkor harcol vele
        /// </summary>
        void CheckForDuel()
        {
            if (!isFighting && HasSurroundingBandit())
            {
                Duel(GetBandit());
            }
        }

        /// <summary>
        /// Lepes lebonyolitasa
        /// </summary>
        /// <param name="cord">koordinata</param>
        void Step((int, int) cord)
        {
            var currentTile = map[cord.Item1][cord.Item2];

            if (currentTile is Whiskey whiskey)
            {
                whiskey.Heal(this);
                (int,int) newPos = whiskey.WhiskeyRegen();
                map[newPos.Item1][newPos.Item2] = new Whiskey(newPos);
            }
            else if (currentTile is Gold)
            {
                goldCounter++;
            }
            else if (currentTile is TownHall)
            {
                Console.Clear();
            }
            var temp = this;
            map[position.Item1][position.Item2] = new Ground((position.Item1,position.Item2));
            map[cord.Item1][cord.Item2] = temp;
            temp.Position = cord;
        }

        /// <summary>
        /// Megprobal a legnemfelfedezettebb terulet fele lepni, ha nem tud oda lepni akkor random helyre lep
        /// </summary>
        public void MoveTowardsMostlyUnrevealed()
        {
            var validNeighbors = GetValidNeighbors((position.Item1, position.Item2));

            if (validNeighbors.Count == 0)
            {
                return;
            }

            var bestNeighbor = validNeighbors.OrderByDescending(neighbor => CountUnrevealedAround(neighbor)).First();

            if (CountUnrevealedAround(bestNeighbor) == 0)
            {
                bool allBlocked = validNeighbors.All(neighbor => CountUnrevealedAround(neighbor) == 0);

                if (allBlocked)
                {
                    var randomMove = validNeighbors.Where(n => IsWalkable(n.Item1, n.Item2)).OrderBy(x => rand.Next()).First();
                    Step(randomMove);
                }
                else
                {
                    Step(bestNeighbor);
                }
            }
            else
            {
                Step(bestNeighbor);
            }
        }

        /// <summary>
        /// Megnezi hogy mennyi unrevealed mezo van a sheriff korul
        /// </summary>
        /// <param name="position">sheriff pozicioja</param>
        /// <returns>unrevealed mezok szama</returns>
        private int CountUnrevealedAround((int, int) position)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var newRow = position.Item1 + i;
                    var newCol = position.Item2 + j;
                    if (IsValidPosition(newRow, newCol) && !TownElement.revArea.Contains((newRow, newCol)))
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Sheriff adatainak kiirasa
        /// </summary>
        public void DataDisplay()
        {
            int maxBarSize = 10;
            Console.Write("|Életerő: |");
            HealthColoring();
            for (int i = 0; i < maxBarSize; i++)
            {
                if (i == 3)
                {
                    Console.Write(healthPower.ToString().PadLeft(3));
                    i += 2;
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.ResetColor();
            Console.Write($"||Arany: {goldCounter}db.|");
        }

        /// <summary>
        /// Lepesek szamanak kiirasa
        /// </summary>
        public void DisplayStepCount() {
            Console.Write($"|Lépések: {stepCounter}db.|");
        }

        /// <summary>
        /// Szinezes az eletero kiirasahoz
        /// </summary>
        void HealthColoring() {
            if (healthPower > 70)
            {
                Console.BackgroundColor = ConsoleColor.Green;
            }
            else if (healthPower > 50)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }
        }
    }

}