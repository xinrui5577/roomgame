using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class NamjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandTing, NamjActionTing>();
            Binder<CommandOperate, NamjActionOperate>();
            Binder<CommandReconnect, NamjActionReconnect>();
            Binder<MahjongGameData, NamjMahjongUserInfo>();
            return mSuppliers.Keys.ToList();
        }

        public override Type PlayerHandExtension()
        {
            return typeof(NamjMahjongPlayerHand);
        }
    }
}