namespace QRSharingApp.ActivationWebApp.ApiResponses
{
    public class ActivationStatusResponse
    {
        public ActivationKeyStateEnum State { get; set; }
        public string Message { get; set; }
    }
}
