using System.Collections.Generic;
using Assets.Scripts.Common.Interface;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Interface;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.View;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common
{
    [RequireComponent(typeof (YxGameData))]
    public class GameEnter : Enter
    {
        protected override void OnAwake()
        {
        }

        protected override ISysCfg GetSystemCfg()
        {
            return new SysConfig();
        }

        protected override IUI GetUIImpl()
        {
            return new NguiImpl();
        }

#if UNITY_EDITOR || YX_DEVE

        private bool _isShow;

        private void OnGUI()
        {
            if (SysCfg==null) return;
            GlobalUtile.ResizeGUIMatrix();
            var sh = Screen.height;
            var fontStyle = new GUIStyle
                {
                    normal =
                        {
                            textColor = Color.white
                        }
                };
            GUILayout.BeginHorizontal();

            if (GUI.Button(new Rect(-5, 0, 15, sh), ""))
            {
                _isShow = !_isShow;
            }
            GUILayout.EndHorizontal();
            if (!_isShow) return;
            GUI.Box(new Rect(-5, 0, Screen.width, sh), "");
            switch (_showUIType)
            {
                case GameLoginState.Login: //登陆
                    OnDrawLoginUI(fontStyle);
                    break;
                case GameLoginState.Create: //创建房间
                    OnDrawCreateUI(fontStyle);
                    break;
                case GameLoginState.Register: //注册
                    OnDrawRegister(fontStyle);
                    break;
            }
        }

        private string _edit_roomId ="";
        /// <summary>
        /// 登陆 UI
        /// </summary>
        /// <param name="fontStyle"></param>
        private void OnDrawLoginUI(GUIStyle fontStyle)
        {
            const int gx = 30;
            var loginfo = LoginInfo.Instance;
            var curY = 10;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            GUI.Label(new Rect(gx, curY, 40, 20), "URL", fontStyle);
            var oldUrl = GUI.TextField(new Rect(gx + 55, curY, 100, 20), PlayerPrefs.GetString("login_editor_url",""));
            SysCfg.ServerUrl = oldUrl;
            PlayerPrefs.SetString("login_editor_url", oldUrl);
            GUILayout.EndHorizontal();
            curY += 20;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            GUI.Label(new Rect(gx, curY, 40, 20), "IP", fontStyle);
            var oldIp = GUI.TextField(new Rect(gx + 55, curY, 100, 20), PlayerPrefs.GetString("login_editor_ip"));
            PlayerPrefs.SetString("login_editor_ip", oldIp);
            var isUse = PlayerPrefs.GetInt("login_editor_use") == 1;
            isUse = GUI.Toggle(new Rect(gx + 170, curY, 25, 25), isUse, "");
            PlayerPrefs.SetInt("login_editor_use", isUse ? 1 : 0);
            if (isUse) App.DevGameServer = oldIp;
            GUILayout.EndHorizontal();
            curY += 25;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            GUI.Label(new Rect(gx, curY, 40, 20), "用户名", fontStyle);
            var newName = GUI.TextField(new Rect(gx + 55, curY, 100, 20), PlayerPrefs.GetString("login_editor_userName"));
            PlayerPrefs.SetString("login_editor_userName", newName);
            var userName = newName;
            GUILayout.EndHorizontal();
            curY += 25;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            GUI.Label(new Rect(gx, curY, 40, 20), "密  码", fontStyle);
            var pwd = GUI.TextField(new Rect(gx + 55, curY, 100, 20), PlayerPrefs.GetString("login_editor_userPwd"));
            PlayerPrefs.SetString("login_editor_userPwd", pwd);
            var userPwd = pwd;
            GUILayout.EndHorizontal();
            curY += 25;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            GUI.Label(new Rect(gx, curY, 40, 20), "创建模式", fontStyle);
            var needCreate = PlayerPrefs.GetInt("login_editor_needCreate") == 1;
            needCreate = GUI.Toggle(new Rect(gx + 60, curY, 25, 25), needCreate, "");
            PlayerPrefs.SetInt("login_editor_needCreate", needCreate ? 1 : 0);
            GUILayout.EndHorizontal();
            if (!needCreate)       //---------------------------------------------------------------------------
            {
                curY += 25;
                GUILayout.BeginHorizontal();
                GUI.Label(new Rect(gx, curY, 40, 20), "房间id", fontStyle);
                var rid = GUI.TextField(new Rect(gx + 55, curY, 100, 20), _edit_roomId);
                int roomId;
                if (int.TryParse(rid, out roomId))
                {
                    _edit_roomId = rid;
                    App.RoomId = roomId;
                }
                GUILayout.EndHorizontal();
            }
            curY += 25;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            if (GUI.Button(new Rect(gx, curY, 40, 20), "登 录"))
            {
                var ctr = RoomListController.Instance;
//                ctr.LaunchGame = (isRejoin) => LoginSfs(SelfJoinTyp.ReJoin);
                if (needCreate) UserController.LaunchGame = OnCreateUI;
                else UserController.LaunchGame = OnLogin;
                UserController.Instance.Login(userName, userPwd, true);
            }
            if (GUI.Button(new Rect(gx + 70, curY, 40, 20), "游 客"))
            {
                var ctr = RoomListController.Instance;
//                ctr.LaunchGame = (isRejoin) => LoginSfs(SelfJoinTyp.ReJoin);
                if (needCreate) UserController.LaunchGame = OnCreateUI;
                else UserController.LaunchGame = OnLogin;
                UserController.Instance.VisitorLogin();
            }
            if (GUI.Button(new Rect(gx + 140, curY, 40, 20), "注 册"))
            {
                _showUIType = GameLoginState.Register;
            }
            GUILayout.EndHorizontal();
        }

        private void OnLogin(object msg)
        {
            _showUIType = GameLoginState.Normal; 
            LogInSmart(true);
        }

        private GameLoginState _showUIType = GameLoginState.Login;

        protected void OnCreateUI(object msg)
        {
            _showUIType = GameLoginState.Create;
        }

        private string _findRoomId = "";

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="fontStyle"></param>
        private void OnDrawCreateUI(GUIStyle fontStyle)
        {
            const int gx = 30;
            var curY = 10;
            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(gx, curY, 100, 20), "规则ID", fontStyle);
            var ruleId = GUI.TextField(new Rect(gx + 100, curY, 250, 20), PlayerPrefs.GetString("login_editor_ruleId"));
            PlayerPrefs.SetString("login_editor_ruleId", ruleId);
            curY += 25;
            GUI.Label(new Rect(gx, curY, 100, 20), "创建房间参数", fontStyle);
            var createParm = GUI.TextField(new Rect(gx + 100, curY, 250, 20),
                                           PlayerPrefs.GetString("login_editor_createparm"));
            PlayerPrefs.SetString("login_editor_createparm", createParm);
            curY -= 25;
            GUILayout.EndHorizontal();
            if (GUI.Button(new Rect(gx + 400, curY, 40, 20), "创 建"))
            {
                OnCreateRoom(ruleId, createParm);
            }
            GUILayout.EndHorizontal();
            curY += 50;
            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(gx, curY, 100, 20), "查找房间ID号", fontStyle);
            _findRoomId = GUI.TextField(new Rect(gx + 100, curY, 250, 20), _findRoomId);
            if (GUI.Button(new Rect(gx + 400, curY, 40, 20), "加 入"))
            {
                OnFindRoom(_findRoomId);
            }
            GUILayout.EndHorizontal();
        }

        private void OnFindRoom(string findRoomId)
        {
            int froomId;
            int.TryParse(findRoomId, out froomId);
            RoomListController.Instance.FindRoom(froomId, obj =>
                {  
                    var data = obj as IDictionary<string, object>;
                    if (data == null)
                    {
                        YxMessageBox.Show("没有找到房间！！");
                        return;
                    }
                    var str = data.ContainsKey(RequestKey.KeyMessage) ? data[RequestKey.KeyMessage] : null;
                    if (str != null)
                    {
                        YxMessageBox.Show(str.ToString());
                        return;
                    }
                    var rid = data["roomId"];
                    Debug.Log(rid);
                    Debug.Log(rid.GetType());
                    if (rid is string)
                    {
                        rid = int.Parse(rid.ToString());
                    }
                    var roomId = int.Parse(rid.ToString());
                    YxDebug.LogError("加入房间的真实ID是" + roomId);
                    if (roomId < 1)
                    {
                        YxMessageBox.Show("查找异常！");
                        return;
                    } 
                    var gameKey = (string)(data.ContainsKey("gameKey") ? data["gameKey"] : App.GameKey);
                    var ctr = RoomListController.Instance;
                    ctr.LaunchGame = (isRejoin) => LogInSmart();
                    ctr.JoinFindRoom(roomId, gameKey);
                    
                });
        }

        private void OnCreateRoom(string ruleId, string createParm)
        {
            var data = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(createParm))
            {
                data["cargs"] = createParm;
            }
            data["ruleId"] = ruleId;
            var ctr = RoomListController.Instance;
            ctr.LaunchGame = (isRejoin) => LogInSmart();
            ctr.CreatRoom(data);
        }

        private string _regName = "";
        private string _regPwd = "";
        private string _regNike = "";

        /// <summary>
        /// 注册界面
        /// </summary>
        private void OnDrawRegister(GUIStyle fontStyle)
        {
            const int gx = 30;
            var curY = 10;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            GUI.Label(new Rect(gx, curY, 40, 20), "用户名", fontStyle);
            _regName = GUI.TextField(new Rect(gx + 55, curY, 100, 20), _regName);
            GUILayout.EndHorizontal();
            curY += 25;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            GUI.Label(new Rect(gx, 35, 40, 20), "密  码", fontStyle);
            _regPwd = GUI.TextField(new Rect(gx + 55, curY, 100, 20), _regPwd);
            GUILayout.EndHorizontal();
            curY += 25;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            GUI.Label(new Rect(gx, curY, 40, 20), "昵  称", fontStyle);
            _regNike = GUI.TextField(new Rect(gx + 55, curY, 100, 20), _regNike);
            GUILayout.EndHorizontal();
            curY += 60;
            GUILayout.BeginHorizontal(); //---------------------------------------------------------------------------
            if (GUI.Button(new Rect(gx, curY, 40, 20), "注  册"))
            {
//                var needCreate = PlayerPrefs.GetInt("login_editor_needCreate") == 1;
//                if (needCreate) UserController.Instance.Restier(_regName, _regPwd, _regNike, "", 1, OnCreateUI);
//                else UserController.Instance.Restier(_regName, _regPwd, _regNike, "", 1, OnLocalLogin);
            }
            if (GUI.Button(new Rect(gx + 70, curY, 40, 20), "取  消"))
            {
                _showUIType = 0;
            }
            GUILayout.EndHorizontal();
        }

        private enum GameLoginState
        {
            Normal,
            Login,
            Create,
            Register
        }
#endif
    }
}
