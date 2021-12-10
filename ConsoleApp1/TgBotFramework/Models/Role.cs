using System;

namespace TgBotFramework
{
    [Flags]
    public enum Role
    {
        Visitor = 0,
        SendGrid = 2,
        Manager = 4,
        Admin = 6
    }
}