using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using System;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{

    public class UtilDef
    {
        public const int NullArrowIndex = -1;

        public const int NullSeat = -1; //空座位
        public const int NullMj = -1;   //空的麻将

        public const int DefInt = Int32.MaxValue;

        public const int ExpPlush = 1000;
        public const int SortTalkPlush = 2000;

        public const int HardMjCnt = 13;

        public const int GamePlayerCnt = 4;

        public const int QiangGangHuType = 1 << 30;
    }

    public class UtilData
    {
        //当前游戏麻将人数
        public static int CurrGamePalyerCnt = UtilDef.GamePlayerCnt;

        public static int PlayerSeat = UtilDef.DefInt;

        public static int BankerSeat = UtilDef.DefInt;

        public static EnRoomType RoomType;

        public static bool HandMjTouchEnable = true;

        public static bool UsingQueryHu = false;

        //金币赔率
        public static float ShowGoldRate = 1;
        //是否自动准备
        public static bool IsAutoPrepare = false;
    }

   public class UtilFunc
    {
        public static void OutPutList<T>(List<T> list, string add = "")
        {
            string output = "";
            foreach (T item in list)
            {
                output += " " + item;
            }
            YxDebug.Log(add + output);
        }

        public static void OutPutArray<T>(T[] list, string add = "")
        {
            string output = "";
            foreach (T item in list)
            {
                output += " " + item;
            }
            YxDebug.Log(add + output);
        }

        public static void OutPutArrayList<T>(List<T[]> list, string add = "")
        {
            int i = 0;
            foreach (T[] item in list)
            {
                OutPutArray<T>(item, add + "[" + i + "]");
                i++;
            }
        }

       /// <summary>
       /// 把所有孩子的layer都变成父亲的layer
       /// </summary>
       /// <param name="tf"></param>
       public static void ChangeLayer(Transform tf)
       {
           if (tf == null || tf.parent == null) return;

           ChangeLayer(tf, tf.parent.gameObject.layer);
       }

       /// <summary>
        /// 把所有孩子的layer都变成指定的层
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="layer"></param>
        public static void ChangeLayer(Transform tf, int layer)
        {
            tf.gameObject.layer = layer;
            if (tf.childCount > 0)
            {
                for (int i = tf.childCount - 1; i >= 0; i--)
                {
                    ChangeLayer(tf.GetChild(i), layer);
                }
            }
        }

       public static int GetNextChair(int curr, int totalCnt)
       {
           if (App.GameKey != "jpmj")
           {
               return GetNextChairNormal(curr, totalCnt);
           }
           return GetNextChairForJp(curr, totalCnt);
       }

       private static int GetNextChairNormal(int curr, int totalCnt)
        {
            if (totalCnt == 2)
            {
                curr = curr == 2 ? 1 : curr;
            }
            if (totalCnt == 3)
            {
                curr = curr == 3 ? 2 : curr;
            }

            int next = (curr + 1) % totalCnt;

            if (totalCnt == 2)
            {
                return next == 1 ? 2 : next;
            }
            if (totalCnt == 3)
            {
                return next == 2 ? 3 : next;
            }
            if (totalCnt == 4)
            {
                return next;
            }

            YxDebug.Log("查找 下个座位号 出错");
            return -1;
        }
        /// <summary>
        /// 单独为精品麻将做的GetNextChair
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="totalCnt"></param>
        /// <returns></returns>
        private static int GetNextChairForJp(int curr, int totalCnt)
       {
           if (totalCnt == 2)
           {
               curr = curr == 2 ? 1 : curr;
           }
           if (totalCnt == 3)
           {

               switch (UtilData.PlayerSeat)
               {
                   case 1:
                       curr = curr == 3 ? 2 : curr;
                       break;
                   case 2:
                       {
                           if (curr == 3) curr = 2;
                           else if (curr == 2) curr = 1;
                           break;
                       }


               }


           }

           int next = (curr + 1) % totalCnt;

           if (totalCnt == 2)
           {
               return next == 1 ? 2 : next;
           }
           if (totalCnt == 3)
           {

               switch (UtilData.PlayerSeat)
               {
                   case 1:
                       return next == 2 ? 3 : next;
                   case 2:
                       {
                           if (next == 1) return 2;
                           if (next == 2) return 3;
                           break;
                       }
               }

               return next;
           }
           if (totalCnt == 4)
           {
               return next;
           }

           YxDebug.Log("查找 下个座位号 出错");
           return -1;
       }


        public static int GetPerChair(int curr, int totalCnt)
        {
            if (App.GameKey != "jpmj")
            {
                return GetPerChairNormal(curr, totalCnt);
            }
            return GetPerChairForJp(curr, totalCnt);
        }

        private static int GetPerChairNormal(int curr, int totalCnt)
       {
           if (totalCnt == 2)
           {
               curr = curr == 2 ? 1 : curr;
           }
           if (totalCnt == 3)
           {
               curr = curr == 3 ? 2 : curr;
           }

           int next = (curr + totalCnt - 1) % totalCnt;

           if (totalCnt == 2)
           {
               return next == 1 ? 2 : next;
           }
           if (totalCnt == 3)
           {
               return next == 2 ? 3 : next;
           }
           if (totalCnt == 4)
           {
               return next;
           }

           YxDebug.Log("查找 上个座位号 出错");
           return -1;
       }

        private static int GetPerChairForJp(int curr, int totalCnt)
       {
           if (totalCnt == 2)
           {
               curr = curr == 2 ? 1 : curr;
           }
           if (totalCnt == 3)
           {
               switch (UtilData.PlayerSeat)
               {
                   case 1:
                       curr = curr == 3 ? 2 : curr;
                       break;
                   case 2:
                       {
                           if (curr == 3) curr = 2;
                           else if (curr == 2) curr = 1;
                           break;
                       }
               }
           }

           int next = (curr + totalCnt - 1) % totalCnt;

           if (totalCnt == 2)
           {
               return next == 1 ? 2 : next;
           }
           if (totalCnt == 3)
           {
               switch (UtilData.PlayerSeat)
               {
                   case 1:
                       return next == 2 ? 3 : next;
                   case 2:
                       {
                           if (next == 1) return 2;
                           if (next == 2) return 3;
                           break;
                       }
               }

               return next;
           }
           if (totalCnt == 4)
           {
               return next;
           }

           YxDebug.Log("查找 上个座位号 出错");
           return -1;
       }


       //玩家的座位与桌子相对的位子转换
       public static int GetChairId(int userSeat, int playerSeat)
        {
            if (App.GameKey != "jpmj")
            {
                return GetChairIdNormal(userSeat, playerSeat);
            }
            return GetChairIdForJp(userSeat, playerSeat);
        }

       private static int GetChairIdNormal(int userSeat, int playerSeat)
       {
           int chair = (userSeat - playerSeat + UtilData.CurrGamePalyerCnt) % UtilData.CurrGamePalyerCnt;

           if (UtilData.CurrGamePalyerCnt == 2)
           {
               chair = chair == 1 ? 2 : chair;
           }
           else if (UtilData.CurrGamePalyerCnt == 3)
           {
               chair = chair == 2 ? 3 : chair;
           }

           return chair;
       }

       private static int GetChairIdForJp(int userSeat, int playerSeat)
        {
            int chair = (userSeat - playerSeat + UtilData.CurrGamePalyerCnt) % UtilData.CurrGamePalyerCnt;

            if (UtilData.CurrGamePalyerCnt == 2)
            {
                chair = chair == 1 ? 2 : chair;
            }
            else if (UtilData.CurrGamePalyerCnt == 3)
            {
                switch (playerSeat)
                {
                    case 0:
                        break;
                    case 1:
                        chair = chair == 2 ? 3 : chair;
                        break;
                    case 2:
                        {
                            if (chair == 1) chair = 2;
                            else if (chair == 2) chair = 3;
                        }

                        break;
                }


            }

            return chair;
        }




        //玩家的座位与桌子相对的位子转换
        public static int GetChairId(int userSeat)
        {
            return GetChairId(userSeat, UtilData.PlayerSeat);
        }

        public static int TryGetInt(ISFSObject obj, string key)
        {
            if (key != null && obj.ContainsKey(key))
            {
                return obj.GetInt(key);
            }
            return UtilDef.DefInt;
        }

        public static int[] TryGetInArray(ISFSObject obj, string key)
        {
            if (key != null && obj.ContainsKey(key))
            {
                return obj.GetIntArray(key);
            }
            return null;
        }

        public static string TryGetInString(ISFSObject obj, string key)
        {
            if (key != null && obj.ContainsKey(key))
            {
                return obj.GetUtfString(key);
            }
            return "";
        }

        public static long TryGetInLong(ISFSObject obj, string key)
        {
            if (key != null && obj.ContainsKey(key))
            {
                return obj.GetLong(key);
            }
            return UtilDef.DefInt;
        }

        public static bool TryGetInBool(ISFSObject obj, string key)
        {
            if (key != null && obj.ContainsKey(key))
            {
                return obj.GetBool(key);
            }
            return false;
        }

        //两个赖子
        public static bool HasLaizi(int laizi1, int laizi2, int cardValue)
        {
            return cardValue == laizi1 || cardValue == laizi2;
        }
    }


}
