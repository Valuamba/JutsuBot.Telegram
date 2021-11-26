using Htlv.Parser.DataAccess.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Htlv.Parser.DataAccess.EF
{
    public class StateMapper
    {
        public static void MapModelToState(TgBotFramework.State state, State model)
        {
            if (model != null)
            {
                state.CacheData = model.CacheData;
                state.Stage = model.Stage;
                state.Step = model.Step;
            }
        }
    }
}
