using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.brnn
{
    public class CommonObject
    {
        public static GameObject CurrentSelectChip;                             //玩家选择筹码
        public static List<GameObject> CurrentChipList = new List<GameObject>();             //所有筹码
        public static GameObject[] CardArray0 = new GameObject[5];
        public static GameObject[] CardArray1 = new GameObject[5];
        public static GameObject[] CardArray2 = new GameObject[5];
        public static GameObject[] CardArray3 = new GameObject[5];
        public static GameObject[] CardArray4 = new GameObject[5];
        public static List<GameObject> CurrentBankerList = new List<GameObject>();
        public static List<GameObject> CurrentPlayerList = new List<GameObject>();
        public static GameObject[] CurrentShowChip;


        public static void Gc()
        {
            CurrentSelectChip = null;
            CurrentChipList.Clear();
            CardArray0 = new GameObject[5];
            CardArray1 = new GameObject[5];
            CardArray2 = new GameObject[5];
            CardArray3 = new GameObject[5];
            CardArray4 = new GameObject[5];
            CurrentBankerList.Clear();
           CurrentPlayerList.Clear();
           CurrentShowChip = null;
        }
    }
}
