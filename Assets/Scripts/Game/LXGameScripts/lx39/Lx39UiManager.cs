
namespace Assets.Scripts.Game.LXGameScripts.lx39
{
    public class Lx39UiManager : GameUiManager
    {
        public ShowLasers ShowLaser;
        public PlayLx39Effect GameEffect;
        public ButtonChange Change;
        public PlayInserIcon PlayInserIcon;
        public ShowAwardList AwardList;

        protected override void ResisteEvent()
        {
            base.ResisteEvent();
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.ShowRedLineIfWin, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.PlayInsertCoinsAnim, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.PlayEffect, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.PlayZaDanEffect, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.ChangeButtonIcon, Resiste);
        }

        protected override void Resiste(int id, EventData data)
        {
            base.Resiste(id, data);
            EventID.GameEventId type = (EventID.GameEventId) id;
            switch (type)
            {
                case EventID.GameEventId.ShowRedLineIfWin:
                    ShowLaser.ShowLaserOnWin();
                    break;
                case EventID.GameEventId.PlayInsertCoinsAnim:
                    PlayInserIcon.OnPlayInsert();
                    break;
                case EventID.GameEventId.PlayEffect:
                    GameEffect.ChooseEffectPlay();
                    break;
                case EventID.GameEventId.PlayZaDanEffect:
                    GameEffect.PlayZaDanEffect(data);
                    break;
                case EventID.GameEventId.ChangeButtonIcon:
                    ChooseIcon(data);
                    break;
            }
        }
        public void OnShowAward()
        {
            AwardList.Show();
        }


        protected override void HideAllEffect()
        {
            ShowLaser.HideAllLaser();//隐藏红线特效
            GameEffect.HideZaDan();//隐藏砸蛋特效
            GameEffect.HideNineEffect();//隐藏九连特效
            GameEffect.HideCaiDaiEffect();//隐藏中奖特效的
            Change.ChooseShowIcon(0);
        }
        protected void ChooseIcon(EventData data)
        {
            if (data.data2 == null)
            {
                Change.ChooseShowIcon((int)data.data1);
            }
            else
            {
                if ((bool)data.data2)
                {
                    if (GameMove.IsStartRollJetton)
                        Change.ChooseShowIcon(3);
                    else
                        Change.ChooseShowIcon(2);
                }
                else
                {
                    if (GameMove.IsStartRollJetton)
                        Change.ChooseShowIcon(1);
                    else
                        Change.ChooseShowIcon(0);
                }
            }
        }
    }
}

