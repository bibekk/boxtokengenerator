
namespace BoxTokenGenerator
{
    /// <summary>
    /// Constants for Box Auth API. reads config from the config.xml file
    /// </summary>
    public static class BoxManagerConst
    {
        public const string BoxUri = "https://api.box.com/oauth2/token";
        public static string client_id = TokenConfig.readConfig("ClientID");
        public static string client_secret = TokenConfig.readConfig("ClientSecret");
        public const string authurl = "https://app.box.com/api/oauth2/authorize?response_type=code";
        public static string redirecturi = TokenConfig.readConfig("RedirectURI");
    }
}
