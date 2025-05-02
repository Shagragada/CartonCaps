namespace CartonCaps.MessageTemplate;

public class SharedMessageTemplate
{
    public string? Subject { get; init; }
    public string Body { get; init; }

    public SharedMessageTemplate(string? subject, string body)
    {
        Subject = subject;
        Body = body;
    }
}
