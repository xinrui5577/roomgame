using Assets.Scripts.Game.fillpit.Mgr;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class RuleViewMgrSk2 : RuleViewMgr
    {

        public UILabel RoomIdLabel;

        public string RoomIdFormat = "桌 号: {0}";

        public UILabel RoomNameLabel;

        public string RoomNameFormat = "名 称: {0}";

        public UILabel AnteLabel;

        public string AnteFormat = "底 分: {0}";



        public override void SetRuleViewInfo(ISFSObject gameInfo)
        {
            if (ItemPrefab == null) return;
            Table.transform.DestroyChildren();
            if (!gameInfo.ContainsKey("rule"))
            {
                gameObject.SetActive(false);
                return;
            }

            string ruleInfo = gameInfo.GetUtfString("rule");
            if (string.IsNullOrEmpty(ruleInfo))
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            var ruleSplit = ruleInfo.Split(';');
            int len = ruleSplit.Length;
            var go = CreateItem();
            var strRule = string.Empty;
            for (int i = 0; i < len; i++)
            {
                var info = ruleSplit[i].Split(':');
                string content =  info[1].Replace(' ', '\n');
                if (!content.Contains("\n"))
                {
                    content += "\n";
                }
                strRule += content;
            }

            go.SetRuleItem(string.Empty, strRule);
            Table.repositionNow = true;
            Table.Reposition();

            if (gameInfo.ContainsKey("rid"))
            {
                int roomId = gameInfo.GetInt("rid");
                ShowLabel(RoomIdLabel, RoomIdFormat, roomId.ToString());
            }   

            if (gameInfo.ContainsKey("roomName"))
            {
                string gameName = gameInfo.GetUtfString("roomName");
                ShowLabel(RoomNameLabel, RoomNameFormat, gameName);
            }

            if (gameInfo.ContainsKey("ante"))
            {
                int ante = gameInfo.GetInt("ante");
                ShowLabel(AnteLabel, AnteFormat, ante.ToString());
            }
        }

       



        void ShowLabel(UILabel label, string format, string main)
        {
            label.text = string.Format(format, main);
            label.gameObject.SetActive(true);
        }
    }
}
