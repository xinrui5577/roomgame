using System.Collections.Generic;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using UnityEngine;
using YxFramwork.Common.Utils;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Tea
{
    public class TeaFindRoom : FindRoomWindow
    {      
        [Tooltip("存储对应玩家茶馆Id的Key")]
        public string KeyTeaId = "TeaId"; 
        [Tooltip("自动申请加入茶馆")]
        public bool AutoApplyJoin = false;
        /// <summary>
        /// 当前茶馆功能的命名 可以叫俱乐部 亲友圈 自己填写
        /// </summary>
        public string CurrentName = "茶馆";

        protected override void OnFindRoom()
        {
            int roomType;
            var roomId = GetCurRoomId();
            if (!int.TryParse(roomId, out roomType)) return;
            var dic = new Dictionary<string, object>();
            dic["id"] = roomId;
            Facade.Instance<TwManager>().SendAction(AutoApplyJoin ? "group.teaHouseApply" : "group.teaGetIn", dic, GetInTea);

        }
     
        private void GetInTea(object msg)
        {
            var dic = (Dictionary<string, object>) msg;
            if (dic.ContainsKey("info"))
            {
                YxMessageBox.Show(dic["info"].ToString());
                return;
            }
            var value = (long)dic["mstatus"];
            if (value != 4)
            {
                var obj = CreateOtherWindow("TeaPanel");
                var panel = obj.GetComponent<TeaPanel>();
                panel.UpdateView(dic);
                panel.SetTeaCode(int.Parse(GetCurRoomId()));
                Util.SetString(KeyTeaId, GetCurRoomId());
                Close();
            }
            else
            {
                YxMessageBox.Show(string.Format("{0}不存在", CurrentName));
            }
            Clear();
        }
    }
}
