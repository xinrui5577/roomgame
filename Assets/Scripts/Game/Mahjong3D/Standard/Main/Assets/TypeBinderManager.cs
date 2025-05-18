using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class TypeBinderManager : MonoBehaviour
    {
        protected Dictionary<Type, Type> mSuppliers = new Dictionary<Type, Type>();

        /// <summary>
        /// 根据绑定类型获取实例
        /// </summary>
        /// <typeparam name="TInject">注入类型</typeparam>
        /// <param name="binderType">绑定类型</param>
        /// <returns></returns>
        public TInject GetInstance<TInject>(Type binderType) where TInject : class
        {
            Type injectType;
            if (mSuppliers.TryGetValue(binderType, out injectType))
            {
                var instance = Activator.CreateInstance(injectType) as TInject;
                if (instance != null) return instance;
            }
            return null;
        }

        /// <summary>
        /// 类型绑定
        /// </summary>
        protected void Binder<TBinder, TInjector>()
        {
            mSuppliers[typeof(TBinder)] = typeof(TInjector);
        }

        /// <summary>
        /// 解除绑定类型
        /// </summary>
        protected void RemoveBinder<TBinder>()
        {
            var binderType = typeof(TBinder);
            if (mSuppliers.ContainsKey(binderType))
            {
                mSuppliers.Remove(binderType);
            }
        }

        public virtual List<Type> CollectionGameLogicType()
        {
            //玩家数据
            Binder<MahjongGameData, MahjongUserInfo>();
            //S2C 命令注册
            Binder<CommandResponseCommon, ActionCommonResponse>();
            Binder<CommandThrowoutCard, ActionThrowoutCard>();
            Binder<CommandSendCard, ActionSendCard>();
            Binder<CommandCpg, ActionCpg>();
            Binder<CommandGetCard, ActionGetCard>();
            Binder<CommandHu, ActionHu>();
            Binder<CommandBuzhang, ActionBuzhang>();
            Binder<CommandOperate, ActionOperate>();
            Binder<CommandReconnect, ActionReconnect>();
            Binder<CommandTing, ActionTing>();
            Binder<CommandChangeCard, ActionChangeCard>();
            Binder<CommandDingque, ActionDingque>();
            Binder<CommandFanbao, ActionFanbao>();
            Binder<CommandScoreDouble, ActionScoreDouble>();
            //lisi--新增游戏开始--start--
            Binder<CommandNewGameBegin, ActionNewGameBegin>();
            //lisi--end--
            return mSuppliers.Keys.ToList();
        }

        public virtual List<Type> CollectionC2SActionType()
        {
            return new List<Type>()
            {
                typeof(RequestCommonAction),
                typeof(RequestOperateAction),
                typeof(RequestTingAction),
            };
        }

        public virtual List<Type> AllPanelTypes()
        {
            return new List<Type>()
            {
                typeof(PanelChooseOperate),
                typeof(PanelExhibition),
                typeof(PanelGameExplain),
                typeof(PanelGameInfo),
                typeof(PanelGameRule),
                typeof(PanelGameTriggers),
                typeof(PanelGM),
                typeof(PanelHandup),
                typeof(PanelHuanAndDq),
                typeof(PanelInviteFriends),
                typeof(PanelOpreateMenu),
                typeof(PanelPlayersInfo),
                typeof(PanelQueryHuCard),
                typeof(PanelScoreDouble),
                typeof(PanelReplay),
                typeof(PanelSetting),
                typeof(PanelSingleResult),
                typeof(PanelTitleMessage),
                typeof(PanelTotalResult),
                typeof(PanelReplayResult),
                typeof(PanelZhaniao),
                typeof(PanelLiangdao),
                typeof(PanelOtherHuTip),
                typeof(PanelChooseXjfd),
                typeof(PanelShowXjfdList),
                typeof(PanelShare),                
            };
        }

        /// <summary>
        /// 默认显示panel
        /// </summary>
        public virtual List<Type> DefaultShowPanelTypes()
        {
            return new List<Type>()  {
                typeof(PanelGM),
                typeof(PanelGameInfo),
                typeof(PanelPlayersInfo),
                typeof(PanelGameTriggers),
                typeof(PanelGameRule),
            };
        }

        /// <summary>
        /// 收集回放逻辑类型
        /// </summary>
        /// <returns></returns>
        public virtual List<Type> ReplayLogicTypes()
        {
            Binder<CommandCommonReplay, ActionCommonReplay>();
            Binder<CommandLaiziReplay, ActionLaiziReplay>();
            Binder<CommandCpgReplay, ActionCpgReplay>();
            Binder<CommandHuReplay, ActionHuReplay>();
            return mSuppliers.Keys.ToList();
        }

        /// <summary>
        /// 收集回放逻辑类型
        /// </summary>
        /// <returns></returns>
        public virtual List<Type> ReplayShowPanelTypes()
        {
            return new List<Type>()
            {
                typeof(PanelReplay),
                typeof(PanelPlayersInfo)
            };
        }

        /// <summary>
        /// MahjongPlayerHand 扩展
        /// </summary>
        public virtual Type PlayerHandExtension()
        {
            return null;
        }

        /// <summary>
        /// 扩展工具收集
        /// </summary>
        public virtual List<Type> ExtensionToolCollection()
        {
            return new List<Type>()
            {
                typeof(AiAgency),
                typeof(MahjongQuery),
            };
        }
    }
}
