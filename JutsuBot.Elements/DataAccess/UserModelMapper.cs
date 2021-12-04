using CliverBot.Console.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TgBotFramework;

namespace Htlv.Parser.DataAccess.EF
{
    public static class UserModelMapper
    {
        public static void MapModelToState(IUserState state, User model)
        {
            state.Role = model.Role;
            //StateMapper.MapModelToState(state.CurrentState, model.CurrentState);
            //StateMapper.MapModelToState(state.PrevState, model.PrevState);
            state.LanguageCode = model.LanguageCode;
            state.PhoneNumber = model.PhoneNumber;
            state.FullName = model.FullName;
            state.IsAuthorized = model.IsAuthorized;
            state.IsBotStopped = model.IsBotStopped;
        }

        public static bool MapStateToModel(IUserState contextUserState, User userDbObject)
        {
            bool result = false;
            if (userDbObject == null)
            {
                return false;
            }

            if (contextUserState.Role != userDbObject.Role)
            {
                userDbObject.Role = contextUserState.Role;
                result = true;
            }

            //CurrentState
            //if(contextUserState.CurrentState?.Step != userDbObject.CurrentState?.Step)
            //{
            //    userDbObject.CurrentState.Step = contextUserState.CurrentState.Step;
            //    result = true;
            //}

            //if (contextUserState.CurrentState?.Stage != userDbObject.CurrentState?.Stage)
            //{
            //    userDbObject.CurrentState.Stage = contextUserState.CurrentState.Stage;
            //    result = true;
            //}

            //if (contextUserState.CurrentState?.CacheData != userDbObject.CurrentState?.CacheData)
            //{
            //    userDbObject.CurrentState.CacheData = contextUserState.CurrentState.CacheData;
            //    result = true;
            //}

            //PrevState
            //if (contextUserState.PrevState?.Step != userDbObject.PrevState?.Step)
            //{
            //    userDbObject.PrevState.Step = contextUserState.PrevState.Step;
            //    result = true;
            //}

            //if (contextUserState.PrevState?.Stage != userDbObject.PrevState?.Stage)
            //{
            //    userDbObject.PrevState.Stage = contextUserState.PrevState.Stage;
            //    result = true;
            //}

            //if (contextUserState.PrevState?.CacheData != userDbObject.PrevState?.CacheData)
            //{
            //    userDbObject.PrevState.CacheData = contextUserState.PrevState.CacheData;
            //    result = true;
            //}

            if (contextUserState.LanguageCode != userDbObject.LanguageCode)
            {
                userDbObject.LanguageCode = contextUserState.LanguageCode;
                result = true;
            }

            if (contextUserState.FullName != userDbObject.FullName)
            {
                userDbObject.FullName = contextUserState.FullName;
                result = true;
            }

            if (contextUserState.PhoneNumber != userDbObject.PhoneNumber)
            {
                userDbObject.PhoneNumber = contextUserState.PhoneNumber;
                result = true;
            }

            if (contextUserState.IsAuthorized != userDbObject.IsAuthorized)
            {
                userDbObject.IsAuthorized = contextUserState.IsAuthorized;
                result = true;
            }

            if (contextUserState.IsBotStopped != userDbObject.IsBotStopped)
            {
                userDbObject.IsBotStopped = contextUserState.IsBotStopped;
                result = true;
            }

            return result;
        }
    }
}
