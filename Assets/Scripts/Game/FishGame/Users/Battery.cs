using System;
using Assets.Scripts.Game.FishGame.Common.core;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.FishGame.Users
{
    public class Battery : MonoBehaviour
    {
        /// <summary>
        /// 座位号
        /// </summary>
        public int SeatNumber;
        /// <summary>
        /// 锚点
        /// </summary>
        public UIAnchor.Side Side;
        /// <summary>
        /// 玩家样式
        /// </summary>
        [SerializeField, UsedImplicitly] 
        private Player _prefabPlayer;

        public Color ColorType;
        public Player User { get; private set; } 


        public Player CreatePlayer()
        {
            User = Instantiate(_prefabPlayer);
            var ts = User.transform;
            ts.parent = transform;
            ts.localPosition = Vector3.zero;
            ts.localScale = Vector3.one;
            ts.localRotation = Quaternion.identity;
            User.Idx = SeatNumber;
            User.CtrllerIdx = SeatNumber;
            User.ColorType = ColorType;
            User.Display(false);
            return User;
        } 

        //设置座位号
        public void SetSeatNumber(int num)
        {
            SeatNumber = num;
            if (User == null) return;
            User.Idx = num;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position,10);
        }
    }
}
