namespace AgileRap_Process2
{
    public interface IEmailSender
    {
        //Task SendEmailAsync(string email, string subject, string message);
        void SendEmail(List<string> toList, string subject, string body);
    }
}
