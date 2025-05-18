using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Public
{
    public class CpgDataCraeterFunc : MonoBehaviour {

        public virtual CpgData CreateCpg(ISFSObject data)
        {
            CpgData ret = null;
            EnGroupType type = EnGroupType.None;
            if (data.ContainsKey(RequestKeyOther.KeyTType))
                type = (EnGroupType)(data.GetInt(RequestKeyOther.KeyTType));
            else if (data.ContainsKey(RequestKey.KeyType))
                type = (EnGroupType)(data.GetInt(RequestKey.KeyType));

            switch (type)
            {
                case EnGroupType.Chi:
                    ret = new CpgChi();
                    break;
                case EnGroupType.Peng:
                    ret = new CpgPeng();
                    break;
                case EnGroupType.ZhuaGang:
                    ret = new CpgZhuaGang();
                    break;
                case EnGroupType.MingGang:
                case EnGroupType.AnGang:
                    if (data.ContainsKey(RequestKeyOther.CardBao))
                    {
                        ret = new CpgSelfGangBao();
                    }
                    else
                    {
                        ret = new CpgSelfGang();
                    }
                    break;
                case EnGroupType.PengGang:
                    ret = new CpgOtherGang();
                    break;
                case EnGroupType.XFGang:
                    ret = new CpgXFGang();
                    break;
                case EnGroupType.YaoDan:
                case EnGroupType.JiuDan:
                case EnGroupType.ZFBDan:
                case EnGroupType.XFDan:
                    ret = new CpgXfdGang();
                    break;
            }

            ret.ParseData(data);
            return ret;
        }
    }
}
