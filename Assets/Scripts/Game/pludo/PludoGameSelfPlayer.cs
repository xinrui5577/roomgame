using Assets.Scripts.Game.pludo.View;
using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     PludoGameSelfPlayer.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-24
 *描述:        	飞行棋当前玩家
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo
{
    public class PludoGameSelfPlayer : PludoGamePlayer 
    {
        #region UI Param
        #endregion

        #region Data Param

        /// <summary>
        /// 起飞标识定义
        /// </summary>
        public bool CouldFly { private set; get; }
        /// <summary>
        /// 是否可以打骰子（打骰子阶段开关）
        /// </summary>
        public bool CouldRollDice { private set; get; }
        /// <summary>
        /// 是否可以使用遥控骰子
        /// </summary>
        public bool CouldUseControlDice {
            get
            {
                if (CurInfo!=null)
                {
                    return CouldRollDice && CurInfo.ControlDiceTime > ConstantData.IntValue;
                }
                else
                {
                    return false;
                }

            } 
        }

        #endregion

        #region Local Data
        #endregion

        #region Life Cycle

        protected override void ShowOpWithCd()
        {
            base.ShowOpWithCd();
            switch (CurInfo.Status)
            {
                case PludoPlayerStatus.RollDic:
                    CouldFly = false;
                    CouldRollDice = true;
                    break;
                case PludoPlayerStatus.ChoosPlane:
                    CouldRollDice = false;
                    CheckFlyState(true);
                    break;
                case PludoPlayerStatus.Sleep:
                    CheckFlyState(false);
                    CurInfo.RollDiceData.PlaneIds.Clear();
                    CouldRollDice = false;
                    break;
            }
            FreshBtnAction();
        }


        #endregion

        #region Function

        private void FreshBtnAction()
        {
            Facade.EventCenter.DispatchEvent(LoaclRequest.FreshSelfPlayerAction,ConstantData.IntDefValue);
        }

        /// <summary>
        /// 检测起飞状态
        /// </summary>
        public void CheckFlyState(bool select)
        {
            CouldFly = false;
            Control.OnPlaneCanChoose(select);
            if (select)
            {
                if (CanChoosePlane)
                {
                    foreach (var item in CurInfo.PlaneDic)
                    {
                        if ((item.Value.Status&(int)EnumPlaneStatus.Home)==(int)EnumPlaneStatus.Home && CurInfo.RollDiceData.PlaneIds.Contains(item.Key))
                        {
                            CouldFly = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 选择飞机请求
        /// </summary>
        /// <param name="planeId">飞机ID</param>
        private void ChoosePlaneRequest(int planeId)
        {
            if (CanChoosePlane)
            {
                ISFSObject data = new SFSObject();
                data.PutInt(ConstantData.KeyPlaneId, planeId);
                data.PutInt(RequestKey.KeyType, (int)EnumGameServer.ChoosePlabe);
                Facade.EventCenter.DispatchEvent(LoaclRequest.ChoosePlaneRequest, data);
            }
        }

        #region UI Action

        #endregion
        /// <summary>
        /// 飞机起飞
        /// </summary>
        public void OnPlaneStartFly()
        {
            if (CouldFly)
            {
                foreach (var planeItem in CurInfo.PlaneDic)
                {
                    var itemValue = planeItem.Value;
                    switch (itemValue.Status)
                    {
                        case (int)EnumPlaneStatus.Home:
                            ChoosePlaneRequest(itemValue.DataId);
                            CouldFly = false;
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// 点击打骰子按钮
        /// </summary>
        public void OnClickRollDice()
        {
            if (CouldRollDice)
            {
                ISFSObject data = new SFSObject();
                data.PutInt(RequestKey.KeyType, (int)EnumGameServer.RollDicRequest);
                Facade.EventCenter.DispatchEvent(LoaclRequest.RollDiceRequest, data);
                CouldRollDice = false;
            }
        }
        /// <summary>
        /// 飞机点击事件
        /// </summary>
        /// <param name="planeId">飞机ID</param>
        public void OnPlaneClick(int planeId)
        {
            ChoosePlaneRequest(planeId);
        }

        /// <summary>
        /// 点击遥控骰子
        /// </summary>
        public void OnClickConrtolDicePoint(string point)
        {
            if (CouldRollDice)
            {
                var pointValue = int.Parse(point);
                ISFSObject data = new SFSObject();
                data.PutInt(ConstantData.KeyRollPoint, pointValue);
                data.PutInt(RequestKey.KeyType, (int)EnumGameServer.ControlPoint);
                Facade.EventCenter.DispatchEvent(LoaclRequest.RollDiceRequest, data);
                CouldRollDice = false;
            }
        }

        /// <summary>
        /// 遥控骰子失败
        /// </summary>
        public void OnControlDiceFail()
        {
            CouldRollDice = true;
        }

        #endregion
    }
}
