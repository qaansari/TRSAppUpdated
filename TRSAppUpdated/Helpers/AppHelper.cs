using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Net.Mail;
using System.Text;

namespace TRSAppUpdated.Helpers
{
    public static class AppHelper
    {
#if DEBUG
        public static string _appUrl { get; set; } = "https://localhost:44358/";
#else
        public static string _appUrl { get; set; } = "https://www.eventland.pk/";
#endif
        //Jazzcash Params Start
        public static string _MerchantID { get; set; } = "YOUR_MERCHANT_ID";
        public static string _Password { get; set; } = "YOUR_PASSWORD";
        public static string _IntegritySalt { get; set; } = "YOUR_INTEGRITY_SALT";
        public static string _ReturnURL { get; set; } = _appUrl + "Reservation/Success";
        public static string _SandboxURL { get; set; } = "https://sandbox.jazzcash.com.pk/ApplicationAPI/API/Payment/DoTransaction";
        public static string _ProductionURL { get; set; } = "https://jazzcash.com.pk/ApplicationAPI/API/Payment/DoTransaction";
        //Jazzcash Params End

        public static void ConfigureSettings(IConfiguration configuration)
        {
            var appUrl = configuration["AppUrl"];
            if (!string.IsNullOrEmpty(appUrl))
            {
                _appUrl = appUrl.EndsWith("/") ? appUrl : appUrl + "/";
            }
            var jazzCash = configuration.GetSection("JazzCash");
            if (jazzCash.Exists())
            {
                _MerchantID = jazzCash["MerchantID"] ?? _MerchantID;
                _Password = jazzCash["Password"] ?? _Password;
                _IntegritySalt = jazzCash["IntegritySalt"] ?? _IntegritySalt;
                _ReturnURL = jazzCash["ReturnURL"] ?? (_appUrl + "Reservation/Success");
                _SandboxURL = jazzCash["SandboxURL"] ?? _SandboxURL;
                _ProductionURL = jazzCash["ProductionURL"] ?? _ProductionURL;
            }
        }
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
        public static DateTime NowPakistan => DateTime.UtcNow.AddHours(5);

        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        public static bool IsValidImageFile(IFormFile? file, out string? errorMessage, long maxSizeBytes = 5 * 1024 * 1024)
        {
            errorMessage = null;
            if (file == null || file.Length == 0)
            {
                return true;
            }

            if (file.Length > maxSizeBytes)
            {
                errorMessage = $"File size exceeds maximum allowed limit of {maxSizeBytes / (1024 * 1024)}MB.";
                return false;
            }

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext) || !AllowedImageExtensions.Contains(ext))
            {
                errorMessage = "Invalid file type. Allowed extensions are: .jpg, .jpeg, .png, .webp";
                return false;
            }

            if (!string.IsNullOrEmpty(file.ContentType) && !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Invalid content type. Only image files are allowed.";
                return false;
            }

            return true;
        }

        public static async Task<string> SaveUploadedImageAsync(IFormFile file, string webRootPath, string subFolder, string prefix)
        {
            var folderPath = Path.Combine(webRootPath, "assets", "images", subFolder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !AllowedImageExtensions.Contains(ext))
            {
                ext = ".jpg";
            }

            var fileName = $"{prefix}_{NowPakistan:yyyyddMMhhmmss}_{Guid.NewGuid().ToString("N")[..6]}{ext}";
            var filePath = Path.Combine(folderPath, fileName);

            var compressedBytes = await CompressImageAsync(file);
            await System.IO.File.WriteAllBytesAsync(filePath, compressedBytes);

            return fileName;
        }

        //Send Password Reset Email
        public static async Task SendVerificationEmailAsync(string emailAddress, string verificationType, string verificationLink, IConfiguration? configuration = null)
        {
            // Configure email settings
            string _senderEmail = configuration?["EmailSettings:SenderEmail"] ?? "eventlandinformation@gmail.com";
            string _senderPassword = configuration?["EmailSettings:Password"] ?? string.Empty;
            if (string.IsNullOrEmpty(_senderPassword))
            {
                throw new InvalidOperationException("Email settings are not properly configured in application settings.");
            }

            string _smtpHost = configuration?["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            int _smtpPort = int.TryParse(configuration?["EmailSettings:Port"], out var port) ? port : 587;
            string _senderName = configuration?["EmailSettings:SenderName"] ?? "Event Land";
            string _logoUrl = configuration?["EmailSettings:LogoUrl"] ?? "https://ik.imagekit.io/mg7bre0yk/web-logo.png";
            string _subject = verificationType.ToLower() == "verify email" ? "Email Verification" : "Reset Password";
            string _btnText = verificationType.ToLower() == "verify email" ? "Login To Continue" : "Reset Password";
            string _paraText = verificationType.ToLower() == "verify email" ? "verify your email" : "reset your password";
            string _safeVerificationLink = System.Text.Encodings.Web.HtmlEncoder.Default.Encode(verificationLink);
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
        .logo{{
            width: 250px;
            height:150px;
        }}
        .header {{
            text-align: center;
            margin-bottom: 10px;
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
        .para_text{{
        font-weight:600;
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
        <p>Please click the button below to <span class='para_text'>{_paraText}</span></p>
        <div class='button-container'>
            <a href='{_safeVerificationLink}' class='button'>{_btnText}</a>
        </div>
    </div>
</body>
</html>
";
            using var message = new MailMessage();
            message.From = new MailAddress(_senderEmail, _senderName);
            message.To.Add(new MailAddress(emailAddress));
            message.Subject = _subject;
            message.Body = _body;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(_senderEmail, _senderPassword);
            smtpClient.EnableSsl = true;
            await smtpClient.SendMailAsync(message);
        }

        //Compress Size
        public static async Task<byte[]> CompressImageAsync(IFormFile imageFile, int maxFileSizeInBytes = 1 * 1024 * 1024)
        {
            using (var inputStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(inputStream);

                // If already under size limit, return original
                if (inputStream.Length <= maxFileSizeInBytes)
                    return inputStream.ToArray();

                inputStream.Position = 0;

                int quality = 75;
                using (var image = await Image.LoadAsync(inputStream))
                {
                    var encoder = new JpegEncoder
                    {
                        Quality = quality
                    };

                    byte[] compressedBytes;

                    using (var outputStream = new MemoryStream())
                    {
                        do
                        {
                            outputStream.SetLength(0); // clear stream
                            await image.SaveAsJpegAsync(outputStream, encoder);
                            compressedBytes = outputStream.ToArray();
                            quality -= 5;

                        } while (compressedBytes.Length > maxFileSizeInBytes && quality > 30);
                    }
                    return compressedBytes;
                }
            }
        }
    }

}
