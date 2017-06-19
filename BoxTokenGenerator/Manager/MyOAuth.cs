using Box.V2.Auth;

namespace BoxTokenGenerator
{
    public class MyOAuth
    {
        public static OAuthSession getSession()
        {
            OAuthSession session = new OAuthSession(TokenConfig.readToken("AccessToken"), TokenConfig.readToken("RefreshToken"), int.Parse(TokenConfig.readToken("ExpiresIn")), "Bearer");
            return session;
        }
    }
}
