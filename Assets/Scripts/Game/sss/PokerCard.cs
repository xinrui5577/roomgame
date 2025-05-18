using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.sss
{
    public class PokerCard : MonoBehaviour
    {
        protected void Start()
        {
            Init();
        }

        void Init()
        {
            _changeFEvent = new EventDelegate(Onfinish);
            _changeOEvent = new EventDelegate(Onfinish1);
        }

        public void OnMoveFinish()
        {
            transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }
   

        /// <summary>
        /// 牌的起始位置
        /// </summary>
        [HideInInspector]
        public Transform From;
        /// <summary>
        /// 牌的目标位置
        /// </summary>
        [HideInInspector]
        public Transform To;
        /// <summary>
        /// 花色
        /// </summary>
        public int Colour;
        /// <summary>
        /// 面值
        /// </summary>
        public int Value;
        /// <summary>
        /// 牌ID = 花色 + 面值
        /// </summary>
        public int Id;
        /// <summary>
        /// 扑克显示对象
        /// </summary>
        public UISprite Sprite;
        /// <summary>
        /// 显示牌型的线
        /// </summary>
        //public UISprite Line;
        /// <summary>
        /// 显示牌值
        /// </summary>
        public UISprite Num;
        /// <summary>
        /// 小花色显示
        /// </summary>
        public UISprite SmallColor;
        /// <summary>
        /// 大花色显示
        /// </summary>
        public UISprite BigColor;

        /// <summary>
        /// 被选中牌的特殊表现
        /// </summary>
        public UISprite Mask;

        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsSelect;

        /// <summary>
        /// 记录牌的索引
        /// </summary>
        [HideInInspector]
        public int Index = 0;

        private int _depth;

        /*************翻转牌**************/
        private EventDelegate _changeFEvent;
        private EventDelegate _changeOEvent;

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

        public PokerCard(int id)
        {
            SetCardId(id);
        }

        private int GetColor(int id)
        {
            return id >> 4;
        }

        private int GetValue(int id)
        {
            id = id & 0xf;
            return id;
        }

        public void SetCardId(int id)
        {
            Id = id;
            Colour = GetColor(id);
            Value = GetValue(id);
        }

        public string GetCardValueStr()
        {
            string result = Id == 0 ? "back" : "front";
            result = (Id == 81 || Id == 97 || Id == 113) ? "0x" + Id.ToString("x") : result;
            return result;
        }

        public string GetCardShowNumStr()
        {
            string result = (Colour == 1 || Colour == 2) ? "red_" + Value.ToString("x") : "black_" + Value.ToString("x");
            return result;
        }

        public string GetBigColorStr()
        {
            string result = "b_" + Colour + "_";
            result += (Value > 10 && Value < 14) ? Value.ToString("x") : "0";
            return result;
        }

        public void SetCardDepth(int depth)
        {
            _depth = depth;
            SetDepth(depth);
        }

        void SetDepth(int depth)
        {
            Sprite.depth = depth;
            Num.depth = depth + 1;
            SmallColor.depth = depth + 1;
            BigColor.depth = depth + 1;
            if (Mask != null)
                Mask.depth = depth + 2;
        }

        /// <summary>
        /// 设置扑克显示成扑克
        /// </summary>
        public void SetCardFront()
        {
            Sprite.spriteName = GetCardValueStr();
            if (Id == 81 || Id == 97 || Id == 113)
            {
                Num.gameObject.SetActive(false);
                SmallColor.gameObject.SetActive(false);
                BigColor.gameObject.SetActive(false);
            }
            else
            {
                Num.gameObject.SetActive(true);
                SmallColor.gameObject.SetActive(true);
                BigColor.gameObject.SetActive(true);
                Num.spriteName = GetCardShowNumStr();
                Num.MakePixelPerfect();
                SmallColor.spriteName = "s_" + Colour + "_0";
                SmallColor.MakePixelPerfect();
                BigColor.spriteName = GetBigColorStr();
                BigColor.MakePixelPerfect();
            }
        }

        /// <summary>
        /// 设置牌型颜色
        /// </summary>
        /// <param name="col"></param>
        public void SetColor(Color col)
        {
            var wis = gameObject.GetComponentsInChildren<UIWidget>(true);
            foreach (UIWidget wi in wis)
            {
                wi.color = col;
            }
        }

        bool _isjumping;

        public float JumpSpeed = 256;
        public void PokerJump()
        {
            if (!_isjumping)
            {
                StartCoroutine(DoJump());
                _isjumping = true;
            }

        }

        IEnumerator DoJump()
        {
            Vector3 selfPos = transform.localPosition;
            float posY = selfPos.y;
            float target = selfPos.y + 30f;
            while (posY < target)
            {
                posY += Time.deltaTime * JumpSpeed;
                if (posY >= target)
                    posY = target;
                transform.localPosition = new Vector3(selfPos.x, posY, selfPos.z);
                yield return null;
            }

            while (posY > selfPos.y)
            {
                posY -= Time.deltaTime * JumpSpeed;
                if (posY <= selfPos.y)
                    posY = selfPos.y;
                transform.localPosition = new Vector3(selfPos.x, posY, selfPos.z);
                yield return null;
            }

            StopJump();
        }

        public void StopJump()
        {
            StopAllCoroutines();
            _isjumping = false;
        }

        protected void OnDestroy()
        {
            StopAllCoroutines();
        }


        /// <summary>
        /// 移动牌
        /// </summary>
        /// <param name="from">起始位置</param>
        /// <param name="to">目标位置</param>
        /// <param name="duration">移动时间</param>
        /// <param name="delay">延迟时间</param>
        public void MoveTo(Vector3 from, Vector3 to, float duration = 0.2f, float delay = 0)
        {
            SetDepth(1024);

            BoxCollider bc = GetComponent<BoxCollider>();
            if (bc != null)
                bc.enabled = false;

            TweenPosition tp = GetComponent<TweenPosition>();
            tp.from = from;
            tp.to = to;
            tp.duration = duration;
            tp.delay = delay;

            tp.ResetToBeginning();
            tp.PlayForward();
        }

        public void MoveToFinish()
        {
            SetDepth(_depth);

            BoxCollider bc = GetComponent<BoxCollider>();
            if (bc != null)
                bc.enabled = true;
        }

        /// <summary>
        /// 是否被选中
        /// </summary>
        /// <param name="isSelected">是否被选中</param>
        public void SetCardSelected(bool isSelected)
        {
            if (Mask == null) return;
            Mask.gameObject.SetActive(isSelected);
        }

        public void StopAllTween()
        {
            TweenAlpha ta = GetComponent<TweenAlpha>();
            if (ta != null)
            {
                ta.tweenFactor = 1;
            }

            TweenPosition tp = GetComponent<TweenPosition>();
            if (tp != null)
            {
                tp.tweenFactor = 1;
                tp.enabled = false;
            }

            TweenRotation tr = GetComponent<TweenRotation>();
            if (tr != null)
            {
                tr.tweenFactor = 1;
                tr.enabled = false;
            }

            TweenScale ts = GetComponent<TweenScale>();
            if (ts != null)
            {
                ts.tweenFactor = 1;
                ts.enabled = false;
            }
        }



    }
}
