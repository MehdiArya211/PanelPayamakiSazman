using DTO.Project.User;
using DTO.User;
using Microsoft.AspNetCore.Http;

namespace Services.SessionServices
{
    /// <summary>
    /// مدیریت اطلاعات کاربر درون سشن
    /// </summary>
    public static class SessionUserManager
    {
        public static readonly string Key = "User";


        public static UserSessionDTO GetUser(this ISession session)
        {
            return session.Get<UserSessionDTO>(Key);
        }
        public static SystemUserSessionDTO GetSystemUser(this ISession session)
        {
            return session.Get<SystemUserSessionDTO>(Key);
        }


        public static void SetUser(this ISession session, SystemUserSessionDTO value)
        {
            session.Set<SystemUserSessionDTO>(Key, value);
        }



        public static void RemoveUser(this ISession session)
        {
            session.Remove(Key);
        }

    }
}
