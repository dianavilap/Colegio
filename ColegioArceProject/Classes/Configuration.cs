using System;

namespace ColegioArceProject.Classes
{
    public static class Configuration
    {

        #region Site configurations

        public static String CannonicalSiteUrl = "http://colegioarce.dsourcecode.com/";

        public static String MessageMagicString = "Message";
        public static String AlertMagicString = "AlertClass";
        #endregion

        #region Cookies
        public static String TempDataCookie = "__iddrassillizaitev__";
        public static Int32 TempDataCookieLifeTime = 60 * 5;

        public static String UserCookie = "__U-Cookie__";
        public static Int32 CookieLifeTime = 60 * 60 * 1;

        #endregion
    }
}