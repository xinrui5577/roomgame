using Assets.Scripts.Game.Fishing.enums;
using Assets.Scripts.Game.Fishing.Factorys;
using com.yxixia.utile.Utiles;
using UnityEngine;

namespace Assets.Scripts.Game.Fishing.datas
{
    public class FishFormationInfo : MonoBehaviour {
        /// <summary>
        /// 重复次数
        /// </summary>
        public uint Times = 1;
        /// <summary>
        /// 屏幕中的位置方向
        /// </summary>
        public EDirection Direction;
        /// <summary>
        /// 间隔
        /// </summary>
        public float Spacing;
        /// <summary>
        /// 鱼阵数据
        /// </summary>
        public FishFormationData[] FormationDatas;

        public void SprinkleOnce(int index, FishFactory fishFactory)
        {
            var len = FormationDatas.Length;
            var generator = fishFactory.TheGenerator;
            for (var i = 0; i < len; i++)
            {
                var data = FormationDatas[i];
                var ids = data.FishIds;
                var idLens = ids.Length;
                var findex = idLens > 1 ? index % idLens : 0;
                var fishId = ids.GetElement(findex);
                var ts = data.transform;
                var fishType = data.FishType;
                //创建鱼 
                var fishData = new FishData
                {
                    Direction = ts.eulerAngles.z,
                    FishId = fishId,
                    Info = generator.GetFishInfo(fishId, fishType),
                    Type = fishType
                };
                var swimmer = fishFactory.CreateSwimmer(fishData);
                if (swimmer != null)
                {
                    swimmer.Speed = data.Speed;
                    fishFactory.ResetSwimmer(swimmer, data.Path, ts.position, ts.eulerAngles.z);
                }
            }
        }

    }
}
