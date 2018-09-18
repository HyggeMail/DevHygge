using System.Net;
using System.Net.Mail;
using System.Configuration;
using System;
using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Managers;
using HyggeMail.BLL.Common;
public static class EmailClient
{
    private readonly static SmtpClient client;
    static EmailClient()
    {
        client = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 25,
            EnableSsl = true
        };
        client.Credentials = new System.Net.NetworkCredential
        {
            UserName = "demoparkingxt@gmail.com",
            Password = "d4Developer"
        };
    }
    public static bool Send(MailMessage msg)
    {
        string errormsg;
        try
        {
          //  msg.From = new MailAddress(Config.Email, Config.AppName);
            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            //client.Credentials = new NetworkCredential(Config.Email, Config.Password);
            client.Send(msg);
        }
        catch (Exception ex)
        {
            errormsg = ex.Message;
            IErrorLogManager _errorManager = new ErrorLogManager();
            _errorManager.LogStringExceptionToDatabase(errormsg);
            return false;
        }
        return true;
    }
}

