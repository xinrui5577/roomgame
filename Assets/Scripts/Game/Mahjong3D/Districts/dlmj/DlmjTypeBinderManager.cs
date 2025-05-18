using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class DlmjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandTing, DlmjActionTing>();
            Binder<CommandOperate, DlmjActionOperate>();
            Binder<CommandReconnect, DlmjActionReconnect>();
            return mSuppliers.Keys.ToList();
        }

        public override Type PlayerHandExtension()
        {
            return typeof(DlmjMahjongPlayerHand);
        }
    }
}