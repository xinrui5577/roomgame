using System;
using UnityEngine;

namespace Assets.Scripts.Game.fillpit
{
    public class Bet : MonoBehaviour
    {
        public TweenPosition Tp = null;

        /// <summary>
        /// 筹码上的数值
        /// </summary>
        public UILabel BetValueLabel = null;

        /// <summary>            
        /// 筹码物体的层级(valye),以及筹码上显示数字的层级(value - 1)
        /// </summary>
        public int BetDepth 
        { 
            set 
            { 
                gameObject.GetComponent<UIWidget>().depth = value;
                BetValueLabel.GetComponent<UIWidget>().depth = value + 1;
            } 
        }

        /// <summary>
        /// 当前的结束类型
        /// </summary>
        public BetFinishedType CurFinishedType = BetFinishedType.None;

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
        /// <param name="sprName">1-4</param>
        public void SetImage(string sprName)
        {
            //Image.spriteName = "Bet_" + index;
            GetComponent<UISprite>().spriteName = sprName;
        }

        /// <summary>
        /// 获得一个桌面上的随机位置，用于放筹码
        /// </summary>
        /// <returns>当前筹码在桌面上的位置</returns>
        public Vector3 GetTableRandomPos()
        {
            Vector3 tempPos = Vector3.zero;

            Vector3 pos = new Vector3(tempPos.x + UnityEngine.Random.Range(-100.0f, 100.0f), tempPos.y + UnityEngine.Random.Range(-50.0f, 50.0f), tempPos.z);
            return pos;
        }

        /// <summary>
        /// 移动筹码到某处
        /// </summary>
        /// <param name="from">起始位置</param>
        /// <param name="to">到达位置</param>
        /// <param name="delay">延迟</param>
        /// <param name="fType">筹码移动后的状态类型</param>
        /// <param name="callback">回调函数</param>
        public void BeginMove(Vector3 from, Vector3 to, float delay = 0, BetFinishedType fType = BetFinishedType.None, Action callback = null)
        {
            gameObject.SetActive(true);
            CurFinishedType = fType;
            Tp.from = from;
            Tp.to = to;
            Tp.delay = delay;
            if(callback != null)
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
