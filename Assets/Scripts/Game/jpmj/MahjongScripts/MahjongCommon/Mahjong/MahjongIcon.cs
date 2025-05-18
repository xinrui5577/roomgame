using System;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class MahjongIcon : MonoBehaviour
    {
        public enum Flag
        {
            Ting,
            Youjin, 
            None,
        }

        public bool IsQuerying = false;

        public Flag CurrFlag { get { return _currFlag; } }
        public MahjongItem MahjongItem { get { return _item; } }

        protected Flag _currFlag = Flag.None;
        protected MahjongItem _item;
        protected GameObject _icon;

        public void Reset()
        {
            _currFlag = Flag.None;
            IsQuerying = false;

            if (_icon != null)
            {
                ObjectCachePool.Singleton.Push(_icon.name, _icon);
                _icon = null;
            }
        }
        
        void Awake()
        {
            _item = gameObject.GetComponent<MahjongItem>();
        }

        void OnEnable()
        {
            if (!UtilData.UsingQueryHu) return;
            EventDispatch.Instance.RegisteEvent((int)GameEventId.TingList, OnTingList);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.ChooseTing, OnTingList);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.YouJinList, OnYouJinList);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.OutPuCard, OnOutCard);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.ChooseTingCancel, OnChooseTingCancel);
            EventDispatch.Instance.RegisteEvent((int)GameEventId.CancelTingIcon, CancelTingIcon);
        }

        void OnDisable()
        {
            if (!UtilData.UsingQueryHu) return;
            EventDispatch.Instance.RemoveEvent((int)GameEventId.TingList, OnTingList);
            EventDispatch.Instance.RemoveEvent((int)GameEventId.ChooseTing, OnTingList);
            EventDispatch.Instance.RemoveEvent((int)GameEventId.YouJinList, OnYouJinList);
            EventDispatch.Instance.RemoveEvent((int)GameEventId.OutPuCard, OnOutCard);
            EventDispatch.Instance.RemoveEvent((int)GameEventId.ChooseTingCancel, OnChooseTingCancel);
            EventDispatch.Instance.RemoveEvent((int)GameEventId.CancelTingIcon, CancelTingIcon);
        }

        private void CancelTingIcon(int eventid, EventData data)
        {
            Reset();
        }

        protected void OnChooseTingCancel(int eventid, EventData data)
        {
            Reset();
        }

        protected void OnOutCard(int eventid, EventData data)
        {
            Reset();  
        }

        protected void OnYouJinList(int eventid, EventData data)
        {
            if (_item.Lock) return;         

            var list = (int[])data.data1;
            if (Array.Exists(list, value => _item.Value == value))
            {
                _currFlag = Flag.Youjin;              

                if (_icon != null)
                    ObjectCachePool.Singleton.Push(_icon.name, _icon);

                var mjSize = MahjongManager.MagjongSize;
                _icon = ObjectCachePool.Singleton.Pop("YouJinIcon(Clone)");

                if (null == _icon)
                {
                    var asset = ResourceManager.LoadAsset("YouJinIcon", "YouJinIcon");
                    if (asset == null) return;

                    _icon = Instantiate(asset);
                }

                _icon.gameObject.layer = _item.gameObject.layer;
                _icon.transform.parent = _item.transform;
                _icon.transform.localPosition = new Vector3(0, mjSize.y, 0);
                _icon.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                _icon.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        protected virtual  void OnTingList(int eventid, EventData data)
        {
            if (_item.Lock) return;
            var list = (int[])data.data1;
            if (Array.Exists(list, value => _item.Value == value))
            {
                _currFlag = Flag.Ting;               

                if (_icon != null)
                    ObjectCachePool.Singleton.Push(_icon.name, _icon);

                var mjSize = MahjongManager.MagjongSize;
                _icon = ObjectCachePool.Singleton.Pop("TingIcon(Clone)");

                if (null == _icon)
                {
                    _icon = Instantiate(MahjongManager.Instance.TingIcon);
                    if (_icon == null) return;
                }

                _icon.gameObject.layer = _item.gameObject.layer;
                _icon.transform.parent = _item.transform;
                _icon.transform.localPosition = new Vector3(0, mjSize.y*0.8f, 0);
                _icon.transform.localScale = new Vector3(0.3f, 0.3f, 1);
                _icon.transform.localRotation = Quaternion.Euler(0, 0, 0);

            }
        }
      
        public void OnRemoveComponent() { Reset(); }

        public bool OnQuery(Transform transform)
        {
            MahjongIcon icon = transform.GetComponent<MahjongIcon>();           

            if (null == icon || icon.CurrFlag == Flag.None || icon.IsQuerying) return false;            

            IsQuerying = true;

            EventDispatch.Dispatch((int)NetEventId.OnQueryHuLish, new EventData(this));  
   
            return true;
        }
    }
}
