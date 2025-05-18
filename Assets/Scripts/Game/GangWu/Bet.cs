using System;
using UnityEngine;

namespace Assets.Scripts.Game.GangWu
{
    public class Bet : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public TweenPosition Tp;
        /// <summary>
        /// 筹码显示
        /// </summary>
        public UISprite Image;
        /// <summary>
        /// 当前的结束类型
        /// </summary>
        public BetFinishedType CurFinishedType;


        public Action Finished;

        public void OnFinshed()
        {
            Finished();
            switch (CurFinishedType)
            {
                case BetFinishedType.None:
                    break;
                case BetFinishedType.Destroy:
                    Destroy(gameObject);
                    break;
                case BetFinishedType.Hide:
                    gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 设置筹码图片
        /// </summary>
        /// <param name="index"></param>
        public void SetImage(int index)
        {
            Image.spriteName = "AddBet_" + index;
        }

        /// <summary>
        /// 开始移动
        /// </summary>
        public void BeginMove(Vector3 from, Vector3 to,float delay = 0, BetFinishedType fType = BetFinishedType.None, Action callback = null)
        {
            gameObject.SetActive(true);
            CurFinishedType = fType;
            Tp.from = from;
            Tp.to = to;
            Tp.delay = delay;
            Finished = callback;
            Tp.ResetToBeginning();
            Tp.PlayForward();
        }
    }

    /// <summary>
    /// 筹码运动完成后的操作类型
    /// </summary>
    public enum BetFinishedType
    {
        None, //没有操作
        Destroy, //删除
        Hide, //隐藏
    }
}
