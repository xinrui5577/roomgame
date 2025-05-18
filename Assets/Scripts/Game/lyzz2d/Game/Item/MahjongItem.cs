/** 
 *文件名称:     MahjongItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-02 
 *描述:         麻将牌类,用于同步麻将数据，控制显示单独的麻将牌在牌桌上的显示。
 *历史记录:     
*/

using System;
using Assets.Scripts.Game.lyzz2d.Game.Data;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Pools;
using Assets.Scripts.Game.lyzz2d.Utils.UI;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Game.Item
{
    public class MahjongItem : MonoBehaviour, IPoolItem
    {
        /// <summary>
        ///     背景图大小
        /// </summary>
        private Vector2 _bgSize;

        [SerializeField] private UISprite _bgSprite;

        [SerializeField] private MahJongData _data;

        private readonly int _defBgLayer = 1;
        private readonly int _defHunValue = 3;
        private readonly int _defHuValue = 4;
        private readonly int _defValueLayer = 2;

        [SerializeField] private UISprite _hunTag;

        [SerializeField] private UISprite _huTag;

        /// <summary>
        ///     标记位置
        /// </summary>
        private Vector2 _tagPos;

        private Transform _transform;

        /// <summary>
        ///     麻将值位置
        /// </summary>
        private Vector2 _valuePos;

        /// <summary>
        ///     值大小
        /// </summary>
        private Vector2 _valueSize;

        [SerializeField] private UISprite _valueSprite;

        private string bgSpriteName;
        private MahjongPile parentPile;
        private string valueSpriteName;

        public MahJongData SelfData
        {
            get
            {
                if (_data == null)
                {
                    _data = new MahJongData(0, CurrentPile.ItemAction, CurrentPile.ItemDirection);
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
            set { SelfData.Value = (EnumMahjongValue) value; }
            get { return (int) SelfData.Value; }
        }

        public float Width
        {
            get { return _bgSprite.width; }
        }

        public float Height
        {
            get { return _bgSprite.height; }
        }

        public UISprite ValueSprite
        {
            get { return _valueSprite; }
        }

        public UISprite BGSprite
        {
            get { return _bgSprite; }
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


        public virtual void Reset()
        {
            _hunTag.gameObject.SetActive(false);
            _huTag.gameObject.SetActive(false);
        }

        [ExecuteInEditMode]
        private void Awake()
        {
            SelfData.OnValueChange = OnChangeValue;
            SelfData.OnActionChange = OnChangeAction;
            SelfData.OnDirectonChange = OnChangeDirection;
            SelfData.OnShowDirectionChange = OnShowDirectionChange;
            SelfData.OnLayerChange = OnLayerChange;
        }

        /// <summary>
        ///     /这块没太想好怎么处理，查空通用处理
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
            if (_huTag == null)
            {
                _huTag = _valueSprite.transform.GetChild(1).GetComponent<UISprite>();
            }
        }

        /// <summary>
        ///     刷新数据：默认处理方向，动作，值。值的优先级最高
        /// </summary>
        public void OnDataChange()
        {
            JudgeNull();
            var hasVlaue = false;
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
                _valueSprite.spriteName = ((int) _data.Value).ToString();
            }

            #region 显示背景图片

            bgSpriteName = string.Format("{0}_{1}", _data.Direction, _data.Action);
            switch (_data.ShowDirection)
            {
                case EnumShowDirection.Left:
                case EnumShowDirection.Right:
                    if (_data.Action.Equals(EnumMahJongAction.StandNo))
                    {
                        bgSpriteName += string.Format("_{0}", _data.ShowDirection);
                    }
                    break;
            }
            _bgSprite.spriteName = bgSpriteName;

            #endregion

            ChangeItemSizeAndPosition(bgSpriteName);
            OnLayerChange();
            _bgSprite.MarkAsChanged();
            _valueSprite.MarkAsChanged();
        }

        /// <summary>
        ///     好吧，这块弄的比较恶心，这个是处理麻将的大小和其中标记的位置的，不太好弄，现在只能写死了。看以后是否能有好的方法进行优化
        /// </summary>
        /// <param name="bgName"></param>
        private void ChangeItemSizeAndPosition(string bgName)
        {
            _valueSprite.transform.localEulerAngles = new Vector3(0, 0, (int) _data.ShowDirection*90);
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
            _bgSprite.width = (int) _bgSize.x;
            _bgSprite.height = (int) _bgSize.y;
            _valueSprite.width = (int) _valueSize.x;
            _valueSprite.height = (int) _valueSize.y;
            _valueSprite.transform.localPosition = _valuePos;
        }

        private void Horizontal_Action()
        {
            _valuePos =
                new Vector2(
                    (_data.ShowDirection.Equals(EnumShowDirection.Right) ? 1 : -1)*ConstantData.Value_Horizontal_LiePosX,
                    ConstantData.Value_Horizontal_LiePosY);
            switch (_data.Action)
            {
                case EnumMahJongAction.Fly:
                    _bgSize = new Vector2(ConstantData.Bg_Horizontal_Fly_SizeX, ConstantData.Bg_Horizontal_Fly_SizeY);
                    break;
                case EnumMahJongAction.StandNo:
                    _bgSize = new Vector2(ConstantData.Bg_Horizontal_Stand_SizeX, ConstantData.Bg_Horizontal_Stand_SizeY);
                    break;
                default:
                    _bgSize = new Vector2(ConstantData.Bg_Horizontal_Lie_SizeX, ConstantData.Bg_Horizontal_Lie_SizeY);
                    break;
            }
            _valueSize = new Vector2(ConstantData.Value_Horizontal_Lie_SizeX, ConstantData.Value_Horizontal_Lie_SizeY);
            _hunTag.transform.localPosition = new Vector3(ConstantData.Tag_Horizontal_PosX,
                ConstantData.Tag_Horizontal_PosY);
            _huTag.transform.localPosition = new Vector3(ConstantData.Tag_Horizontal_PosX,
                ConstantData.Tag_Horizontal_PosY);
        }

        private void Vertical_Action()
        {
            switch (_data.Action)
            {
                case EnumMahJongAction.Fly:
                    _bgSize = new Vector2(ConstantData.Bg_Vertical_Fly_SizeX, ConstantData.Bg_Vertical_Fly_SizeY);
                    break;
                case EnumMahJongAction.StandNo:
                case EnumMahJongAction.Push:
                    _bgSize = new Vector2(ConstantData.Bg_Normal_SizeX, ConstantData.Bg_Normal_SizeY);
                    break;
                case EnumMahJongAction.StandWith:
                    if (_data.NeedOppsetValue)
                    {
                        _valuePos = new Vector2(ConstantData.Value_Oppset_Stand_PosX,
                            ConstantData.Value_Oppser_Stand_PosY);
                    }
                    else
                    {
                        _valuePos = new Vector2(ConstantData.Value_Vertical_Stand_PosX,
                            ConstantData.Value_Vertical_Stand_PosY);
                    }
                    _bgSize = new Vector2(ConstantData.Bg_Normal_SizeX, ConstantData.Bg_Normal_SizeY);
                    break;
                case EnumMahJongAction.Lie:
                    if (_data.NeedOppsetValue)
                    {
                        _valuePos = new Vector2(ConstantData.Value_Oppset_Lie_PosX, ConstantData.Value_Oppser_lie_PosY);
                    }
                    else
                    {
                        _valuePos = new Vector2(ConstantData.Value_Vertical_Lie_PosX,
                            ConstantData.Value_Vertical_Lie_PosY);
                    }

                    _bgSize = new Vector2(ConstantData.Bg_Normal_SizeX, ConstantData.Bg_Normal_SizeY);
                    break;
            }
            _valueSize = new Vector2(ConstantData.Bg_Normal_SizeX, ConstantData.Bg_Normal_SizeY);
            if (!_data.NeedOppsetValue)
            {
                _valueSprite.transform.localEulerAngles = Vector3.zero;
            }
            _hunTag.transform.localPosition = new Vector3(ConstantData.Tag_Vertical_PosX, ConstantData.Tag_Vertical_PosY);
            _huTag.transform.localPosition = new Vector3(ConstantData.Tag_Vertical_PosX, ConstantData.Tag_Vertical_PosY);
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

        public virtual void JudgeHunTag(int hunNum)
        {
            if (_data.Value <= 0 || hunNum <= 0)
            {
                return;
            }
            _hunTag.gameObject.SetActive(hunNum.Equals(Value));
        }

        public virtual void SetHuTag(bool state = true)
        {
            _huTag.gameObject.SetActive(state);
        }

        public void MoveTo(Vector3 target, float time, Action MoveEnd)
        {
        }

        #region Color

        public void SetColor(Color color)
        {
            _bgSprite.color = color;
            _valueSprite.color = color;
        }

        public void ResetColorFlag()
        {
            SetColor(Color.white);
        }

        #endregion
    }
}