using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.fillpit.skin1
{
    public class PubCardScript : MonoBehaviour
    {
        public UITweener[] Tweens;

        public PokerCard[] Cards;

        public PokerCard CardPrefab;

        public GameObject PassBtn;

        #region  测试使用
        //public int[] CardsVal;
        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.W))
        //    {
        //        PlayCardsTweens();
        //    }
        //    if (Input.GetKeyDown(KeyCode.R))
        //    {
        //        ShowView(CardsVal);
        //    }
        //}
        #endregion


        /// <summary>
        /// 牌的初始化原点位置
        /// </summary>
        public Vector3 CardOriginPos;

        public void ShowView(int[] cardVal)
        {
            int len = cardVal.Length;
            InitCardCount(len);

            gameObject.SetActive(true);
            Reset();
            for (int i = 0; i < len; i++)
            {
                var card = Cards[i];
                card.SetCardId(cardVal[i]);
                card.SetCardFront();
                card.SetCardDepth(100 + i*2);
                //牌的位置初始化
                card.transform.localPosition = new Vector3(GetPosX(len, i), CardOriginPos.y, CardOriginPos.z);
                card.transform.localEulerAngles = Vector3.zero;
            }
        }

        /// <summary>
        /// 初始化牌的个数
        /// </summary>
        /// <param name="cardCount"></param>
        void InitCardCount(int cardCount)
        {
            if (Cards != null && cardCount == Cards.Length) return;
            //销毁已有的牌
            if (Cards != null)
            {
                for (int i = 0; i < Cards.Length; i++)
                {
                    if(Cards[i] != null)
                        Destroy(Cards[i].gameObject);
                }
            }

            //创建牌
            Cards = new PokerCard[cardCount];
            for (int i = 0; i < cardCount; i++)
            {
                var card = Instantiate(CardPrefab);
                var prefabTran = CardPrefab.transform;
                card.gameObject.SetActive(true);
                card.transform.parent = prefabTran.parent;
                card.transform.localScale = prefabTran.localScale;
                Cards[i] = card;
            }
        }

      

        int GetPosX(int len, int index)
        {
            int mid = len/2;
            int space = 8;
            return (index - mid)*space;
        }

        private void Reset()
        {
            int len = Tweens.Length;
            for (int i = 0; i < len; i++)
            {
                var tween = Tweens[i];
                tween.ResetToBeginning();
                tween.enabled = false;
            }

            len = Cards.Length;
            for (int i = 0; i < len; i++)
            {
                var card = Cards[i];
                var tr = card.GetComponent<TweenRotation>();
                StopPlayTween(tr);
                var tp = card.GetComponent<TweenPosition>();
                StopPlayTween(tp);
            }

            SetBoxcolliderEnable(true);
            _isPlaying = false;
            PassBtn.SetActive(true);
        }


        public void PlayTweens()
        {
            int len = Tweens.Length;
            for (int i = 0; i < len; i++)
            {
                PlayTween(Tweens[i]);
            }
        }
       

        public float Angular = 18;
        void PlayCardsTweens()
        {
            int len = Cards.Length;
            float middleIndex = (float)(len - 1)/2;      //第一对牌的旋转角度
            for (int i = 0; i < len; i++)
            {
                var cardTran = Cards[i].transform;

                var tp = cardTran.GetComponent<TweenPosition>();
                if (tp != null)
                {
                    tp.from = cardTran.localPosition;
                    tp.to = CardOriginPos;          //回到牌的原点
                    PlayTween(tp);
                }

                //扇形展开牌
                var tr = cardTran.GetComponent<TweenRotation>();

                if (tr == null) continue;

                var from = Vector3.zero;
                tr.from = from;
                tr.to = new Vector3(0, 0, (middleIndex - i)*Angular);
                PlayTween(tr);
            }
        }


        private void StopPlayTween(UITweener tween)
        {
            if (tween == null) return;
            tween.ResetToBeginning();
            tween.enabled = false;
        }


        private void PlayTween(UITweener tween)
        {
            if (tween == null) return;
            tween.ResetToBeginning();
            tween.PlayForward();
        }

        /// <summary>
        /// 外挂方法
        /// </summary>
        public void FinishPlayHide()
        {
            gameObject.SetActive(false);
        }

        private bool _isPlaying;
        /// <summary>
        /// 界面消失,有过程
        /// </summary>
        public void PlayHide()
        {
            if (_isPlaying) return;
            _isPlaying = true;
            PlayTweens();
            PlayCardsTweens();
            SetBoxcolliderEnable(false);
            PassBtn.SetActive(false);
        }

        /// <summary>
        /// 设置牌是否可以拖拽
        /// </summary>
        /// <param name="b"></param>
        private void SetBoxcolliderEnable(bool b)
        {
            int len = Cards.Length;
            for (int i = 0; i < len; i++)
            {
                var bc = Cards[i].GetComponentInChildren<BoxCollider>();
                bc.enabled = b;
            }
        }


        public void OnClickPass()
        {
            App.GetGameManager<FillpitGameManagerSk1>().SendFinishRubDone();
        }
    }
}
