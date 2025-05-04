using CartonCaps.IServices;
using CartonCaps.MessageTemplate;
using HandlebarsDotNet;

namespace CartonCaps.Services;

public class TemplateService : ITemplateService
{
    private readonly IHandlebars _handlebars;
    private readonly string _templateDirectory;

    public TemplateService(string templateDirectory)
    {
        _templateDirectory = templateDirectory;
        _handlebars = Handlebars.Create();
        RegisterTemplates();
    }

    private void RegisterTemplates()
    {
        RegisterTemplate("EmailTemplate");
        RegisterTemplate("SmsTemplate");
        RegisterTemplate("EmailSubjectTemplate");
    }

    private void RegisterTemplate(string templateName)
    {
        var templatePath = Path.Combine(_templateDirectory, $"{templateName}.hbs");
        var templateContent = File.ReadAllText(templatePath);
        _handlebars.RegisterTemplate(templateName, templateContent);
    }

    public SharedMessageTemplate CreateEmail(string downloadUrl)
    {
        var bodyWriter = new StringWriter();
        var subjectWriter = new StringWriter();

        var bodyTemplate = _handlebars.Configuration.RegisteredTemplates["EmailTemplate"];
        var subjectTemplate = _handlebars.Configuration.RegisteredTemplates["EmailSubjectTemplate"];

        bodyTemplate(bodyWriter, new { downloadUrl });
        subjectTemplate(subjectWriter, new { });

        var body = bodyWriter.ToString();
        var subject = subjectWriter.ToString();

        return new SharedMessageTemplate(subject: subject, body: body);
    }

    public SharedMessageTemplate CreateSms(string downloadUrl)
    {
        var writer = new StringWriter();
        var template = _handlebars.Configuration.RegisteredTemplates["SmsTemplate"];

        template(writer, new { downloadUrl });

        return new SharedMessageTemplate(subject: null, body: writer.ToString());
    }
}
