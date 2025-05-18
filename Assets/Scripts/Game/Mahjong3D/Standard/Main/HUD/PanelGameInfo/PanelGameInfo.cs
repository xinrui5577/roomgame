using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using YxFramwork.View;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelGameInfo), UIPanelhierarchy.Base)]
    public class PanelGameInfo : UIPanelBase
    {
        public AbsGameInfo GameInfo;
        public GameObject CopyBtn;

        public override void OnInit()
        {
            base.OnInit();
            EventHandlerComponent Event = GameCenter.EventHandle;
            Event.Subscriber((int)EventKeys.UpdateMahCount, UpdateMahjongCount);
        }

        public override void OnGetInfoUpdate()
        {
            GameInfo.OnGetInfoRefresh();
            if (DataCenter.Config.CopyRoomid && CopyBtn != null)
            {
                CopyBtn.gameObject.SetActive(DataCenter.Room.RealityRound == 0 && DataCenter.Room.RoomType == MahRoomType.FanKa);
            }
        }

        public override void OnReadyUpdate()
        {
            GameInfo.OnReadyRefresh();          
        }

        public override void OnStartGameUpdate()
        {
            GameInfo.OnStartGameUpdate();
            if (CopyBtn != null)
            {
                CopyBtn.gameObject.SetActive(false);
            }
        }

        public void UpdateMahjongCount(EvtHandlerArgs args)
        {
            GameInfo.UpdateMahjongCount(null);
        }

        public void OnCopyClick()
        {
            Facade.Instance<YxGameTools>().PasteBoard = DataCenter.Room.RoomID.ToString();
            YxMessageBox.Show(new YxMessageBoxData { Msg = "成功复制到剪切板", });
        }
    }
}