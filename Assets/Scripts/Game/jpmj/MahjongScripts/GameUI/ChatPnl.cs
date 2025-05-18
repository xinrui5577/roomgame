using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class ChatPnl : PopPnlBase
    {
        public GameObject ExpressionGrid;
        public GameObject SortTalkGrid;
        public Text Input;

        public bool _isSetSortTalk;
        void Start()
        {
            //设置表情
            SetExpress();          
        }

        public void SetExpress()
        {
            List<GameObject> list = ChatManager.Instance.GetExpressIcon(OnExpressClick);
            foreach (GameObject obj in list)
            {
                var rectTf = obj.GetComponent<RectTransform>();
                rectTf.parent = ExpressionGrid.transform;
                rectTf.localScale = Vector3.one;
                rectTf.localPosition = Vector3.zero;
            }
        }

        public virtual void SetSortTalk(int sex)
        {
            
            if (_isSetSortTalk) return;

            _isSetSortTalk = true;
            SortTalkGrid.transform.DestroyChildren();
            List<GameObject> list = ChatManager.Instance.GetSortTalkList(sex, OnSortTalkClick);
            foreach (GameObject obj in list)
            {
                var rectTf = obj.GetComponent<RectTransform>();
                rectTf.SetParent(SortTalkGrid.transform);
                rectTf.localScale = Vector3.one;
                rectTf.localPosition = Vector3.zero;
            }
        }

        protected void OnExpressClick(int key)
        {
            EventDispatch.Dispatch((int)NetEventId.OnUserTalk, new EventData(EnChatType.exp, key));
            Hide();
        }

        protected void OnSortTalkClick(int key)
        {
            EventDispatch.Dispatch((int)NetEventId.OnUserTalk, new EventData(EnChatType.sorttalk, key));
            Hide();
        }

        public void Close()
        {
            Hide();
        }

        public void SendChat()
        {
            if (Input.text.Length > 0)
            {
                EventDispatch.Dispatch((int)NetEventId.OnUserTalk, new EventData(EnChatType.text, Input.text));
                Input.text = "";
                Input.GetComponent<InputField>().text = "";
                Hide();
            }
        }
        
    }
}
