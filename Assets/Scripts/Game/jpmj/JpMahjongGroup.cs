using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj
{
    public class JpMahjongGroup : MahjongGroup
    {
        /// <summary>
        /// 游戏数据容器
        /// </summary>
        [SerializeField] protected TableData TableData;

        protected override Vector3 GetPos(MjIndex index)
        {
            Vector3 mahjongSize = MahjongManager.MagjongSize;

            return new Vector3(mahjongSize.x * (index.x + 0.5f), mahjongSize.y * (index.y + 0.5f), -mahjongSize.z * (index.z + 0.5f));
        }

        /// <summary>
        /// 排序牌时检查是否赋值杠头角标
        /// </summary>
        protected override void SetMahjongPos()
        {
            base.SetMahjongPos();
            if (TableData==null)return;

            var gtouValue =  TableData.Fanpai;
            bool isGantouChongCao = gtouValue >= 96 && gtouValue <= 103;
            foreach (MahjongItem mahjongItem in MahjongList)
            {
                var mjValue = mahjongItem.Value;

                if (isGantouChongCao &&
                    mjValue >= 96 && mjValue <= 103)
                {
                    mahjongItem.IsGangTou = true;
                    continue;
                }

                mahjongItem.IsGangTou = mjValue == gtouValue;
            }
        }
    }
}
