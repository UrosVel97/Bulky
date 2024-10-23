using Microsoft.AspNetCore.Identity.UI.Services; //Dodali smo ovaj using kako bismo mogli da koristimo interfejs 'IEmailSender'
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    public class EmailSender : IEmailSender
    {
        public string SendGridKey { get; set; } //Ovaj properti ce cuvati API Key za SendGrid

        public EmailSender(IConfiguration _conf)//Parametar tipa 'IConfiguration' nam omogucava da pristupimo JSON objektima u 'appsettings.json' fajlu
        {
            SendGridKey = _conf.GetValue<string>("SendGrid:SecretKey"); //Pristupamo propertiju 'SecretKey'. To je properti JSON objekta 'SendGrid' koji se nalazi u 'appsettings.json' fajlu
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            var client = new SendGridClient(SendGridKey); //Kreiramo SendGrid klijent objekat koji ce da komunicira sa SendGrid serverom

            var from = new EmailAddress("info@kommeta.com","Neki naslov"); //Primaocu kada stigne mejl, pisace da mu je stigao od 'info@kommeta.com'. Naslov ce biti prosledjen kao drugi parametar konstruktoru  'EmailAddress()'

            var to = new EmailAddress(email); //Podatak kome saljemo mejl ce cuvati parametar 'email'

            var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage); //Kreiramo mejl koji ce da se posalje



            return client.SendEmailAsync(message);//Saljemo mejl 
        }
    }
}
