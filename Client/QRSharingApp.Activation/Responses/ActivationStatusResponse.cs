using System.Text.Json.Serialization;

namespace QRSharingApp.Activation.Responses
{
    public class ActivationStatusResponse
    {
        public ActivationKeyStateEnum State { get; set; }
        public string Message { get; set; }
    }
}
