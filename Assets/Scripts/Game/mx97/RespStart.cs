using UnityEngine;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

/**
 * 用户点击开始之后 处理回包
 * 
 */
namespace Assets.Scripts.Game.mx97
{
    public class RespStart
    {
        public bool MIsAllBar = false;                                         // key = "allb"
        public List<int> MLineList = new List<int>();                          // key = "lines"    
        public int MSeven = 0;                                                 // key = "seven"
        public long MTotalGold = 0;                                            // key = "ttgold"
        public bool MIsAllFruit = false;                                       // key = "allf"
        public int MGotJackpotGlod = 1;                                        // key = "gold"         本局获得的分数
        public List<int> MFruitList = new List<int>();                         // key = "fruits"    
        public int MType = 1;           

        public void ParseData(ISFSObject sfsObject)
        {

            MLineList.Clear();
            MFruitList.Clear();

            MIsAllBar = sfsObject.GetBool("allb");

            string strLogLines = "";
            int[] lines = sfsObject.GetIntArray("lines");
            for (int i = 0; i < lines.Length; i++)
            {
                MLineList.Add(lines[i]);

                strLogLines = strLogLines + " " + lines[i]+"";
            }
            YxDebug.Log("  ----> RespStart: Lines info of start data is " + strLogLines + " ! \n");

            MSeven = sfsObject.GetInt("seven");

            MTotalGold = sfsObject.GetLong("ttgold");

            MIsAllFruit = sfsObject.GetBool("allf");

            //全水果、Bar
            BigWin.getInstance().isAllFruit = MIsAllFruit;
            BigWin.getInstance().isAllBar = MIsAllBar;

            MGotJackpotGlod = sfsObject.GetInt("gold");

            string strLogFruits = "";
            int[] fruit = sfsObject.GetIntArray("fruits");
            for (int i = 0; i < fruit.Length; i++)
            {
                MFruitList.Add(fruit[i]);

                strLogFruits = strLogFruits + " " + fruit[i]+"";
            }
            YxDebug.Log("  ----> RespStart: Fruit info of start data is " + strLogFruits + " ! \n");

            MType = sfsObject.GetInt("type");

        }
    }
}
