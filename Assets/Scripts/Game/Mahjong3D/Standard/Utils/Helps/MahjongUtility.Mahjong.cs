using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class MahjongUtility
    {
        public static CpgData CreateCpg(ISFSObject data)
        {
            CpgData ret = null;
            EnGroupType type = EnGroupType.None;
            if (data.ContainsKey(AnalysisKeys.KeyTType))
            {
                type = (EnGroupType)(data.GetInt(AnalysisKeys.KeyTType));
            }
            else if (data.ContainsKey(RequestKey.KeyType))
            {
                type = (EnGroupType)(data.GetInt(RequestKey.KeyType));
            }
            switch (type)
            {
                case EnGroupType.Chi:
                    ret = new CpgChi();
                    break;
                case EnGroupType.JueGang:
                case EnGroupType.Peng:
                    ret = new CpgPeng();
                    break;
                case EnGroupType.ZhuaGang:
                    ret = new CpgZhuaGang();
                    break;
                case EnGroupType.MingGang:
                case EnGroupType.AnGang:
                    {
                        if (data.ContainsKey("bao"))
                        {
                            ret = new CpgSelfGangBao();
                        }
                        else
                        {
                            ret = new CpgSelfGang();
                        }
                    }
                    break;
                case EnGroupType.PengGang:
                    ret = new CpgOtherGang();
                    break;
                case EnGroupType.XFGang:
                case (EnGroupType)NetworkProls.CPGXFG:
                    ret = new CpgXFGang();
                    break;
                case EnGroupType.XFDan:
                case EnGroupType.ZFBDan:
                case EnGroupType.YaoDan:
                case EnGroupType.JiuDan:
                    ret = new CpgXfdGang();
                    break;
                case EnGroupType.AnJueGang:
                    ret = new CpgAnJueGang();
                    break;
            }
            ret.ParseData(data);
            return ret;
        }

        public static MahjongCpg CreateCpg(CpgData data, int index = DefaultUtils.DefInt)
        {
            GameObject cpg = new GameObject();
            MahjongCpg ret = null;
            switch (data.Type)
            {
                case EnGroupType.Chi:
                    ret = cpg.AddComponent<MahjongCpgChi>();
                    break;
                case EnGroupType.JueGang:
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
                    ret = cpg.AddComponent<MahjongCpgXFGang>();
                    break;
                case EnGroupType.YaoDan:
                case EnGroupType.JiuDan:
                case EnGroupType.ZFBDan:
                case EnGroupType.XFDan:
                    ret = cpg.AddComponent<MahjongCpgXjfdGang>();
                    break;
                case EnGroupType.AnJueGang:
                    ret = cpg.AddComponent<MahjongCpgAnJuegang>();
                    break;
            }
            ret.Init(data, index);
            return ret;
        }
    }
}