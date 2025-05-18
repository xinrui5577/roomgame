using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class QjmjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandOperate, QjmjActionOperate>();
            Binder<CommandReconnect, QjmjActionReconnect>();
            return mSuppliers.Keys.ToList();
        }
    }
}
