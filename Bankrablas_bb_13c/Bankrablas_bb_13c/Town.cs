using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bankrablas_bb_13c
{
    public class Town : TownElement
    {
        //valtozok
        Sheriff sh = null;
        Stopper s = new Stopper();
        Dictionary<Bandit, long> banditaMoveTimes = new Dictionary<Bandit, long>();
        long lastSheriffMove = Environment.TickCount;
        static int banditaDelay = 400;
        int sheriffDelay = (int)(banditaDelay / 1.5);
        //-------
        public Town()
        {
            InitializePalya();
        }

        /// <summary>
        /// Szimulacio lebonyolitasa
        /// </summary>
        public void Simulation()
        {
            //palyagen
            GenerateValues();
            //tickout a banditakra movementhez
            FillBanditMovesDic();
            //Timer start
            s.Start();
            while (allas == Allapot.On)
            {
                Display();
                long currentTime = Environment.TickCount;
                for (int i = 0; i < MapSize; i++)
                {
                    for (int j = 0; j < MapSize; j++)
                    {
                        var element = map[i][j];
                        if (element is Bandit bandita)
                        {
                            if ((currentTime - banditaMoveTimes[bandita]) >= banditaDelay)
                            {
                                bandita.BanditMove();
                                banditaMoveTimes[bandita] = currentTime;
                            }
                        }
                        else if (element is Sheriff sheriff && (currentTime - lastSheriffMove) >= sheriffDelay)
                        {
                            sh = sheriff;
                            DisplayInfo();
                            lastSheriffMove = currentTime;
                        }
                    }
                }
                if (allas == Allapot.Win || allas == Allapot.Lose)
                {
                    s.Stop();
                    break;
                }
                Console.SetCursorPosition(0, 0);
            }
            CheckOutcome();
        }

        /// <summary>
        /// Nyert vagy vesztett
        /// </summary>
        void CheckOutcome() {
            if (allas == Allapot.Win)
            {
                WinCondition();
            }
            else
            {
                LoseCondition();
            }
        }

        /// <summary>
        /// Filleli a banditaMoveTimes-t
        /// </summary>
        void FillBanditMovesDic() {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (map[i][j] is Bandit bandita)
                    {
                        banditaMoveTimes[bandita] = Environment.TickCount;
                    }
                }
            }
        }

        /// <summary>
        /// Ha nyer ez tortenik
        /// </summary>
        void WinCondition() {
            Console.Clear();
            Console.WriteLine("Nyertél");
            Console.WriteLine($"Szimuláció ideje:{s.GetElapsedTime()}\nLépések száma:{sh.stepCounter}");
            using (StreamWriter wr = new StreamWriter("data.txt", true))
            {
                wr.WriteLine($"{sh.HealthPower}, {sh.stepCounter}, {s.GetElapsedTime()}, w");
            }
            Console.WriteLine("mentve a data.txt");
        }

        /// <summary>
        /// Ha veszit ez tortenik
        /// </summary>
        void LoseCondition() {
            Console.Clear();
            Console.WriteLine("Vesztettél");
            using (StreamWriter wr = new StreamWriter("data.txt", true))
            {
                wr.WriteLine($"{sh.HealthPower}, {sh.stepCounter}, {s.GetElapsedTime()}, l");
            }

            Console.WriteLine("mentve a data.txt");
        }

        /// <summary>
        /// Sheriff infok es futasido
        /// </summary>
        void DisplayInfo() {
            sh.DataDisplay();
            sh.SheriffMozog();
            sh.stepCounter++;
            sh.DisplayStepCount();
            Console.WriteLine($"|Eltelt idő:{s.GetElapsedTime()}|");
        }

        /// <summary>
        /// Map megjelenitese
        /// </summary>
        public static void Display()
        {

            const int elementWidth = 3;

            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    var tempCord = (i, j);
                    var palyaElement = map[i][j];
                    Console.BackgroundColor = ConsoleColor.DarkBlue;

                    if (IsInVisionRange(i, j) || revArea.Contains((tempCord)))
                    {
                        ChooseConsoleColor(palyaElement);
                        Console.Write(palyaElement?.ToString() ?? " ".PadRight(elementWidth));
                        if (!revArea.Contains(tempCord))
                        {
                            revArea.Add(tempCord);
                        }
                    }
                    else
                    {
                        Console.Write(" ".PadRight(elementWidth));
                    }
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Elemnek megfeleloen szinez
        /// </summary>
        /// <param name="elem">elem amit szinezni kell</param>
        static void ChooseConsoleColor(TownElement elem)
        {
            if (elem is Ground)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
            }
            else if (elem is Barricade)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
            }
            else if (elem is Gold)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
            }
            else if (elem is Whiskey)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
            }
            else if (elem is Bandit)
            {
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
            }
            else if (elem is TownHall)
            {
                Console.BackgroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
            }
        }

        /// <summary>
        /// 3*3-ban van-e a sheriffnek a kapott mezo
        /// </summary>
        /// <param name="x">kapott mezo x koordinataja</param>
        /// <param name="y">kapott mezo y koordinataja</param>
        /// <returns>igen/nem</returns>
        static bool IsInVisionRange(int x, int y)
        {
            var sheriff = map.SelectMany(row => row).FirstOrDefault(elem => elem is Sheriff);
            if (sheriff == null) return false;
            int centerX = ((Sheriff)sheriff).Position.Item1;
            int centerY = ((Sheriff)sheriff).Position.Item2;
            int startX = Math.Max(centerX - 1, 0);
            int endX = Math.Min(centerX + 1, MapSize - 1);
            int startY = Math.Max(centerY - 1, 0);
            int endY = Math.Min(centerY + 1, MapSize - 1);
            return x >= startX && x <= endX && y >= startY && y <= endY;
        }

        /// <summary>
        /// Elemek resetelese, szimulacio setupja
        /// </summary>
        void InitializePalya()
        {
            allas = Allapot.On;
            revArea = new List<(int, int)>();
            map = new List<List<TownElement>>(MapSize);
            for (int i = 0; i < MapSize; i++)
            {
                var row = new List<TownElement>(new TownElement[MapSize]);
                map.Add(row);
            }
        }

        /// <summary>
        /// A map valuejainak kigeneralasa
        /// </summary>
        void GenerateValues()
        {
            var avPlace = FillList();
            for (int i = 0; i < 4; ++i)
            {
                PlaceBandita(avPlace);
            }
            MultipleItemPlace(avPlace, 100, new Barricade());
            MultipleItemPlace(avPlace, 5, new Gold());
            MultipleItemPlace(avPlace, 3, new Whiskey());
            ItemPlace(avPlace, new Sheriff());
            ItemPlace(avPlace, new TownHall());
            FillNullsWithGround();
        }

        /// <summary>
        /// Tobb elem lerakasa
        /// </summary>
        /// <param name="elerhetoPozik">Elerheto poziciok listaja</param>
        /// <param name="count">db szam az elembol</param>
        /// <param name="elementType">lerakni kivant mezo typeja</param>
        void MultipleItemPlace(List<(int, int)> elerhetoPozik, int count, TownElement elementType)
        {
            for (int i = 0; i < count; i++)
            {
                ItemPlace(elerhetoPozik, elementType);
            }
        }

        void ItemPlace(List<(int, int)> elerhetoPozik, TownElement elementType)
        {
            int index = rand.Next(elerhetoPozik.Count);
            (int x, int y) = elerhetoPozik[index];
            elerhetoPozik.RemoveAt(index);
            map[x][y] = (TownElement)Activator.CreateInstance(elementType.GetType(), (x, y));
        }

        /// <summary>
        /// Feltolt egy listat az osszes mezo koordinatajaval ((0,0)tol (24,24)ig)
        /// </summary>
        /// <returns>lista((0,0)tol (24,24)ig)</returns>
        List<(int, int)> FillList()
        {
            var list = new List<(int, int)>();
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    list.Add((i, j));
                }
            }
            return list;
        }

        /// <summary>
        /// A mapon maradt nullokat feltolti groundal
        /// </summary>
        void FillNullsWithGround()
        {
            for (int i = 0; i < MapSize; i++)
            {
                for (int j = 0; j < MapSize; j++)
                {
                    if (map[i][j] == null)
                    {
                        map[i][j] = new Ground((i, j));
                    }
                }
            }
        }

        /// <summary>
        /// Banditak lerakasa
        /// </summary>
        /// <param name="avPlace">elerheto helyek listaja</param>
        void PlaceBandita(List<(int, int)> avPlace)
        {
            int x, y;
            bool validE;
            do
            {
                int index = rand.Next(avPlace.Count);
                (x, y) = avPlace[index];
                validE = BanditaPlaceCheck(x, y);

                if (validE)
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            int checkX = x + i;
                            int checkY = y + j;

                            if (checkX >= 0 && checkX < MapSize && checkY >= 0 && checkY < MapSize)
                            {
                                avPlace.Remove((checkX, checkY));
                            }
                        }
                    }
                }

            } while (!validE);
            map[x][y] = new Bandit((x, y));
        }

        /// <summary>
        /// Megnezi hogy le tudja-e tenni a banditat az adott mezore
        /// </summary>
        /// <param name="x">mezo x koordinataja</param>
        /// <param name="y">mezo y koordinataja</param>
        /// <returns>leteheto/nem leteheto</returns>
        bool BanditaPlaceCheck(int x, int y)
        {
            if (map[x][y] != null) return false;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int checkX = x + i;
                    int checkY = y + j;

                    if (checkX >= 0 && checkX < MapSize && checkY >= 0 && checkY < MapSize)
                    {
                        if (map[checkX][checkY] is Bandit)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
