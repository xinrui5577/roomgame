using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;

namespace Assets.Scripts.Game.Fishing.datas
{
    /// <summary>
    /// 鱼的数据
    /// </summary>
    public class FishData
    {
        /// <summary>
        /// 服务器唯一id
        /// </summary>
        public int Id;
        /// <summary>
        /// 
        /// </summary>
        public int FishId;
        /// <summary>
        /// 类型
        /// </summary>
        public EFishType Type;
        /// <summary>
        /// 出生地，整数部分： 0：上 1：下 2：左 3：右    小数部分    长度的比值
        /// </summary>
        public float Homeplace;
        /// <summary>
        /// 出生时方向
        /// </summary>
        public float Direction;
        /// <summary>
        /// 倍数
        /// </summary>
        public int Rate;
        /// <summary>
        /// 数据
        /// </summary>
        public FishGenerator.FishInfo Info;

        public string FishIdKey()
        {
            return string.Format("{0}_{1}", FishId, Type);
        }
    }
}
