using CartonCaps.MessageTemplate;

namespace CartonCaps.IServices;

public interface ITemplateService
{
    SharedMessageTemplate CreateEmail(string downloadUrl);
    SharedMessageTemplate CreateSms(string downloadUrl);
}
