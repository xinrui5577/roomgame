using System.Collections.Generic;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class BbmjTypeBinderManager : TypeBinderManager
    {
        public override List<Type> CollectionGameLogicType()
        {
            base.CollectionGameLogicType();

            Binder<CommandHu, BbmjActionHu>();
            Binder<CommandCpg, BbmjActionCpg>();
            Binder<CommandTing, BbmjActionTing>();
            Binder<CommandOperate, BbmjActionOperate>();
            Binder<MahjongGameData, BbmjMahjongUserInfo>();
            return mSuppliers.Keys.ToList();
        }

        public override Type PlayerHandExtension()
        {
            return typeof(BbmjMahjongPlayerHand);
        }
    }
}