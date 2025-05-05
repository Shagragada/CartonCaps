using CartonCaps.MessageTemplate;

namespace CartonCaps.IServices;

public interface IMessageTemplateService
{
    SharedMessageTemplate CreateEmail(string downloadUrl);
    SharedMessageTemplate CreateSms(string downloadUrl);
}
