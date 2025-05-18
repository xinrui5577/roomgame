using UnityEngine;

namespace Assets.Scripts.Game.GangWu
{
    /// <summary>
    /// 下注区域的扑克管理
    /// </summary>
    public class BetPoker : MonoBehaviour
    {

        /// <summary>
        /// 左侧的扑克位置
        /// </summary>
        public GameObject LeftPokerPos;
        /// <summary>
        /// 右侧的扑克位置
        /// </summary>
        public GameObject RightPokerPos;
        /// <summary>
        /// 第一张扑克
        /// </summary>
        [HideInInspector]
        public PokerCard LeftPoker = null;
        /// <summary>
        /// 第二张扑克
        /// </summary>
        [HideInInspector]
        public PokerCard RightPoker = null;
        /// <summary>
        /// 用户数据
        /// </summary>
        //public PlayerPanel PlayerP;
        /// <summary>
        /// 牌型显示(同花等)
        /// </summary>
        public UISprite PokerType;
        ///// <summary>
        ///// 左牌值
        ///// </summary>
        [HideInInspector]
        public int LeftCardValue;
        ///// <summary>
        ///// 右牌值
        ///// </summary>
        [HideInInspector]
        public int RightCardValue;

      
        /// <summary>
        /// 翻牌
        /// </summary>
        public void TurnOverCard()
        {
            LeftPoker.TurnCard();
        }
      
        /// <summary>
        /// 关闭对象
        /// </summary>
        public void CloseGameObject(GameObject gob)
        {
            gob.SetActive(false);
        }
      

        public void Reset()
        {
            if (LeftPoker != null)
            {
                Destroy(LeftPoker.gameObject);
            }
            if (RightPoker != null)
            {
                Destroy(RightPoker.gameObject);
            }
            PokerType.gameObject.SetActive(false);
        }
    }

}