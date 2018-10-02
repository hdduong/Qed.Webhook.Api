namespace Qed.Webhook.Service.Models.Responses.Event
{
    public class AddEncompassEventResponse
    {
        public int EventId { get; set; }
        public bool IsSuccessBit { get; set; }
        public string ErrorMsg { get; set; }

        public AddEncompassEventResponse()
        {
            EventId = 0;
            IsSuccessBit = true;
            ErrorMsg = string.Empty;
        }
    }
}
