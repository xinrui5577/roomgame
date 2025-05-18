using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class WmbbmjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandCpg, WmbbmjActionCpg>();
            Binder<CommandReconnect, WmbbmjActionReconnect>();
            Binder<CommandThrowoutCard, WmbbmjActionThrowoutCard>();
            return mSuppliers.Keys.ToList();
        }
    }
}