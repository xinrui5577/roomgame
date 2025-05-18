using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class HzlzgTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandCpg, HzlzgActionCpg>();
            Binder<CommandOperate, QjmjActionOperate>();
            Binder<CommandReconnect, QjmjActionReconnect>();
            return mSuppliers.Keys.ToList();
        }
    }
}

