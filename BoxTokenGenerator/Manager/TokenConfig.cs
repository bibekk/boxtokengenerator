using System;
using System.Xml.Linq;

namespace BoxTokenGenerator
{
    public class TokenConfig
    {
        public const string config_filename = "../../Auth/config.xml";
        private const string token_filename = "../../Auth/tokens.xml";
        /// <summary>
        /// Writes token to the xml file
        /// </summary>
        /// <param name="AccessToken"></param>
        /// <param name="RefreshToken"></param>
        public static void writeTokens(string AccessToken, string RefreshToken)
        {
            XDocument doc = new XDocument(
            new XElement("Tokens",
                new XElement("AccessToken", AccessToken),
                new XElement("RefreshToken", RefreshToken),
                new XElement("ExpiresIn", "3600")
                 )
                );
            doc.Save(token_filename);
        }

        /// <summary>
        /// Reads config from the config.xml file
        /// </summary>
        /// <param name="type">type can be either: ClientID , ClientSecret, RedirectURI</param>
        /// <returns></returns>
        public static string readConfig(string type)
        {
            try
            {
                XDocument doc = XDocument.Load(config_filename);
                string value = string.Empty;

                foreach (XElement el in doc.Root.Elements())
                {
                    if (el.Name.ToString().ToLower().Trim() == type.Trim().ToLower())
                        value = el.Value;
                }
                return value;
            }
            catch (Exception)
            {
               //(ex.Message);
                return "error";
            }
        }

        /// <summary>
        /// Read tokens from tokens.xml file
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string readToken(string type)
        {
            XDocument doc = XDocument.Load(token_filename);
            string value = string.Empty;

            foreach (XElement el in doc.Root.Elements())
            {
                if (el.Name.ToString().ToLower().Trim() == type.Trim().ToLower())
                    value = el.Value;
            }
            return value;
        }

    }
}
