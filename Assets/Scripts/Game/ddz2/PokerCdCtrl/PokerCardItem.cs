using Assets.Scripts.Game.ddz2.PokerRule;
using System.Globalization;
using UnityEngine;

namespace Assets.Scripts.Game.ddz2.PokerCdCtrl
{

    /// <summary>
    ///单张扑克控制
    /// </summary>
    public class PokerCardItem : MonoBehaviour
    {
      
        /// <summary>
        /// 手牌总控制脚本
        /// </summary>
        private HdCdsCtrl _hdCdsCtrlInstance;
        public void SetHdcdctrlInstance(HdCdsCtrl instance)
        {
            _hdCdsCtrlInstance = instance;
        }

        /// <summary>
        /// 这张手牌的牌值信息（带花色）
        /// </summary>
        public int CdValue { protected set; get; }

        /// <summary>
        /// 牌面
        /// </summary>
        [SerializeField]
        protected UISprite CdBg;
        /// <summary>
        /// 左上花色
        /// </summary>
        [SerializeField]
        protected UISprite ColorLeftUp;
        /// <summary>
        /// 右下花色
        /// </summary>
        [SerializeField]
        protected UISprite ColorRightDown;
        /// <summary>
        /// 左上牌值
        /// </summary>
        [SerializeField]
        protected UISprite CdValueSpUp;
        /// <summary>
        /// 右下牌值
        /// </summary>
        [SerializeField]
        protected UISprite CdValueSpDown;

        /// <summary>
        /// 设置这张扑克的层级
        /// </summary>
        /// <param name="layerIndex"></param>
        public void SetLayer(int layerIndex)
        {
            CdBg.GetComponent<UISprite>().depth = layerIndex;

            ColorLeftUp.GetComponent<UISprite>().depth = layerIndex + 1;
            ColorRightDown.GetComponent<UISprite>().depth = layerIndex + 1;
            CdValueSpUp.GetComponent<UISprite>().depth = layerIndex + 1;
            CdValueSpDown.GetComponent<UISprite>().depth = layerIndex + 1;
        }
        

        /// <summary>
        /// 设置这张扑克的牌值信息，显示效果
        /// </summary>
        /// <param name="cdValueData">牌值信息</param>
        public void SetCdValue(int cdValueData)
        {
            CdValue = cdValueData;
            SetCardFront();
        }

        protected void SetCardFront()
        {
            ColorLeftUp.gameObject.SetActive(false);
            ColorRightDown.gameObject.SetActive(false);
            CdValueSpUp.gameObject.SetActive(false);
            CdValueSpDown.gameObject.SetActive(false);
            switch (CdValue)
            {
                case HdCdsCtrl.SmallJoker:
                    {
                        CdBg.spriteName = HdCdsCtrl.SmallJoker.ToString(CultureInfo.InvariantCulture);
                        CdBg.MakePixelPerfect();
                        return;
                    }
                case HdCdsCtrl.BigJoker:
                    {
                        CdBg.spriteName = HdCdsCtrl.BigJoker.ToString(CultureInfo.InvariantCulture);
                        CdBg.MakePixelPerfect();
                        return;
                    }
                case HdCdsCtrl.MagicKing:
                    {
                        CdBg.spriteName = HdCdsCtrl.MagicKing.ToString(CultureInfo.InvariantCulture);
                        CdBg.MakePixelPerfect();
                        return;
                    }
            }

            CdBg.spriteName = "front";
            ColorLeftUp.gameObject.SetActive(true);
            ColorRightDown.gameObject.SetActive(true);
            CdValueSpUp.gameObject.SetActive(true);
            CdValueSpDown.gameObject.SetActive(true);


            var color = PokerRuleUtil.GetColor(CdValue);
            var value = HdCdsCtrl.GetValue(CdValue).ToString(CultureInfo.InvariantCulture);

            ColorLeftUp.spriteName = "s_" + color + "_0";
            ColorRightDown.spriteName = "s_" + color + "_0";
            if (color > 2)
            {
                CdValueSpUp.spriteName = "black_" + value;
                CdValueSpDown.spriteName = "black_" + value;
            }
            else
            {
                CdValueSpUp.spriteName = "red_" + value;
                CdValueSpDown.spriteName = "red_" + value;
            }

            CdBg.MakePixelPerfect();
            ColorLeftUp.MakePixelPerfect();
            ColorRightDown.MakePixelPerfect();
            CdValueSpUp.MakePixelPerfect();
            CdValueSpDown.MakePixelPerfect();
        }
      

        private bool _isCdUp;
        /// <summary>
        /// 控制并标记是否这张牌已经抬起
        /// </summary>
        public bool IsCdUp
        {
            set
            {
                var v3 = gameObject.transform.localPosition;
                //抬起高度
                if (value)
                {
                    gameObject.transform.localPosition = new Vector3(v3.x, 30f, v3.z);
                }
                else
                {
                    gameObject.transform.localPosition = new Vector3(v3.x, 0, v3.z);
                }
                _isCdUp = value;
            }

            get { return _isCdUp; }
        }

        #region  eventTrigger 的事件相应方法
        public void OnPress()
        {
            ChangeCdDark();
            if (_hdCdsCtrlInstance != null)
            {
                _hdCdsCtrlInstance.DragOverCdValue = HdCdsCtrl.NoneCdValue;
                _hdCdsCtrlInstance.PressCdValue = CdValue;
            }
        }

        public void OnRelease()
        {
            if (_hdCdsCtrlInstance != null)
            {
                _hdCdsCtrlInstance.RelseseCdValue = CdValue;
                _hdCdsCtrlInstance.OnDargOverCd(true);
            }
        }

        public void OnDragOver()
        {
            ChangeCdDark();
            if (_hdCdsCtrlInstance != null)
            {
                _hdCdsCtrlInstance.DragOverCdValue = CdValue;
                _hdCdsCtrlInstance.OnDargOverCd();
            }
        }
        #endregion

        /// <summary>
        /// 让扑克变暗色
        /// </summary>
        public void ChangeCdDark()
        {
            var color = new Color(0.5f, 0.5f, 0.5f);
            CdBg.color = color;
            ColorLeftUp.color = color;
            ColorRightDown.color = color;
            CdValueSpUp.color = color;
            CdValueSpDown.color = color;
        }

        /// <summary>
        /// 让扑克颜色还原
        /// </summary>
        public void ResetCdColor()
        {
            var color = new Color(1f, 1f, 1f);
            CdBg.color = color;
            ColorLeftUp.color = color;
            ColorRightDown.color = color;
            CdValueSpUp.color = color;
            CdValueSpDown.color = color;
        }
    }

}