using Sfs2X.Entities.Data;
using YxFramwork.Common.Utils;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.LXGameScripts
{
    public class OverallData : YxGameData
    {
        private int _ante;//底注
        private int _pour;

        private long _caichi;//彩池的金币数

        public long CaiChi
        { get { return _caichi; } }

        private int _line;//有多少条线
        public int Line { get { return _line; } }

        private long _userGold;//用户的金币
        public long UserGold { get { return _userGold; } }

        private int _userCash;
        public int UserCash { get { return _userCash; } }

        private readonly ResponseData _response = new ResponseData();
        public ResponseData Response { get { return _response; } }

        public void SetResponseData(ISFSObject sfsObject)
        {
            _response.ParseData(sfsObject);
            _pour = sfsObject.GetInt("ante");
            GetPlayer().Coin = _userGold - (_pour * _ante * Line);
            GetPlayer().UpdateView();
            _userGold = sfsObject.GetLong("ttgold");
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            _ante = gameInfo.GetInt("ante");
            _caichi = gameInfo.GetLong("caichi");
            gameInfo.GetInt("hor");//横向有几个
            _line = gameInfo.GetInt("lines");//总共有几条线
            _response.ShowLine = gameInfo.GetInt("ver");//纵向有几个
            ISFSObject userInfo = gameInfo.GetSFSObject("user");
            _userGold = userInfo.GetLong("ttgold");
            _userCash = userInfo.GetInt("cash");
            EventDispatch.Dispatch((int)EventID.GameEventId.InitBottomPourData, new EventData(_ante, _line));
            EventDispatch.Dispatch((int)EventID.GameEventId.AlterPotGold, new EventData(_caichi));
        }
    }
}