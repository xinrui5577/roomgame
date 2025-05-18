/** 
 *文件名称:     MahjongItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-02 
 *描述:         麻将牌类,用于同步麻将数据，控制显示单独的麻将牌在牌桌上的显示。
 *历史记录:     
*/

using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.Item
{
    public class MahjongItem : MonoBehaviour
    {
        [SerializeField]
        private UISprite _bgSprite;
        [SerializeField]
        private UISprite _valueSprite;
        [SerializeField]
        private UISprite _huTag;
        [SerializeField]
        private UISprite _hunTag;
        private int _defBgLayer = 1;
        private int _defValueLayer = 2;
        private int _defHunValue = 3;
        private int _defHuValue = 4;
        [SerializeField]
        private MahJongData _data;
        private Transform _transform;
        private string bgSpriteName;
        private string valueSpriteName;
        private MahjongPile parentPile;
        public static EnumMahJongColorType EnumMahJongColorType = EnumMahJongColorType.G;
        public static EnumMahJongValueType EnumMahjongValueType= EnumMahJongValueType.A;
        private static List<MahjongItem> _list=new List<MahjongItem>();
        [SerializeField]
        public MahJongData SelfData
        {
            get
            {
                if (_data == null)
                {
                    _data = new MahJongData(0, CurrentPile.ItemAction, CurrentPile.ItemDirection, CurrentPile.ItemShow);
                }
                return _data;
            }
            set
            {
                if (_data.Equals(value))
                {
                    return;
                }
                _data = value;
                OnDataChange();
            }
        }

        public int Value
        {
            set
            {
                SelfData.Value = (EnumMahjongValue)value;
            }
            get { return (int)SelfData.Value; }
        }

        public float Width
        {
            get
            {
               return _bgSprite.width;
            }
        }

        public float Height
        {
            get { return _bgSprite.height; }
        }

        public UISprite ValueSprite
        {
            get
            {
                return _valueSprite;
            }
        }

        public UISprite BGSprite
        {
            get { return _bgSprite; }
        }

        public UISprite HuiTagSprite
        {
            get
            {
                return _hunTag;
            }
        }

        public UISprite HuTagSprite
        {
            get
            {
                return _huTag;
            }
        }

        [ExecuteInEditMode]
        void Awake()
        {
            _list.Add(this);
            SelfData.OnValueChange = OnChangeValue;
            SelfData.OnActionChange = OnChangeAction;
            SelfData.OnDirectonChange = OnChangeDirection;
            SelfData.OnShowDirectionChange = OnShowDirectionChange;
            SelfData.OnLayerChange = OnLayerChange;
            SelfData.OnLockStateChange = OnDataChange;
            OnDataChange();
        }

        void OnDestroy()
        {
            _list.Remove(this);
        }
        public MahjongPile CurrentPile
        {
            get
            {
                if (parentPile == null)
                {
                    parentPile = transform.parent.GetComponent<MahjongPile>();
                }
                return parentPile;
            }
            set
            {
                parentPile.RemoveItem(this);
                parentPile = value;
            }
        }
        /// <summary>
        /// /这块没太想好怎么处理，查空通用处理
        /// </summary>
        private void JudgeNull()
        {
            if (_bgSprite == null)     
            {
                _bgSprite = transform.GetChild(0).GetComponent<UISprite>();
            }
            if (_valueSprite == null)
            {
                _valueSprite = transform.GetChild(1).GetComponent<UISprite>();
            }
            if (_hunTag == null)
            {
                _hunTag = _valueSprite.transform.GetChild(0).GetComponent<UISprite>();
            }
            if (_huTag==null)
            {
                _huTag = _valueSprite.transform.GetChild(1).GetComponent<UISprite>();
            }
        }

        /// <summary>
        /// 刷新数据：默认处理方向，动作，值。值的优先级最高
        /// </summary>
        public void OnDataChange()
        {
            JudgeNull();
            bool hasVlaue = false;
            switch (_data.Action)
            {
                case EnumMahJongAction.Lie:
                case EnumMahJongAction.StandWith:
                    hasVlaue = true;
                    break;
            }
            _valueSprite.gameObject.SetActive(_data.Value > 0 && hasVlaue);
            if (_valueSprite.gameObject.activeInHierarchy)
            {
                if(!_data.LockValueType)
                {
                    _valueSprite.spriteName = string.Format("{0}_{1}", EnumMahjongValueType, Value);
                }
            }
            #region 显示背景图片
            bgSpriteName = string.Format("{0}_{1}", _data.Direction, _data.Action);
            switch (_data.ShowDirection)
            {
                case EnumShowDirection.Left:
                case EnumShowDirection.Right:
                    if (_data.Action.Equals(EnumMahJongAction.StandNo)|| _data.Action.Equals(EnumMahJongAction.Push))
                    {
                        bgSpriteName += string.Format("_{0}",_data.ShowDirection);
                    }                   
                    break;
            }
            bgSpriteName += string.Format("_{0}", EnumMahJongColorType);
            _bgSprite.spriteName = bgSpriteName;
            #endregion
            ChangeItemSizeAndPosition(bgSpriteName);
            OnLayerChange();
            _bgSprite.MarkAsChanged();
            _valueSprite.MarkAsChanged();
        }
        /// <summary>
        /// 背景图大小
        /// </summary>
        private Vector2 _bgSize;
        /// <summary>
        /// 麻将值位置
        /// </summary>
        private Vector2 _valuePos ;
        /// <summary>
        /// 标记位置
        /// </summary>
        private Vector2 _tagPos;
        /// <summary>
        /// 值大小
        /// </summary>
        private Vector2 _valueSize;
        /// <summary>
        /// 好吧，这块弄的比较恶心，这个是处理麻将的大小和其中标记的位置的，不太好弄，现在只能写死了。看以后是否能有好的方法进行优化
        /// </summary>
        /// <param name="bgName"></param>
        private void ChangeItemSizeAndPosition(string bgName)
        {
            _valueSprite.transform.localEulerAngles = new Vector3(0, 0, (int)_data.ShowDirection * 90);
            switch (_data.ShowDirection)
            {
                case EnumShowDirection.Left:
                    _data.NeedOppsetValue = false;  
                    Horizontal_Action(); 
                    break;             
                case EnumShowDirection.Right:                 
                    Horizontal_Action();
                    _data.NeedOppsetValue = false;
                    break;
                case EnumShowDirection.Oppset:
                    Vertical_Action();
                    break;
                case EnumShowDirection.Self:
                    _data.NeedOppsetValue = false;
                    Vertical_Action();
                    break;
            }
            _bgSprite.width = (int)_bgSize.x;
            _bgSprite.height = (int)_bgSize.y;  
            _valueSprite.width= (int)_valueSize.x;
            _valueSprite.height = (int)_valueSize.y;
            _valueSprite.transform.localPosition = _valuePos;
        }

        void Horizontal_Action()
        {
            _valuePos = new Vector2((_data.ShowDirection.Equals(EnumShowDirection.Right) ? 1 : -1) * ConstantData.ValueHorizontalLiePosX, ConstantData.ValueHorizontalLiePosY);
            switch (_data.Action)
            {
                case EnumMahJongAction.Fly:
                    _bgSize = new Vector2(ConstantData.BgHorizontalFlySizeX, ConstantData.BgHorizontalFlySizeY);
                    break;
                case EnumMahJongAction.StandNo:
                    _bgSize = new Vector2(ConstantData.BgHorizontalStandSizeX, ConstantData.BgHorizontalStandSizeY);
                    break;
                default:
                    _bgSize = new Vector2(ConstantData.BgHorizontalLieSizeX, ConstantData.BgHorizontalLieSizeY);
                    break;
            }
            _valueSize = new Vector2(ConstantData.ValueHorizontalLieSizeX, ConstantData.ValueHorizontalLieSizeY);
            _hunTag.transform.localPosition = new Vector3(ConstantData.TagHorizontalPosX, ConstantData.TagHorizontalPosY);
            _huTag.transform.localPosition = new Vector3(ConstantData.TagHorizontalPosX, ConstantData.TagHorizontalPosY);
        }

        void Vertical_Action()
        {  
            switch (_data.Action)
            {
                case EnumMahJongAction.Fly:
                    _bgSize = new Vector2(ConstantData.BgVerticalFlySizeX, ConstantData.BgVerticalFlySizeY);
                    break;
                case EnumMahJongAction.StandNo:
                case EnumMahJongAction.Push:
                    _bgSize = new Vector2(ConstantData.BgNormalSizeX, ConstantData.BgNormalSizeY);
                    break;
                case EnumMahJongAction.StandWith:
                    if (_data.NeedOppsetValue)
                    {
                        _valuePos = new Vector2(ConstantData.ValueOppsetStandPosX, ConstantData.ValueOppserStandPosY);
                       
                    }
                    else
                    {
                        _valuePos = new Vector2(ConstantData.ValueVerticalStandPosX, ConstantData.ValueVerticalStandPosY);
                    }
                    _bgSize = new Vector2(ConstantData.BgNormalSizeX, ConstantData.BgNormalSizeY);
                    break;
                case EnumMahJongAction.Lie:
                    if (_data.NeedOppsetValue)
                    {
                        _valuePos = new Vector2(ConstantData.ValueOppsetLiePosX, ConstantData.ValueOppserLiePosY);
                    }
                    else
                    {
                        _valuePos = new Vector2(ConstantData.ValueVerticalLiePosX, ConstantData.ValueVerticalLiePosY);
                    }       
                             
                    _bgSize = new Vector2(ConstantData.BgNormalSizeX, ConstantData.BgNormalSizeY);
                    break;
            }
            _valueSize =new Vector2(ConstantData.BgNormalSizeX, ConstantData.BgNormalSizeY);
            if (!_data.NeedOppsetValue)
            {
                _valueSprite.transform.localEulerAngles = Vector3.zero;
            }
            _hunTag.transform.localPosition = new Vector3(ConstantData.TagVerticalPosX, ConstantData.TagVerticalPosY);
            _huTag.transform.localPosition = new Vector3(ConstantData.TagVerticalPosX, ConstantData.TagVerticalPosY);
        }

        public virtual void OnChangeValue()
        {
            OnDataChange();
        }

        public virtual void OnChangeAction()
        {
            OnDataChange();
        }

        public virtual void OnChangeDirection()
        {
            OnDataChange();
        }

        public virtual void OnShowDirectionChange()
        {
            OnDataChange();
        }

        public virtual void OnLayerChange()
        {
            _bgSprite.depth = _defBgLayer + _data.MahjongLayer;
            _valueSprite.depth = _defValueLayer + _data.MahjongLayer;
            _hunTag.depth = _defHunValue + _data.MahjongLayer;
            _huTag.depth = _defHuValue + _data.MahjongLayer;
        }


        public virtual void Reset()
        {
            _hunTag.gameObject.SetActive(false);
            _huTag.gameObject.SetActive(false);
        }

        public virtual void JudgeHunTag(int hunNum)
        {
            if(_data.Value<=0||hunNum<=0)
            {
                return;
            }
            _hunTag.gameObject.SetActive(hunNum.Equals(Value));
        }

        public virtual void SetHuTag(bool state=true)
        {
            _huTag.gameObject.SetActive(state);
        }
        #region Color

        public void SetColor(Color color)
        {
            _bgSprite.color = color;
            _valueSprite.color = color;
        }

        public static void OnChangeBgType(EnumMahJongColorType type)
        {
            EnumMahJongColorType = type;
            for (int i = 0, lenth = _list.Count; i < lenth; i++)
            {
                _list[i].OnDataChange();
            }
        }
        public static void OnChangeValueType(EnumMahJongValueType type)
        {
            EnumMahjongValueType = type;
            for (int i = 0,lenth= _list.Count; i <lenth; i++)
            {
                _list[i].OnDataChange();
            }
        }

        public void ResetColorFlag()
        {
            SetColor(Color.white);
        }
        #endregion
    }
}
