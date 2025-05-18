using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class YkmjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandReconnect, YkmjActionReconnect>();
            Binder<CommandResponseCommon, YkmjActionResponseCommon>();
            return mSuppliers.Keys.ToList();
        }
    }
}
