using System.Collections.Generic;
using Sfs2X.Entities.Data;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.LXGameScripts
{
    public class ResponseData
    {
        /// <summary>
        /// 显示的激光列表
        /// </summary>
        public List<int> LineList = new List<int>();
        /// <summary>
        /// 用户身上的金币
        /// </summary>
        public long UserTotalGold = 0;
        /// <summary>
        /// 本局获得的金币
        /// </summary>
        public int GetJackpotGold = 0;
        /// <summary>
        /// 显示的图片列表
        /// </summary>
        public List<int> JettonList = new List<int>();
        /// <summary>
        /// 自动停止时用的显示图片列表
        /// </summary>
        public List<int> IconList = new List<int>();
        /// <summary>
        /// 是否中奖
        /// </summary>
        public bool IsWin = false;
        /// <summary>
        /// 是否都相同
        /// </summary>
        public bool IsSame = false;
        /// <summary>
        /// 共显示几列图片
        /// </summary>
        public int ShowLine = 0;
        /// <summary>
        /// 最大倍数是哪条线
        /// </summary>
        public int MaxLine = 0;

        /// <summary>
        /// 从服务器接收游戏数据并转化
        /// </summary>
        public virtual void ParseData(ISFSObject sfsObject)
        {
            LineList.Clear();
            JettonList.Clear();
            IconList.Clear();

            string strLogLines = "";
            int[] lines = sfsObject.GetIntArray("lines");
            for (int i = 0; i < lines.Length; i++)
            {
                LineList.Add(lines[i]);
                strLogLines = strLogLines + " " + lines[i] + " ";
            }
            YxDebug.Log("  ----> RespStart: Lines info of start data is " + strLogLines + " ! \n");

            IsWin = LineList.Contains(1);

            UserTotalGold = sfsObject.GetLong("ttgold");
            GetJackpotGold = sfsObject.GetInt("gold");
            IsSame = sfsObject.GetBool("same");

            MaxLine = sfsObject.GetInt("maxline");


            string strLogJet = "";
            int[] jettons = sfsObject.GetIntArray("fruits");
            /*下面这段是横着来的图片顺序
            string strLogJet1 = "";
            for (int i = 0; i < jettons.Length; i++)
            {
                strLogJet1 = strLogJet1 + " " + jettons[i] + " ";
            }
            YxDebug.LogError("  ----> RespStart: Jetton info of start data is " + strLogJet1 + " ! \n");
            */
            //服务器传过来的数据是横着来的,而我做的是竖着来的,需要把横着的图片顺序改为竖着的图片顺序
            int temp = 0;
            for (int i = 0; i < ShowLine; i++)
            {
                if (i + temp * ShowLine < jettons.Length)
                {
                    JettonList.Add(jettons[i + temp * ShowLine]);
                    strLogJet = strLogJet + " " + jettons[i + temp * ShowLine] + "   ";
                    temp++;
                    i--;
                }
                else
                    temp = 0;
            }
            YxDebug.Log("  ----> RespStart: Jetton info of start data is " + strLogJet + " ! \n");
        }
        /// <summary>
        /// 自动停止时获得该位置的图片名称
        /// </summary>
        /// <param name="str">图集的图片名</param>
        /// <returns>图集中某个图片名</returns>
        public virtual string GetJettonName(string str)
        {
            string temp = str + IconList[0];
            IconList.RemoveAt(0);
            return temp;
        }
        /// <summary>
        /// 对服务器传来的显示图片顺序进行重新排列,适合自动停止时的顺序
        /// </summary>
        public virtual void RegroupIconList()
        {
            if (JettonList.Count % ShowLine != 0)
                return;
            int num = JettonList.Count / ShowLine;
            for (int i = 1; i <= ShowLine; i++)
            {
                for (int j = i * num - 1; j >= (i - 1) * num; j--)
                {
                    IconList.Add(JettonList[j]);
                }
            }
        }
        /// <summary>
        /// 三九连线判断是否需要砸蛋
        /// </summary>
        public bool NeedPlayZaDan(int line, int row)
        {
            if (!IsWin) return false;
            //if (IsSame) return true;//还没有接收Same数据,所以先注释
            List<int> temp = new List<int>();
            for (int i = 0; i < LineList.Count; i++)
            {
                if (LineList[i] == 1)
                    temp.Add(i);
            }
            if (temp.Count <= 0) return false;
            foreach (var item in temp)
            {
                if (item + 1 == row) return true;
                if (item == line + 3) return true;
                if (line + 1 == row && item == 7) return true;
                if (2 - line == row && item == 6) return true;
            }
            return false;
        }
    }
}