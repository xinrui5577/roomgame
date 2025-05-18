using UnityEngine;

namespace Assets.Scripts.Game.Texas
{
    public class PokerCard : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            _changeFEvent = new EventDelegate(Onfinish);
            _changeOEvent = new EventDelegate(Onfinish1);
        }
       
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
        public UISprite Line;
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
        /// 是否被选中
        /// </summary>
        public bool IsSelect;

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
            rightTweenR.AddOnFinished(_changeOEvent);
            rightTweenR.ResetToBeginning();
            rightTweenR.PlayForward();
        }
        private void Onfinish1()
        {
            TweenRotation rightTweenR = GetComponent<TweenRotation>();
            rightTweenR.RemoveOnFinished(_changeOEvent);
        }

        public void TurnCard()
        {
            transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            TweenRotation rightTweenR = GetComponent<TweenRotation>();

            rightTweenR.from = new Vector3(0, 0, 0);
            rightTweenR.to = new Vector3(0, -90, 0);
            rightTweenR.AddOnFinished(_changeFEvent);
            rightTweenR.ResetToBeginning();
            rightTweenR.PlayForward();
        }

        public PokerCard()
        {

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
            string result = Id == 0 ? "back":"front";
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
            result += (Value > 10 && Value < 14) ? Value.ToString("x"):"0";
            return result;
        }

        public void SetCardDepth(int depth)
        {
            Sprite.depth = depth;
            Num.depth = depth + 1;
            SmallColor.depth = depth + 1;
            BigColor.depth = depth + 1;
            Line.depth = depth + 1;
        }

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
        /// 被选中的
        /// </summary>
        public void Selected()
        {
            IsSelect = true;
            Line.gameObject.SetActive(true);
            Sprite.transform.localPosition = new Vector3(0f,20f,0f);
            SetColor(Color.white);
            Line.color = new Color(1f, 0.6f, 0f);
        }
        /// <summary>
        /// 初始化选中
        /// </summary>
        public void InitSelect()
        {
            IsSelect = false;
            Line.gameObject.SetActive(false);
            Sprite.transform.localPosition = Vector3.zero;
            SetColor(Color.white);
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

    }
}
