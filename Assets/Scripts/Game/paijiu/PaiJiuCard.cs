using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.paijiu
{
    public class PaiJiuCard : MonoBehaviour
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            _changeFEvent = new EventDelegate(Onfinish);
            _changeOEvent = new EventDelegate(Onfinish1);
        }

        public void MoveFinish()
        {
            transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }


        /// <summary>
        /// 牌ID = 花色 + 面值
        /// </summary>
        public int Id;
        /// <summary>
        /// 扑克显示对象
        /// </summary>
        public UISprite Front;

        public UISprite TurnCardAnim;

        public int CardIndex = -1;

        public bool IsSelf;


        /*************翻转牌**************/
        private EventDelegate _changeFEvent = new EventDelegate();
        private EventDelegate _changeOEvent = new EventDelegate();

        private void Onfinish()
        {
            TweenRotation rightTweenR = GetComponent<TweenRotation>();

            SetCardFront();

            rightTweenR.RemoveOnFinished(_changeFEvent);

            rightTweenR.from = new Vector3(0, -90, 0);
            rightTweenR.to = new Vector3(0, 0, 0);
            if (_changeOEvent == null)
                _changeOEvent = new EventDelegate(Onfinish1);
            rightTweenR.AddOnFinished(_changeOEvent);
            rightTweenR.ResetToBeginning();
            rightTweenR.PlayForward();
        }
        private void Onfinish1()
        {
            TweenRotation rightTweenR = GetComponent<TweenRotation>();
            rightTweenR.RemoveOnFinished(_changeOEvent);
        }

        bool _isPlaying;

        public void TurnCard()
        {
            TurnCardAnim.gameObject.SetActive(true);
            Front.gameObject.SetActive(false);
            TurnCardAnim.enabled = true;


            if (_isPlaying) return;

            _isPlaying = true;
            StartCoroutine(WaitForSet());
        }

        IEnumerator WaitForSet()
        {
            float sec = 1f / 45;
            int index = 0;
            //播放翻牌动画
            while (index < 6)
            {
                TurnCardAnim.spriteName = "turn_" + index++;
                TurnCardAnim.MakePixelPerfect();
                yield return new WaitForSeconds(sec);
            }
            SetCardFront();
            yield return new WaitForSeconds(4 * sec);
            SetScale();
            _isPlaying = false;
            StopAllCoroutines();
        }


        public void SetScale()
        {
            var ts = gameObject.GetComponent<TweenScale>();
            var list = new System.Collections.Generic.List<EventDelegate>();
            TweenScale.Begin(gameObject, 0.1f, new Vector3(0.9f, 0.9f, 0.9f));
            list.Add(new EventDelegate(() =>
            {
                TweenScale.Begin(gameObject, 0.1f, Vector3.one * 0.8f);
            }));
            ts.onFinished = list;
        }

        public int Val
        {
            get { return Id % 0x10; }
        }


        public void SetCardId(int id)
        {
            Id = id;
        }

        public string GetCardValueStr()
        {
            string result = Id == 0 ? "back" : "front";
            result = (Id == 81 || Id == 97 || Id == 113) ? "0x" + Id.ToString("x") : result;
            return result;
        }

        public string GetCardShowNumStr()
        {
            return null;
        }

        /// <summary>
        /// 是不是被选中牌
        /// </summary>
        public bool IsSelected;
        public void MoveUp()
        {
            IsSelected = true;
            Vector3 v3 = transform.localPosition;
            transform.localPosition = new Vector3(v3.x, 20, v3.z);
        }

        public void MoveDown()
        {
            IsSelected = false;
            Vector3 v3 = transform.localPosition;
            transform.localPosition = new Vector3(v3.x, 0, v3.z);
        }

        /// <summary>
        /// 当点击牌,外挂方法
        /// </summary>
        public void OnClickCard()
        {
            var betPoker = GetComponentInParent<BetPoker>();
            if (betPoker != null)
            {
                betPoker.OnClickCard(CardIndex);
            }
        }

        /// <summary>
        /// 牌是否可以点击
        /// </summary>
        /// <param name="could">能否点击</param>
        public void CardCouldClick(bool could)
        {
            GetComponent<BoxCollider>().enabled = could;
        }

        ///// <summary>
        ///// 仅用于对分
        ///// </summary>
        ///// <returns></returns>
        //public string GetBigColorStr()
        //{
        //    return null;
        //}

        //public void SetDiPaiMark(bool active)
        //{

        //}

        public void SetCardDepth(int depth)
        {
            Front.depth = depth;
            TurnCardAnim.depth = depth;
        }

        /// <summary>
        /// 设置扑克显示正面
        /// </summary>
        public void SetCardFront()
        {
            Front.spriteName = Id.ToString("x3");
            Front.gameObject.SetActive(true);
            TurnCardAnim.gameObject.SetActive(false);
        }


        ///// <summary>
        ///// 设置牌的公共标记
        ///// </summary>
        ///// <param name="isPublicCard">是否是公共牌</param>
        //public void SetPublicMark(bool isPublicCard)
        //{

        //}

        /// <summary>
        /// 被选中的
        /// </summary>
        //public void Selected()
        //{
        //    IsSelect = true;
        //    Line.gameObject.SetActive(true);
        //    Sprite.transform.localPosition = new Vector3(0f,20f,0f);
        //    SetColor(Color.white);
        //    Line.color = new Color(1f, 0.6f, 0f);
        //}
        ///// <summary>
        ///// 初始化选中
        ///// </summary>
        //public void InitSelect()
        //{
        //    IsSelect = false;
        //    Line.gameObject.SetActive(false);
        //    Sprite.transform.localPosition = Vector3.zero;
        //    SetColor(Color.white);
        //}

        //移动结束
        public void OnMoveFinish(System.Action action = null)
        {
            if (action == null)
                return;
            action();
        }

    }
}
