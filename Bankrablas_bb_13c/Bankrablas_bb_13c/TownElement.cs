using System;
using System.Collections.Generic;

namespace Bankrablas_bb_13c
{
    abstract public class TownElement
    {
        public static Random rand = new Random();
        //itt van tarolva a palya
        public static List<List<TownElement>> map = new List<List<TownElement>>();
        //revealelt teruletek koordinataji
        public static List<(int, int)> revArea = new List<(int, int)>();
        //banditak szama
        public static int BanditaCouner = 0;
        public static int MapSize = 25;
        public enum Allapot { On, Win, Lose }
        public static Allapot allas = Allapot.On;
    }
}
