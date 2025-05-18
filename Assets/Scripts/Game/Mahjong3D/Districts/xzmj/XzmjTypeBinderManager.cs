using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class XzmjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();          
                 
            Binder<CommandHu, XzmjActionHu>();
            Binder<CommandSendCard, XzmjActionSendCard>();           
            Binder<CommandReconnect, XzmjActionReconnect>();
            Binder<CommandResponseCommon, XzmjActionCommonReponse>();
            return mSuppliers.Keys.ToList();
        }

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
