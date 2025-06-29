namespace FastKMSApi.Core.Model
{
    public class JwtModel
    {
        public string key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double Expires { get; set; }
    }

    public class JwtCheck
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
