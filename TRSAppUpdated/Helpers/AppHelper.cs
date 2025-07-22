using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Net.Mail;
using System.Text;

namespace TRSAppUpdated.Helpers
{
    public static class AppHelper
    {
#if DEBUG
        public static string _appUrl { get; set; } = "https://localhost:44358/";
#else
        public static string _appUrl { get; set; } = "https://localhost:44358/";
#endif
        public static List<string> Cities = new List<string> { "Karachi","Lahore","Islamabad","Peshawar","Quetta" };
        public static string HashWithSalt(this string password)
        {
            string saltstring = "CHYzqeN4plZekNC88Umm1Q==";
            // Generate a 128-bit salt using a sequence of
            // cryptographically strong random bytes.
            byte[] salt = Encoding.ASCII.GetBytes(saltstring);

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
        //Send Password Reset Email
        public static void SendVerificationEmail(string emailAddress,string verificationType, string verificationLink)
        {
            // Configure email settings
            string _senderEmail = "eventland28@gmail.com";
            string _senderName = "Event Land ";
            string _logoUrl = "https://i.ibb.co/9kmnH46B/web-logo.png";
            string _subject = verificationType.ToLower()=="verify email"?"Email Verification":"Reset Password";
            string _body = $@"
<html>
<head>
    <style>
        /* CSS styles for the email */
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #212439;
            font-family: Arial, sans-serif;
            color: #333;
        }}

        .header {{
            text-align: center;
            margin-bottom: 30px;
            color:#fff;
        }}


        .button-container {{
            text-align: center;
        }}

        .button {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff !important;
            text-decoration: none !important;
            border-radius: 4px;
            font-weight: bold;
        }}

        .button:hover {{
            background-color: #0056b3;
        }}

        p {{
            font-weight: 500;
            text-align: center;
            color:#fff;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
        <img src='{_logoUrl}' alt='Logo' class='logo'>
            <h1>{_subject}</h1>
        </div>
        <p>Please click the button below to verify your email:</p>
        <div class='button-container'>
            <a href='{verificationLink}' class='button'>Login To Continue</a>
        </div>
    </div>
</body>
</html>
";
            // Create and configure the email message
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_senderEmail, _senderName);
            message.To.Add(new MailAddress(emailAddress));
            message.Subject = _subject;
            message.Body = _body;
            message.IsBodyHtml = true;

            // Configure the SMTP client
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(_senderEmail, "qldgvfcxlvferbjn");
                smtpClient.EnableSsl = true;
                // Send the email
                smtpClient.Send(message);
            }
        }
    }
}
