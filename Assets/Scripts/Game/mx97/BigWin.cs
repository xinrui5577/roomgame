using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.mx97
{
    public class BigWin : MonoBehaviour
    {
        private static BigWin instance;
        public static BigWin getInstance()
        {
            return instance;
        }

        public bool isAllFruit;
        public bool isAllBar;

        public List<string> finalFruits = new List<string>();

        void Awake()
        {
            instance = this;
        }

        public bool IsBigWin()
        {
            if (isAllFruit || isAllBar)
            {
                return true;
            }
            else
            {
                var tmpName = finalFruits[0];

                int idx = 0;

                while (idx < finalFruits.Count)
                {
                    if (finalFruits[idx] != tmpName)
                        break;
                    ++idx;
                }

                if (idx >= finalFruits.Count)
                    return true;
                else
                    return false;
            }
        }
    }
}