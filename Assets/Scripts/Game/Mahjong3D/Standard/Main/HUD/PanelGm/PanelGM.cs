using System.Collections.Generic;
using YxFramwork.ConstDefine;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelGM), UIPanelhierarchy.Base)]
    public class PanelGM : UIPanelBase
    {
        public RectTransform Parent;
        public GameObject ShowBody;
        public Image GmCard;
        public GmBao GmBao;
        private bool mFlag;

        public override void OnGetInfoUpdate()
        {
#if MJ_DEBUG && MJ_BAO
            GmBao.Show();
#else
            GmBao.Hide();
#endif

#if MJ_DEBUG
            GmCard.gameObject.SetActive(true);
            GmCard.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
#else
            var db = GameCenter.DataCenter;
            if (db.OneselfData.VipUser && db.Config.GmFlag)
            {
                GmCard.gameObject.SetActive(true);
                GmCard.color = new Color(0, 0, 0, 0);
            }
            else
            { 
                Close();
            }
#endif
        }

        public void Show()
        {
            var db = GameCenter.DataCenter;
#if MJ_DEBUG
            if (!mFlag)
#else
            if (!mFlag && db.OneselfData.VipUser)
#endif
            {
                mFlag = true;
                var cards = db.Room.SysCards;
                if (null != cards)
                {
                    SetCardValues(db.Room.SysCards);
                }
            }
            ShowBody.SetActive(true);
        }

        public void Hide()
        {
            ShowBody.SetActive(false);
        }

        private void SetCardValues(List<int> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                GameObject mj = GameCenter.Assets.GetUIMahjong(cards[i], UIMahjongType.Big);
                var img = mj.GetComponent<Image>();
                if (img != null) img.raycastTarget = true;
                var mjRTF = mj.GetComponent<RectTransform>();
                mjRTF.SetParent(Parent);
                mjRTF.localScale = Vector3.one;
                mjRTF.localPosition = Vector3.zero;
                var clickEvent = new Button.ButtonClickedEvent();
                var cardValue = cards[i];
                clickEvent.AddListener(() =>
                {
                    if (GmBao != null && GmBao.GmBaoState)
                    {
                        GameCenter.Network.OnRequestC2S((sfs) =>
                        {
                            sfs.PutInt(RequestKey.KeyType, NetworkProls.NextBao);
                            sfs.PutInt("bao", cardValue);
                            return sfs;
                        });
                    }
                    else
                    {
                        GameCenter.Network.OnRequestC2S((sfs) =>
                        {
                            sfs.PutInt(RequestKey.KeyType, NetworkProls.GetNeedCard);
                            sfs.PutInt(RequestKey.KeyCard, cardValue);
                            return sfs;
                        });
                    }
                    Hide();
                });
                var mjBtn = mj.AddComponent<Button>();
                mjBtn.onClick = clickEvent;
            }
        }
    }
}