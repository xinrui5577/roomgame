using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Public
{ //CPG类型
    public enum EnGroupType
    {
        Chi = 1,
        Peng = 2,
        WZhuaGang = 3,
        /// <summary>
        /// 先碰，后杠
        /// </summary>
        ZhuaGang = 4,

        /// <summary>
        /// 直接杠别人的
        /// </summary>
        PengGang = 5,

        /// <summary>
        /// 明杠
        /// </summary>
        MingGang = 6,

        /// <summary>
        /// 暗杠
        /// </summary>
        AnGang = 7,
        /// <summary>
        /// 旋风杠
        /// </summary>
        XFGang = 10,

        /// <summary>
        /// 绝杠 -暗杠
        /// </summary>
        AnJueGang = 0xb,

        /// <summary>
        /// 旋风杠，以下四条长春麻将专用
        /// </summary>
        XiaoJi = 100,

        YaoDan = 101,

        JiuDan = 102,

        ZFBDan = 103,

        XFDan = 104,

        None,
    }

    public class CpgDataCreater
    {
        public static CpgData CreateCpg(ISFSObject data)
        {
            var createrFunc = GameObject.Find("GameData").GetComponent<CpgDataCraeterFunc>();
            if (createrFunc != null)
                return createrFunc.CreateCpg(data);

            return null;
        }
    }

    public class CpgMahjongCreater
    {
        public static MahjongCpg CreateCpg(CpgData data, int index = UtilDef.DefInt,int arrowIndex = -1)
        {
            var createrFunc = GameObject.Find("GameData").GetComponent<CpgMjCreateFunc>();
            if (createrFunc != null)
                return createrFunc.CreateCpg(data, index, arrowIndex);

            return null;
        }
    }
}