using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    /// <summary>
    /// 癞子牌控制类，这个应该属于Common部分
    /// </summary>
    public class LaiZiControl : MonoSingleton<LaiZiControl>
    {
        [SerializeField]
        private GameObject _laiZiLabel;
        [SerializeField]
        private Transform _fromTrans;
        [SerializeField]
        private Transform _toTrans;

        private MahjongItem _item;

        public float ScaleX = 1;
   
        public float ScaleY = 1;

        [SerializeField]
        public float ShowLaiziWait = 1;

        [SerializeField]
        public float ShowStandWait = 1;
        [SerializeField]
        public float MoveWait = 1;

        [SerializeField]
        public float MoveTime=2;

        [SerializeField]
        private UISprite _fromBg;

        [SerializeField]
        private float _laiZiScale = 1.15f;

        [SerializeField]
        private int _laiZiLayer=105;

        private int _showValue;
        public int ShowValue
        {
            get { return ShowValue; }
        }

        public override void Awake()
        {
            base.Awake();
            if(_fromBg)
            _fromBg.gameObject.SetActive(false);
        
        }
        public void CreateMahjong(int value,bool immediate)
        {
            _item =App.GetGameManager<Mahjong2DGameManager>().GetNextMahjong().GetComponent<MahjongItem>();
            _item.transform.localScale = new Vector3(ScaleX, ScaleY);
            _item.SelfData.ShowDirection = EnumShowDirection.Self;
            _item.SelfData.Direction = EnumMahJongDirection.Vertical;
            _item.Value = value;
            _showValue = value;
            if (immediate)
            {
                _item.SelfData.Action = EnumMahJongAction.Lie;
                SetFanImmediate();
            }
            else
            {
                _laiZiLabel.TrySetComponentValue(false);
                if (_fromBg)
                _fromBg.gameObject.SetActive(true);
                _item.SelfData.Action = EnumMahJongAction.Push;
                GameTools.AddChild(_fromTrans, _item.transform);
                Invoke("ShowLaiZiValue", ShowLaiziWait);
            }
        }

        private void ShowLaiZiValue()
        {
            Debug.Log("显示癞子值");
            CancelInvoke("ShowLaiZiValue");
            _item.SelfData.Action=EnumMahJongAction.StandWith;
            Invoke("ShowTag",ShowStandWait);
        }

        private void ShowTag()
        {
            CancelInvoke("ShowTag");
            _item.SelfData.Action = EnumMahJongAction.Lie;
            Invoke("Moto",MoveWait);
        }


        private void Moto()
        {
            CancelInvoke("Moto");
            iTween.MoveTo(_item.gameObject, _toTrans.transform.position, MoveTime);
            if (_fromBg)
            _fromBg.gameObject.SetActive(false);
            Invoke("SetFanImmediate",MoveTime);
        }

        private void SetFanImmediate()
        {
            CancelInvoke("SetFanImmediate");
            if (_toTrans)
            {
                if (_toTrans.childCount > 0)
                {
                    _toTrans.DestroyChildren();
                }
                GameTools.AddChild(_toTrans, _item.transform, _laiZiScale, _laiZiScale);
                _item.SelfData.MahjongLayer = _laiZiLayer;
                _laiZiLabel.TrySetComponentValue(true);
            }
        }

        public void Reset()
        {
            if (_fromBg)
            {
                _fromBg.gameObject.SetActive(false);
                _laiZiLabel.TrySetComponentValue(false);
                int lenth = _toTrans.childCount;
                while (lenth > 0)
                {
                    lenth--;
                    Destroy(_toTrans.GetChild(0).gameObject);
                }
            }
        }

    }
}
