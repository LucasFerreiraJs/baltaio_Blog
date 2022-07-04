namespace Blog
{
    public static class Configuration
    {

        public static string JwtKey { get; set; } = "";
        public static string ApiKeyName = "api_key";
        public static string ApiKey = "";
        public static SmtpConfiguration Smtp = new SmtpConfiguration();


        // config email

        public class SmtpConfiguration
        {

            public string Host { get; set; }
            public int Port { get; set; } = 00;
            public string UserName { get; set; }
            public string Password { get; set; }

        }


    }
}
