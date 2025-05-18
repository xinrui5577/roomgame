using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.fillpit
{
    public class PokerCard : MonoBehaviour
    {
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            _changeFEvent = new EventDelegate(Onfinish);
            _changeOEvent = new EventDelegate(Onfinish1);
        }

        public void OnMoveFinish()
        {
            transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
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
        /// 公共牌标记
        /// </summary>
        public UISprite PublicMark;


        /*************翻转牌**************/
        /// <summary>
        /// 翻牌前半段
        /// </summary>
        private EventDelegate _changeFEvent;

        /// <summary>
        /// 翻牌后半段
        /// </summary>
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
            if (Id == 0) return;

            TweenRotation rightTweenR = GetComponent<TweenRotation>();
            rightTweenR.from = new Vector3(0, 0, 0);
            rightTweenR.to = new Vector3(0, -90, 0);
            rightTweenR.AddOnFinished(_changeFEvent);
            rightTweenR.ResetToBeginning();
            rightTweenR.PlayForward();
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

        protected string GetCardValueStr()
        {
            string result = Id == 0 ? "back":"front";
            result = (Id == 81 || Id == 97 || Id == 113) ? "0x" + Id.ToString("x") : result;
            return result;
        }

        protected string GetCardShowNumStr()
        {
            return string.Format("{0}{1}", GetColorStr(), Value.ToString("x"));
        }

        protected string GetColorStr()
        {
            return (Colour == 1 || Colour == 2) ? "red_" : "black_";
        }

        /// <summary>
        /// 大花色
        /// </summary>
        /// <returns></returns>
        protected string GetBigColorStr()
        {
            string result = "b_" + Colour + "_";
            result += (Value > 10 && Value < 14) ? Value.ToString("x"):"0";
            return result;
        }

        /// <summary>
        /// 设置扑克层级
        /// </summary>
        /// <param name="depth"></param>
        public void SetCardDepth(int depth)
        {
            Sprite.depth = depth;
            Num.depth = depth + 1;
            SmallColor.depth = depth + 1;
            BigColor.depth = depth + 1;
            Line.depth = depth + 1;
            PublicMark.depth = depth + 1;
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
                SmallColor.spriteName = App.GetGameData<FillpitGameData>().LaiziValue == Id
                    ? string.Format("{0}magic",GetColorStr())
                    : string.Format("s_{0}_0", Colour); 
                SmallColor.MakePixelPerfect();
                BigColor.spriteName = GetBigColorStr();
                BigColor.MakePixelPerfect();
            }
        }


        /// <summary>
        /// 设置牌的公共标记
        /// </summary>
        /// <param name="isPublicCard">是否是公共牌</param>
        public void SetPublicMarkActive(bool isPublicCard)
        {
            if(PublicMark != null)
                PublicMark.gameObject.SetActive(isPublicCard);
        }

        public void ShowPokerBack()
        {
            SetPublicMarkActive(false);
            BigColor.gameObject.SetActive(false);
            SmallColor.gameObject.SetActive(false);
            Line.gameObject.SetActive(false);
            Num.gameObject.SetActive(false);
            Sprite.spriteName = "back";
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
