using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliverBot.Console.DataAccess
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
                state.StatePriority = model.StatePriority;
            }
        }

        public static bool MapFrameworkStateToEntityState(TgBotFramework.State state, State entityState)
        {
            bool result = false;
            if (entityState == null)
            {
                return false;
            }

            if(entityState.Stage != state.Stage)
            {
                entityState.Stage = state.Stage;
                result = true;
            }

            if (entityState.Step != state.Step)
            {
                entityState.Step = state.Step;
                result = true;
            }

            if (entityState.StatePriority != state.StatePriority)
            {
                entityState.StatePriority = state.StatePriority;
                result = true;
            }

            if (entityState.CacheData != state.CacheData)
            {
                entityState.CacheData = state.CacheData;
                result = true;
            }

            if (entityState.MessageId != state.MessageId)
            {
                entityState.MessageId = state.MessageId;
                result = true;
            }

            return result;
        }
    }
}
