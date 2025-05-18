using System.Collections.Generic;
using Assets.Scripts.Common.Models.CreateRoomRules;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using Assets.Scripts.TeaLq;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaCreateRoomWindow : CreateRoomWindow
    {
        public TeaPanel teaPanel;
        public string MessageBoxName;
        public int CurFloor;
        public int TrueTeaId;

        protected override void SendCreateRoom(Dictionary<string, object> data)
        {
            if (teaPanel != null)
            {
                string teaid = teaPanel.Code.text;
                data["tea_id"] = teaid;
            }
            data["type"] = (int)CreateType;
            if (CreateType == YxECreateRoomType.CaffTower)
            {
                var info = TabDatas[LastIndex].Data as CreateRoomRuleInfo;
                if (info != null)
                {
                    data["tea_name"] = info.Name;
                    data["game_name"] = info.Name;
                    data["game_key"] = info.GameKey;
                }
                data["group_sign"] = string.Empty;
                data["type"] = (int)CreateType;
                var floor = CurFloor == 0 ? TeaMainPanel.Floor : CurFloor;
                data["floor"] = floor;
                if (TrueTeaId != 0)
                {
                    data["trueTeaId"] = TrueTeaId;
                }
                Facade.Instance<TwManager>().SendAction("group.creatTeaHouse", data, CreatTeaCallBack);
            }
            else
            {
                RoomListController.Instance.CreatGroupRoom(data, CreateRoomBack);
            }
        }

        private void CreateRoomBack(object obj)
        {
            TeaUtil.GetBackString(obj, MessageBoxName);
            teaPanel.GetTableList(false);
            Close();
        }

        private void CreatTeaCallBack(object data)
        {
            var dic = data as Dictionary<string, object>;
            Facade.EventCenter.DispatchEvent<string, object>("TeaFresh", dic);
            Facade.EventCenter.DispatchEvent<string, object>("TeaListFresh");
            Close();
        }

        public void OnModificaTeaRule()
        {
            var dic = new Dictionary<string, object>();
            dic["id"] = TeaMainPanel.CurTeaId;
            dic["floor"] = TeaMainPanel.Floor;
            dic["mstatus"] = TeaMainPanel.Mstatus;
            var cArgs = CurRuleInfo.CreateArgs;
            var argsInfo = AnalyzeArgs(cArgs);
            var parm = GetParm(argsInfo);
            dic["cargs"] = parm["cargs"];
            Facade.Instance<TwManager>().SendAction("group.changeTeaRule", dic, FreshTeaTable);
        }

        private void FreshTeaTable(object msg)
        {
            var obj = msg as Dictionary<string, object>;
            if (obj == null) return;
            var info = obj["info"].ToString();
            YxMessageBox.Show(info);
            Facade.EventCenter.DispatchEvent("TeaTableFresh", obj);
        }
    }
}
