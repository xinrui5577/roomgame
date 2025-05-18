using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class Bet : MonoBehaviour
    {
        public TweenPosition Tp = null;

        public GameObject HideObj;

        /// <summary>
        /// 筹码上的数值
        /// </summary>
        public UILabel BetValueLabel = null;

        /// <summary>
        /// 筹码最终飞向的玩家
        /// </summary>
        public Transform TargetTran;

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
        }

        /// <summary>
        /// 设置筹码图片
        /// </summary>
        /// <param name="index">1-4</param>
        public void SetImage(int index)
        {
            GetComponent<UISprite>().spriteName = string.Format("bet_{0}", index.ToString());
        }


        /// <summary>
        /// 设置筹码图片
        /// </summary>
        /// <param name="spriteName">图片名称</param>
        public void SetImage(string spriteName)
        {
            GetComponent<UISprite>().spriteName = spriteName;
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


        public void BeginMove(Vector3 to, float duration = 0.5f, float delay = 0)
        {
            gameObject.SetActive(true);
            Tp.from = transform.localPosition;
            Tp.to = to;
            Tp.duration = duration;
            Tp.delay = delay;

            Tp.ResetToBeginning();
            Tp.PlayForward();

            Finished = /*MoveTo;/*/Wait;//*/
        }

        void Wait()
        {
            StartCoroutine(Testt());
        }

        IEnumerator Testt()
        {
            yield return new WaitForSeconds(0.8f);
            MoveTo();
        }


        void FinishEnd()
        {
            gameObject.SetActive(false);
        }


        void MoveTo()
        {
            transform.parent = TargetTran;
            Tp.from = transform.localPosition;
            Tp.to = Vector3.zero;
            Tp.ResetToBeginning();
            Tp.PlayForward();

            Finished = FinishEnd;
            StopCoroutine("Testt");
        }
    }

    /// <summary>
    /// 筹码运动完成后的操作类型
    /// </summary>
    public enum BetFinishedType
    {
        /// <summary>
        /// 没有操作
        /// </summary>
        None,
        /// <summary>
        /// 销毁
        /// </summary>
        Destroy,
        /// <summary>
        /// 隐藏
        /// </summary>
        Hide,
    }


}
