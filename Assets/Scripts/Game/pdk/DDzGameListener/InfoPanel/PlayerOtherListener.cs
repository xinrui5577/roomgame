using System.Collections;
using System.Globalization;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.pdk.DDzGameListener.InfoPanel
{
    /// <summary>
    /// 其他玩家的listener基类
    /// </summary>
    public abstract class PlayerOtherListener : PlayerInfoListener
    {

        protected override void OnAwake()
        {
            gameObject.SetActive(false);
            base.OnAwake();
            PdkGameManager.AddOnUserOutEvt(OnUserOut);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeDouble, OnDouble);
        }
        /// <summary>
        /// 手牌数
        /// </summary>
        [SerializeField]
        protected UILabel CdNumLabel;

        /// <summary>
        /// 正在选择加倍
        /// </summary>
        [SerializeField]
        protected UILabel SeltingDoublelabel;

        /// <summary>
        /// 断线图标
        /// </summary>
        [SerializeField]
        protected GameObject DuanxianSp;

        protected void SetDuanXianSp(bool isActive)
        {
            DuanxianSp.SetActive(isActive);

            HeadTexture.color = isActive ? new Color(0.5f, 0.5f, 0.5f) : new Color(1f, 1f, 1f);
        }

        /// <summary>
        /// 警灯
        /// </summary>
        [SerializeField] protected GameObject JingDengAnim;

        /// <summary>
        /// 从服务器获得底牌信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public override void OnGetDipai(object sender, DdzbaseEventArgs args)
        {
            base.OnGetDipai(sender,args);
            UpdateCdsNum();
        }

        /// <summary>
        /// 游戏结束后清空CdNumLabel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnTypeGameOver(object sender, DdzbaseEventArgs args)
        {
            base.OnTypeGameOver(sender, args);
            if (CdNumLabel != null) CdNumLabel.text = "0";
            if (JingDengAnim != null) JingDengAnim.SetActive(false);
        }

        /// <summary>
        /// 当给这个玩家发牌时
        /// </summary>
        protected override void OnAlloCateCds(object sender, DdzbaseEventArgs args)
        {
            SeltingDoublelabel.text = "";
            base.OnAlloCateCds(sender,args);
            UpdateCdsNum();
        }

        protected virtual void OnUserOut(object sender, DdzbaseEventArgs args)
        {
            if(UserDataTemp == null) return;

            var data = args.IsfObjData;
            if (data == null || !data.ContainsKey(RequestKey.KeySeat)) return;
            if (UserDataTemp != null && data.GetInt(RequestKey.KeySeat) == UserDataTemp.GetInt(RequestKey.KeySeat))
            {
                //如果已经开始游戏了，则不做玩家离开设置
                if (App.GetGameData<GlobalData>().IsStartGame)
                {
                    SetDuanXianSp(true);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 根据缓存的信息刷新用户信息ui
        /// </summary>
        public override void RefreshUiInfo()
        {
            if (UserDataTemp == null) return;

            base.RefreshUiInfo();

            //多一个显示牌数的组件
            if (CdNumLabel != null && UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum))
            {
                var selfhdCdsNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum);
                CdNumLabel.text = selfhdCdsNum.ToString(CultureInfo.InvariantCulture);
                if (selfhdCdsNum>0 && selfhdCdsNum < 2 && JingDengAnim != null) JingDengAnim.SetActive(true);
            }

            gameObject.SetActive(true);

            if (UserDataTemp != null && UserDataTemp.ContainsKey(NewRequestKey.KeyNetWork))
            {
                SetDuanXianSp(!UserDataTemp.GetBool(NewRequestKey.KeyNetWork));
            }
        }

        /// <summary>
        /// 根据服务器数据设置用户信息
        /// </summary>
        protected override void SetUserInfo(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            SetOtherPlayerData(data);

/*             //如果是选择加倍阶段则不显示出牌操作相关按钮
            if (data.ContainsKey(NewRequestKey.KeyState) &&
                data.GetInt(NewRequestKey.KeyState) == GlobalConstKey.StatusDouble)
            {

               
                var users = data.GetSFSArray(RequestKey.KeyUserList);

                for (int i = 0; i < users.Count; i++)
                {
                    ISFSObject user = users.GetSFSObject(i);
                    int userSeat = user.GetInt("seat");
                    if (userSeat != UserDataTemp.GetInt(RequestKey.KeySeat)) continue;
                    

       
                    var rate = user.GetInt("rate");
                    if (rate > 0)
                        ShowSpeakSp.spriteName = rate > 1 ? SpkJiabei : SpkBuJiabei;
                    else
                    {
                        SeltingDoublelabel.gameObject.SetActive(true);
                        SeltingDoublelabel.text = "正在选择加倍";
                    }
                    break;
                }
            }*/
        }

        /// <summary>
        /// 根据玩家自己座位号存储其他玩家信息
        /// </summary>
        /// <param name="servData">服务器信息</param>
        protected abstract void SetOtherPlayerData(ISFSObject servData);

        /// <summary>
        /// 当用户进入房间时，刷新用户数据
        /// </summary>
        /// <param name="userData"></param>
        protected void UpdateUserdata(ISFSObject userData)
        {
            //存储牌数
            var cdnumTemp = 0;
            if (UserDataTemp != null && UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum))
                cdnumTemp = UserDataTemp.GetInt(NewRequestKey.KeyCardNum);

            UserDataTemp = userData;

            //如果是用户重练进来可能不含有NewRequestKey.KeyCardNum参数，这时候需要重置牌数
            if (!UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum))
                UserDataTemp.PutInt(NewRequestKey.KeyCardNum, cdnumTemp);
        }

        //更新手牌数
        private void UpdateCdsNum()
        {
            if (UserDataTemp==null || !UserDataTemp.ContainsKey(NewRequestKey.KeyCardNum)) return;
            var cdNum = UserDataTemp.GetInt(NewRequestKey.KeyCardNum);
            CdNumLabel.text = cdNum.ToString(CultureInfo.InvariantCulture);

            if (!UserDataTemp.ContainsKey(RequestKey.KeySeat)) return;
            var globalUserInfo = App.GetGameData<GlobalData>().GetUserInfo(UserDataTemp.GetInt(RequestKey.KeySeat));
            if (globalUserInfo != null) globalUserInfo.PutInt(NewRequestKey.KeyCardNum, cdNum);
            
        }

        /// <summary>
        /// 检查加倍
        /// </summary>
        /// <param name="data"></param>
        public override void OnCheckSelectDouble(ISFSObject data)
        {
            if (UserDataTemp == null) return;

            //如果加倍
            if (!data.ContainsKey(NewRequestKey.KeyJiaBei) || !data.GetBool(NewRequestKey.KeyJiaBei))
            {
                return;
            }

            var globaldata = App.GetGameData<GlobalData>();
            if (globaldata.JiaBeiType != 2 || (globaldata.JiaBeiType == 2 && UserDataTemp.GetInt(RequestKey.KeySeat) != data.GetInt(RequestKey.KeySeat)))
            {
                SetSpeakSpState(false);
                SeltingDoublelabel.gameObject.SetActive(true);
                SeltingDoublelabel.text = "正在选择加倍";
            }
        }

        /// <summary>
        /// 当收到加倍信息
        /// </summary>
        private void OnDouble(object sender, DdzbaseEventArgs args)
        {
            if (UserDataTemp == null) return;

            var data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
            if (seat == UserDataTemp.GetInt(RequestKey.KeySeat))
            {
                SeltingDoublelabel.text = "";
            }
        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        protected override void OnDoubleOver(object sender, DdzbaseEventArgs args)
        {
            base.OnDoubleOver(sender,args);
            SeltingDoublelabel.text = "";
        }
    }
}
