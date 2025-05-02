namespace CartonCaps.MessageTemplate
{
    public class Sms(string downloadUrl)
    {
        private readonly string _downloadUrl = downloadUrl;

        public string Message()
        {
            return $@"
        Hi, Join me in earning money for our school by using the Carton Caps app. It's an easy way to make a difference. Use the link below to download the Carton Caps app: {_downloadUrl}";
        }
    }
}
