using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Public
{
    public class CpgMjCreateFunc : MonoBehaviour {

        //统一创建接口 cpgData 数据 index 为横着的index 为重连时读取本地留的位子
        public virtual MahjongCpg CreateCpg(CpgData data, int index = UtilDef.DefInt, int arrowindex = UtilDef.NullArrowIndex)
        {
            GameObject cpg = new GameObject();
            MahjongCpg ret = null;
            switch (data.Type)
            {
                case EnGroupType.Chi:
                    ret = cpg.AddComponent<MahjongCpgChi>();
                    break;
                case EnGroupType.Peng:
                case EnGroupType.PengGang:
                    ret = cpg.AddComponent<MahjongCpg>();
                    break;
                case EnGroupType.ZhuaGang:
                    ret = cpg.AddComponent<MahjongCpgZhuaGang>();
                    break;
                case EnGroupType.MingGang:
                case EnGroupType.AnGang:
                    ret = cpg.AddComponent<MahjongCpgAngang>();
                    break;
                case EnGroupType.XFGang:
                    ret = cpg.AddComponent<MahjongCpgSelfGang>();
                    break;
            }

            ret.Init(data, index, arrowindex);
            return ret;
        }
    }
}
