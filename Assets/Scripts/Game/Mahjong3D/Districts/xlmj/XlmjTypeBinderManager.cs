using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class XlmjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();           
            
            Binder<CommandHu, XlmjActionHu>();
            Binder<CommandSendCard, XzmjActionSendCard>();
            Binder<CommandReconnect, XzmjActionReconnect>();
            Binder<CommandReconnect, XlmjActionReconnect>();
            return mSuppliers.Keys.ToList();
        }

        /// <summary>
        /// 默认显示panel
        /// </summary>
        public override List<Type> DefaultShowPanelTypes()
        {
            var list = base.DefaultShowPanelTypes();
            list.Add(typeof(PanelHuanAndDq));
            return list;
        }

        public override Type PlayerHandExtension()
        {
            return typeof(XzmjMahjongPlayerHand);
        }
    }
}
