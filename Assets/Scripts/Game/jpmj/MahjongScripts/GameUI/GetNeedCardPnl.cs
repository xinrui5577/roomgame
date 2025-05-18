using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class GetNeedCardPnl : MonoBehaviour
    {
    
        public RectTransform Parent;
        public GameObject ShowBody;

        private List<int> _cardValues;

        void Awake()
        {
#if MJ_DEBUG
            EventDispatch.Instance.RegisteEvent((int)UIEventId.RoomInfo,OnRecveEvent);
#else
            Destroy(gameObject);
#endif
        }

        private void OnRecveEvent(int id,EventData data)
        {
            var roomInfo = (RoomInfo) data.data1;
            _cardValues = roomInfo.SysCards;

            SetCardValues();
        }

        public void Show()
        {
            ShowBody.SetActive(true);
        }

        public void Hide()
        {
            ShowBody.SetActive(false);
        }

        private void SetCardValues()
        {
            for (int i = 0; i < _cardValues.Count; i++)
            {
                GameObject mj = D2MahjongMng.Instance.GetMj(_cardValues[i],EnD2MjType.Me);
                var img = mj.GetComponent<Image>();
                if (img != null) img.raycastTarget = true;

                var mjRTF = mj.GetComponent<RectTransform>();
                mjRTF.SetParent(Parent);              
                mjRTF.localScale = Vector3.one;
                mjRTF.localPosition = Vector3.zero;

                var clickEvent = new Button.ButtonClickedEvent();
                var cardValue = _cardValues[i];
                clickEvent.AddListener(() =>
                    {
                        EventDispatch.Dispatch((int)NetEventId.GetNeedCard, new EventData(cardValue));
                        Hide();
                    });
                var mjBtn = mj.AddComponent<Button>();
                mjBtn.onClick = clickEvent;
            }
        }

    }
}
