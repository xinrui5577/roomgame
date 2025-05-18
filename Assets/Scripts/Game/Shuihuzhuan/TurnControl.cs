using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class TurnControl : MonoBehaviour {
        /// <summary>
        /// 转动控制实例
        /// </summary>
        public static TurnControl instance;
        /// <summary>
        /// 开始转换
        /// </summary>
        public Transform[] bornTransforms;
        /// <summary>
        /// 转换结束
        /// </summary>
        public Transform[] endTransforms;
        /// <summary>
        /// 剩下的卡片
        /// </summary>
        public Transform[] homeTransforms;
        /// <summary>
        /// 有颜色的9张卡
        /// </summary>
        public Sprite[] cardSprites;
        /// <summary>
        /// 没有颜色的9张卡
        /// </summary>
        public Sprite[] graySprites;
        /// <summary>
        /// 可移动
        /// </summary>
        public bool canMove = false;
        /// <summary>
        ///开始按钮
        /// </summary>
        public Button beginBtn;
        /// <summary>
        /// 暂停按钮
        /// </summary>
        public Button stopBtn;
        /// <summary>
        /// 转的图片
        /// </summary>
        public GameObject[] turnItems;
        /// <summary>
        /// 转
        /// </summary>
        public Image[] resultItems;
        /// <summary>
        /// 位置
        /// </summary>
        public Image[] resultImages;
        void Awake()
        {
            instance = this;
        }
        /// <summary>
        ///打开开始按钮
        /// </summary>
        /// <param name="isStop"></param>
        public void SetStop(bool isStop)//开始和暂停的按钮
        {
            //beginBtn.gameObject.SetActive(isStop);
            stopBtn.gameObject.SetActive(!isStop);
        }
        public void GameResultFun()
        {
            SetStop(false);
            for (int i = 0; i < resultImages.Length; i++)
            {
                resultImages[i].GetComponent<Animator>().enabled = false;
            }  
            Play();
        }
        public void Play()
        {
            canMove = true;

            for (int i = 0; i < turnItems.Length; i++)
            {
                turnItems[i].GetComponent<TurnItemControl>().canMove = true;
                turnItems[i].GetComponent<TurnItemControl>().turnNum = 0;
            }
        }
    }
}
