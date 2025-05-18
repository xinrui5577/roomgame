using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    /// <summary>
    ///     癞子牌控制类，这个应该属于Common部分
    /// </summary>
    public class LaiZiControl : MonoSingleton<LaiZiControl>
    {
        [SerializeField] private UISprite _fromBg;

        [SerializeField] private Transform _fromTrans;

        private MahjongItem _item;

        [SerializeField] private GameObject _laiZiLabel;

        private int _showValue;

        [SerializeField] private Transform _toTrans;

        [SerializeField] public float MoveTime = 2;

        [SerializeField] public float MoveWait = 1;

        public float ScaleX = 1;

        public float ScaleY = 1;

        [SerializeField] public float ShowLaiziWait = 1;

        [SerializeField] public float ShowStandWait = 1;

        public int ShowValue
        {
            get { return ShowValue; }
        }

        public override void Awake()
        {
            base.Awake();
            _fromBg.gameObject.SetActive(false);
        }

        public void CreateMahjong(int value, bool immediate)
        {
            _item = App.GetGameManager<Lyzz2DGameManager>().GetNextMahjong().GetComponent<MahjongItem>();
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
                _laiZiLabel.SetActive(false);
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
            _item.SelfData.Action = EnumMahJongAction.StandWith;
            Invoke("ShowTag", ShowStandWait);
        }

        private void ShowTag()
        {
            Debug.Log("显示标记");
            CancelInvoke("ShowTag");
            _item.SelfData.Action = EnumMahJongAction.Lie;
            Invoke("Moto", MoveWait);
        }


        private void Moto()
        {
            CancelInvoke("Moto");
            iTween.MoveTo(_item.gameObject, _toTrans.transform.position, MoveTime);
            _fromBg.gameObject.SetActive(false);
            Invoke("SetFanImmediate", MoveTime);
        }

        private void SetFanImmediate()
        {
            CancelInvoke("SetFanImmediate");
            GameTools.AddChild(_toTrans, _item.transform);
            _laiZiLabel.SetActive(true);
        }

        public void Reset()
        {
            _fromBg.gameObject.SetActive(false);
            _laiZiLabel.SetActive(false);
            var lenth = _toTrans.childCount;
            while (lenth > 0)
            {
                lenth--;
                Destroy(_toTrans.GetChild(0).gameObject);
            }
        }
    }
}