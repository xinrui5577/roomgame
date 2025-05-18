using Assets.Scripts.Game.sanpian.DataStore;
using Assets.Scripts.Game.sanpian.server;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sanpian.item
{
    public class CardItem : MonoBehaviour
    {
        public UISprite Face;
        public UISprite SelectShade;
        public PoKerCard Card;


        protected int MValue;
        /// <summary>
        /// 是否选中了
        /// </summary>
        private bool _status;
        /// <summary>
        /// 显示牌值
        /// </summary>
        public UISprite[] Num;
        /// <summary>
        /// 小花色显示
        /// </summary>
        public UISprite[] SmallColor;
        /// <summary>
        /// 大花色显示
        /// </summary>
        public UISprite BigColor;
        
        /// <summary>
        /// 头标记
        /// </summary>
        [HideInInspector]
        public bool  IsHead=false;


        /// <summary>
        /// 阴影标记
        /// </summary>
        [HideInInspector]
        public bool IsShade = false;

        public UILabel CardsLenNum;
        

       
        public string BackKey = "back";

        /// <summary>
        /// 是否是我的手牌标记
        /// </summary>
        public bool MyCardFlag = false;

        private float SelfDefaultY = 0;


       

        public void SetPos()
        {
            SelfDefaultY = transform.localPosition.y;
        }

        public void SetTouchBoxPos()
        {
            BoxCollider bc = transform.GetComponent<BoxCollider>();
            Vector3 v3 = new Vector3(-1.4f, 61, 0);
            bc.center = v3;
            v3 = new Vector3(136.7f, 192.45f, 1);
            bc.size = v3;
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
                    foreach (var sprite in Num)
                    {
                        sprite.gameObject.SetActive(false);
                    }
                    foreach (var sprite in SmallColor)
                    {
                        sprite.gameObject.SetActive(false);
                    }
                    BigColor.gameObject.SetActive(false);
                }
                else
                {
                    foreach (var sprite in Num)
                    {
                        sprite.gameObject.SetActive(true);
                    }
                    foreach (var sprite in SmallColor)
                    {
                        sprite.gameObject.SetActive(true);
                    }
                    BigColor.gameObject.SetActive(true);
                    foreach (var sprite in Num)
                    {
                        sprite.spriteName = Card.GetCardShowNumStr();
                        sprite.MakePixelPerfect();
                    }
                    foreach (var sprite in SmallColor)
                    {
                        sprite.spriteName = "s_" + Card.Colour + "_0";
                        sprite.MakePixelPerfect();
                    }
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
            foreach (var sprite in Num)
            {
                sprite.gameObject.SetActive(false);
            }
            foreach (var sprite in SmallColor)
            {
                sprite.gameObject.SetActive(false);
            }
            BigColor.gameObject.SetActive(false);
        }

        public void SetCardDepth(int depth)
        {
            depth = depth*4;
            gameObject.SetActive(false);
            Face.depth = depth;
            foreach (var sprite in Num)
            {
                sprite.depth = depth + 1;
            }
            foreach (var sprite in SmallColor)
            {
                sprite.depth = depth + 1;
            }
            CardsLenNum.depth =depth+ 1;
            BigColor.depth = depth + 1;
            SelectShade.depth = depth + 2;
            gameObject.SetActive(true);
        }

        public void SetCardColor(Color color)
        {
            Face.color = color;
        }


        public virtual void DownCard()
        {
            Vector3 v = transform.localPosition;
            _status = false;
            v.y = SelfDefaultY;
            transform.localPosition = v;

        }

        public virtual void UpCard()
        {
            Vector3 v = transform.localPosition;
            _status = true;
            v.y = SelfDefaultY + Constants.CardOffsetY;
            transform.localPosition = v;
        }

        public void ShowShade()
        {
            SelectShade.gameObject.SetActive(true);
            IsShade = true;
        }

        public void HideShade()
        {
            SelectShade.gameObject.SetActive(false);
            IsShade = false;
        }

        public virtual int GetCardValue()
        {
            return Card.Value;
        }

        public virtual int GetCardColor()
        {
            return Card.Colour;
        }

        public virtual bool IsStatus()
        {
            return _status;
        }

        public void OnUpdatePos()
        {
            if (!_status)
            {
                UpCard();
            }
            else
            {
                DownCard();
            }
        }

       

        public void OnPressCard(GameObject gameObj)
        {
            if (!MyCardFlag)
            {
              return;  
            }
            IsHead = true;
            App.GetGameManager<SanPianGameManager>().RealPlayer.ChooseHead();
        }

        public void OnReleaseCard()
        {
            if (!MyCardFlag)
            {
                return;
            }
            App.GetGameManager<SanPianGameManager>().RealPlayer.HideAllShade(-1);
            _status = !_status;
            OnUpdatePos();
        }

        public void OnDrag(Vector2 v)
        {
            if (!MyCardFlag)
            {
                return;
            }
            App.GetGameManager<SanPianGameManager>().RealPlayer.DragCards(v.x);
        }

        public void OnDragEnd()
        {
            if (!MyCardFlag)
            {
                return;
            }
            App.GetGameManager<SanPianGameManager>().RealPlayer.HideAllShade(-1);
        }

        public void SetLen(int num)
        {
            CardsLenNum.gameObject.SetActive(true);
            CardsLenNum.text = num + "";
        }


    }
}
