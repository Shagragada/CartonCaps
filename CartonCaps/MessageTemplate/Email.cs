namespace CartonCaps.MessageTemplate;

public class Email(string downloadUrl)
{
    private readonly string _downloadUrl = downloadUrl;

    public string Subject { get; } = "Earn Cash for Our School with Carton Caps!";

    public string Body()
    {
        return @$"
        <html>
        <body>
            Hey!
            
            Join me in earning cash for our school by using the CartonCaps app. It's an easy way to make a difference. All you have to do is buy Carton Caps participating products (like Cheerios!) and scan your grocery receipt. Carton Caps are worth $.10 each and they add up fast! Twice a year, our school receives a check to help pay for whatever we need - equipment, supplies or experiences the kids love!
            Download the Carton Caps app here: <a href='{_downloadUrl}'>{_downloadUrl}</a>
        </body>
        </html>";
    }
}
