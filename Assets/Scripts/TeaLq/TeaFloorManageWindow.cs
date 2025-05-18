using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Tea;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.TeaLq
{
    public class TeaFloorManageWindow : YxNguiWindow
    {
        public List<GameObject> FloorbBoards;
        public List<NguiLabelAdapter> FloorGameNames;
        public List<GameObject> CurrentPosTips;
        public GameObject RestBtn;
        public GameObject OpenBtn;

        protected override void OnStart()
        {
            base.OnStart();
            if (CurrentPosTips.Count != 0)
            {
                CurrentPosTips[TeaMainPanel.Floor - 1].SetActive(true);
            }
            var dic = new Dictionary<string, object>();
            dic["teaId"] = TeaMainPanel.CurTeaId;
            Facade.Instance<TwManager>().SendAction("group.getTeaFloor", dic, UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dic = Data as Dictionary<string, object>;
            if (dic == null) return;
            var datas = dic["data"] as List<object>;
            if (datas != null)
            {
                foreach (var data in datas)
                {
                    var info = data as Dictionary<string, object>;
                    if (info == null) return;
                    var floor = int.Parse(info["floor_id"].ToString());
                    var gameName = info["game_name"].ToString();
                    FloorShow(floor - 1, gameName);

                }
            }
            var teaState = int.Parse(dic["tea_state"].ToString());
            if (teaState == 0)
            {
                BtnCtrl(true);
            }
            else if (teaState == 1)
            {
                BtnCtrl(false);
            }
        }

        private void FloorShow(int index, string gameName)
        {
            FloorbBoards[index].SetActive(false);
            FloorGameNames[index].TrySetComponentValue(gameName);
            FloorGameNames[index].gameObject.SetActive(true);
        }

        public void OnTeaStateControl(string objName)
        {
            if (int.Parse(objName) == 1)
            {
                BtnCtrl(false);
            }
            else if (int.Parse(objName) == 0)
            {

                BtnCtrl(true);
            }
            var dic = new Dictionary<string, object>();
            dic["teaId"] = TeaMainPanel.CurTeaId;
            dic["type"] = objName;
            Facade.Instance<TwManager>().SendAction("group.teaStateManage", dic, data =>
            {
                var obj = data as Dictionary<string, object>;
                if (obj == null) return;
                var info = obj["info"].ToString();
                YxMessageBox.Show(info);

                if (obj.ContainsKey("backhall"))
                {
                    Facade.EventCenter.DispatchEvent<string, object>("BackHall", info);

                }
            });
        }

        private void BtnCtrl(bool state)
        {
            if (RestBtn)
            {
                RestBtn.SetActive(state);
            }
            if (OpenBtn)
            {
                OpenBtn.SetActive(!state);

            }
        }

        public void OpenTeaCreatRoomWindow(string windowName, string floor)
        {
            var main = MainYxView as YxWindow;
            if (main == null) return;
            var win = main.CreateChildWindow(windowName);
            win.GetComponent<TeaCreateRoomWindow>().FromInfo = 1.ToString();
            win.GetComponent<TeaCreateRoomWindow>().CurFloor = int.Parse(floor);
            win.GetComponent<TeaCreateRoomWindow>().TrueTeaId = TeaMainPanel.CurTeaId;
        }

        /// <summary>
        /// 打开规则面板
        /// </summary>
        /// <param name="window"></param>
        public void OpenRuleWindow(string window)
        {
            var main = MainYxView as YxWindow;
            if (main == null) return;
            var win = main.CreateChildWindow(window);
            win.GetComponent<TeaCreateRoomWindow>().GameKey = TeaMainPanel.CurGameKey;
            win.UpdateView();
        }
    }
}
