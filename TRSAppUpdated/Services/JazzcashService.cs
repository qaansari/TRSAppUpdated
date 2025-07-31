using System.Security.Cryptography;
using System.Text;
using TRSAppUpdated.Helpers;
using TRSAppUpdated.Interfaces;
using TRSAppUpdated.Models;
using TRSAppUpdated.ViewModels;

namespace TRSAppUpdated.Services
{
    public class JazzcashService : IJazzcashService
    {
        private readonly ILogger<JazzcashService> _logger;
        private readonly string _Title = "Jazzcash";
        public JazzcashService(AppDbContext context, ILogger<JazzcashService> logger)
        {
            _logger = logger;
        }
        //Create Payment Request
        public JazzcashPaymentRequestVM CreatePaymentRequest(PaymentMethodRequestVM Model)
        {
            try
            {
                var _request = new JazzcashPaymentRequestVM()
                {
                    pp_MerchantID = AppHelper._MerchantID,
                    pp_Password = AppHelper._Password,
                    pp_TxnRefNo = $"T{DateTime.UtcNow.AddHours(5).ToString("yyyyMMddHHmmss")}",
                    pp_Amount = Model.Amount.ToString("0"),
                    pp_TxnDateTime = DateTime.UtcNow.AddHours(5).ToString("yyyyMMddHHmmss"),
                    pp_BillReference = Model.TicketNo.ToString(),
                    pp_Description = Model.Description,
                    pp_ReturnURL = AppHelper._ReturnURL
                };
                _request.pp_SecureHash = GenerateSecureHash(_request);
                return _request;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at CreatePaymentRequest in {_Title} Service due to {ex}");
            }
            return new JazzcashPaymentRequestVM();
        }
        //Generate Secure Hash
        private string GenerateSecureHash(JazzcashPaymentRequestVM Model)
        {
            try
            {
                var _properties = Model.GetType().GetProperties().Where(x => x.Name != "pp_SecureHash"
                    && x.GetValue(Model) != null).OrderBy(x => x.Name).Select(x => $"{x.GetValue(Model)}");
                string _concatenatedString = string.Join("&", _properties);
                string _integritySalt = AppHelper._IntegritySalt;
                using (var _hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_integritySalt)))
                {
                    byte[] _hashBytes = _hmac.ComputeHash(Encoding.UTF8.GetBytes(_concatenatedString));
                    return BitConverter.ToString(_hashBytes).Replace("-", "").ToLower();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured at GenerateSecureHash in {_Title} Service due to {ex}");
            }
            return string.Empty;
        }
    }
}
