using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class RoomInfoPnl : PopPnlBase
    {
        //public Text Content;
        //public GameObject Pnl;
        [SerializeField]
        private Animator animator;

        //void Awake()
        //{
        //    EventDispatch.Instance.RegisteEvent((int)UIEventId.RoomInfo, OnRoomInfo);            
        //}

        //protected virtual void OnRoomInfo(int id, EventData data)
        //{
        //    var roomInfo = (RoomInfo) data.data1;
        //    Content.text = "局数:" + roomInfo.MaxRound + roomInfo.GetLoopString();
        //    Content.text += "\r\n";
        //    Content.text += "玩法:" + roomInfo.GetRoomRuleString();
        //}

        protected override void OnShow()
        {
            if (animator == null)
            {
                return;
            }         
            animator.SetBool("show",true);
        }

        protected override void OnHide()
        {
            if (animator == null)
            {
                 return;
            }
            animator.SetBool("show",false);
        }
    }
}
