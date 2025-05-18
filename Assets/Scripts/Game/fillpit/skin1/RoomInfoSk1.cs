namespace Assets.Scripts.Game.fillpit.skin1
{
    public class RoomInfoSk1 : RoomInfo
    {

        public UILabel DoubleMark;

        public UILabel LanDiMark;

        public UIGrid Grid;

        protected override void ShowDoubleString()
        {
            var gdata = YxFramwork.Common.App.GetGameData<FillpitGameData>();
            string roundInfo = string.Empty;

            LanDiMark.gameObject.SetActive(gdata.IsLanDi);
            
            roundInfo += string.Format(RoomRoundFormat, CurRound, MaxRound);
            RoundLabel.text = roundInfo;

            DoubleMark.gameObject.SetActive(gdata.IsDoubleGame);

            if (Grid != null)
            {
                Grid.repositionNow = true;
                Grid.Reposition();
            }
        }
    }
}
