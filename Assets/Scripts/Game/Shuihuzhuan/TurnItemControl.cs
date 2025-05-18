using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class TurnItemControl : MonoBehaviour
    {   /// <summary>
        ///数组位置 
        /// </summary>
        public int kind;
        /// <summary>
        /// 时间
        /// </summary>
        public int timeKind;
        /// <summary>
        /// 
        /// </summary>
        private Image image;
        /// <summary>
        /// 自己的位置
        /// </summary>
        public Transform homePosition;
        /// <summary>
        /// 可以移动
        /// </summary>
        public bool canMove = true;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        /// <summary>
        /// 速度
        /// </summary>
        private float speed = 50.0f;
#else 
        private float speed = 50.0f;
#endif
        /// <summary>
        /// 转动数字
        /// </summary>
        public int turnNum = 0;
        /// <summary>
        /// 限制转
        /// </summary>
        public int limitTurn = 8;
        /// <summary>
        /// 玩家赢的钱
        /// </summary>
        public Animator ani;

        void Awake()
        {
            image = gameObject.GetComponent<Image>();
            ani = gameObject.GetComponent<Animator>();
            ani.enabled = false;
        }
        public void OnEnable()
        {
           //Open();
        }

        public void Open()
        {
            Animator an = gameObject.GetComponent<Animator>();
            an.Stop();
        }
        public void StopMoveing()
        {
            canMove = false;

        }


        void Update()
        {
            var server = App.RServer;
            if (server == null) return;
            if (!server.HasGetGameInfo) return;
            if (this.transform.localPosition.y <= TurnControl.instance.endTransforms[1].localPosition.y)
            {
                transform.localPosition = TurnControl.instance.bornTransforms[kind].localPosition;
                image.sprite = TurnControl.instance.cardSprites[Random.Range(0, 9)];
                if (turnNum >= limitTurn)
                {
                    if (timeKind != 0)
                    {
                        // Moveing();
                        ShowResult.instance.Moveing(timeKind);
                    }

                    transform.localPosition = homePosition.localPosition;
                    gameObject.SetActive(false);
                    canMove = false;
                    turnNum = 0;
                }
                if (BottomUIControl.instance.skip_bool)
                {
                    turnNum += 20;
                }
                else
                {
                    turnNum += 1; 
                }
                
            }
            if (canMove == true)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - speed, transform.localPosition.z);
            }
        }
    }


}
