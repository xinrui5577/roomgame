using System;
using UnityEngine;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameCardItem : MonoBehaviour
    {
        public PoKerCard Card;
        public UISprite Face;

        public UISprite SelectShade;
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
        /// 手牌的张数
        /// </summary>
        public UILabel CardsLenNum;

        /// <summary>
        /// 是否是我的手牌标记
        /// </summary>
        public bool MyCardFlag = false;

        public bool IsCardSelect = false;

        public bool IsFoldCard = false;

        public bool IsOutCard = false;

        protected int MValue;
        public string BackKey = "cardBack";

        public void SetCardDepth(int depth)
        {
            depth = depth * 4;
            gameObject.SetActive(false);
            Face.depth = depth;
            Num.depth = depth + 1;

            SmallColor.depth = depth + 1;
            BigColor.depth = depth + 1;
            SelectShade.depth = depth + 2;
            CardsLenNum.depth = depth + 2;
            gameObject.SetActive(true);
        }

        public void SetLen(int num)
        {
            if (MyCardFlag) return;
            CardsLenNum.gameObject.SetActive(true);
            CardsLenNum.text = num + "";
        }

        public void SetCardUp(bool up=true)
        {
            transform.localPosition = new Vector3(transform.localPosition.x,up? 35: 0, transform.localPosition.z);
            IsCardSelect = up;
        }

        public void SetCardDeath()
        {
            IsFoldCard = false;
            IsOutCard = false;
            SelectShade.gameObject.SetActive(true);
            transform.GetComponent<BoxCollider>().enabled = true;
        }

        public void SetCardActive(bool enable)
        {
            IsOutCard = true;
            SelectShade.gameObject.SetActive(false);
            transform.GetComponent<BoxCollider>().enabled = enable;
        }

        public void NoSpeaker()
        {
            transform.GetComponent<BoxCollider>().enabled = false;
        }

        public void Reset()
        {
            IsCardSelect = false;
            SetCardDeath();
            if (IsCardSelect)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 35, transform.localPosition.z);
            }

        }
        /// <summary>
        /// 扑克名字
        /// </summary>
        public int Value
        {
            get { return MValue; }
            set
            {
                MValue = value;
                ChangeFace();
            }
        }


        protected virtual void ChangeFace()
        {
            CardsLenNum.gameObject.SetActive(false);
            if (MValue > 0)
            {
                if (Card == null)
                {
                    Card = new PoKerCard();
                }
                Card.SetCardId(MValue);

                if (Face != null)
                {
                    Face.spriteName = Card.GetCardValueStr();
                    Face.MakePixelPerfect();
                }

                if (MValue == 81 || MValue == 97 || MValue == 113)
                {
                    BigColor.gameObject.SetActive(false);
                }
                else
                {
                    Num.gameObject.SetActive(true);
                    Num.spriteName = Card.GetCardShowNumStr();
                    Num.MakePixelPerfect();

                    SmallColor.gameObject.SetActive(true);
                    SmallColor.spriteName = "s_" + Card.Colour + "_0";
                    SmallColor.MakePixelPerfect();

                    BigColor.gameObject.SetActive(true);
                    BigColor.spriteName = Card.GetBigColorStr();
                    BigColor.MakePixelPerfect();
                }
            }
            else
            {
                if (Face != null)
                {
                    SetBackKey();
                    Face.MakePixelPerfect();
                }
            }
        }
        public void SetBackKey()
        {
            Face.spriteName = BackKey;
            BigColor.gameObject.SetActive(false);
        }
    }
    [Serializable]
    public class PoKerCard
    {
        /// <summary>
        /// 花色
        /// </summary>
        public int Colour;
        /// <summary>
        /// 面值
        /// </summary>
        public int Value;
        /// <summary>
        /// 传输值
        /// </summary>
        public int Id;

        public PoKerCard()
        {
        }

        public PoKerCard(int id)
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
            string result = Id == 0 ? "cardBack" : "front";
            result = (Id == 81 || Id == 97 || Id == 113) ? "0x" + Id.ToString("x") : result;
            return result;
        }

        public string GetCardShowNumStr()
        {
            var value = Value >= 10 ? Value.ToString() : Value.ToString("x");
            string result = (Colour == 1 || Colour == 2) ? "red_" + value : "black_" + value;
            return result;
        }

        public string GetBigColorStr()
        {
            string result = "b_" + Colour + "_";
            result += (Value > 10 && Value < 14) ? Value.ToString("x") : "0";
            return result;
        }
    }
}
