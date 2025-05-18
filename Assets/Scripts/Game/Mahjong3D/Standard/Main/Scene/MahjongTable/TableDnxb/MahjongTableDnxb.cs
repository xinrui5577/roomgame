using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 方向
    /// </summary>
    public enum DnxbDirection
    {
        East,
        South,
        West,
        North,
        None,
    }

    /// <summary>
    /// 方向类型
    /// </summary>
    public enum DnxbDirectionType
    {
        /// <summary>
        /// 有方向
        /// </summary>
        Direction,
        /// <summary>
        /// 无方向
        /// </summary>
        DirectionLess,
    }

    public class MahjongTableDnxb : MahjongTablePart, IGameInfoICycle
    {
        public DnxbDirectionType DirectionType = DnxbDirectionType.Direction;
        public DnxbObjItem[] DnxbObjItems;

        public void SwitchDirection(DnxbDirection direction)
        {
            for (int i = 0; i < DnxbObjItems.Length; i++)
            {
                DnxbObjItems[i].CurrentState = i == (int)direction;
            }
        }

        public override void OnReset()
        {
            SwitchDirection(DnxbDirection.None);
        }

        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
        }

        public void OnGameInfoICycle()
        {
            SetDriectionMaterial();
            var db = GameCenter.DataCenter;
            SetDirectionAngle(db.MaxPlayerCount, db.OneselfData.Seat);
        }

        public void SetDriectionMaterial()
        {
            DirectionType = GameCenter.DataCenter.MaxPlayerCount == 3 ? DnxbDirectionType.DirectionLess : DnxbDirectionType.Direction;
            var materials = GameCenter.Assets.MaterialAssets;
            Material directionLightMate = null;
            Material directionMate = null;
            switch (DirectionType)
            {
                case DnxbDirectionType.Direction:
                    directionMate = materials.GetAsset<Material>("DirectionNormalDark");
                    directionLightMate = materials.GetAsset<Material>("DirectionNormalLight");
                    break;
                case DnxbDirectionType.DirectionLess:
                    directionMate = materials.GetAsset<Material>("DirectionSpecialDark");
                    directionLightMate = materials.GetAsset<Material>("DirectionSpecialLight");
                    break;
            }
            for (int i = 0; i < DnxbObjItems.Length; i++)
            {
                DnxbObjItems[i].OnInitiation(directionMate, directionLightMate);
            }
        }

        /// <summary>
        /// 设置方向角度
        /// </summary>
        /// <param name="playerCount">玩家数量</param>
        /// <param name="seat">自己服务器座位号</param>
        private void SetDirectionAngle(int playerCount, int seat)
        {
            var direction = transform.FindChild("direction");
            if (playerCount == 2)
            {
                //如果是2人场，方向旋转180                
                direction.localRotation = Quaternion.Euler(new Vector3(0, 180 * seat, 0));
            }
            else if (playerCount == 4)
            {
                Vector3 offsetAngle = new Vector3(0, 90 * seat, 0);
                direction.localRotation = Quaternion.Euler(offsetAngle);
            }
        }
    }
}