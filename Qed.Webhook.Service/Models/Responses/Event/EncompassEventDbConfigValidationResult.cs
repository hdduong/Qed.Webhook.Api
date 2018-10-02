namespace Qed.Webhook.Service.Models.Responses.Event
{
    public class EncompassEventDbConfigValidationResult
    {
        public AddEncompassEventResponse EncompassEventResponse { get; set; }
        public DictionaryId EncompassSource { get; set; }
        public DictionaryId EventStatusNotStart { get; set; }
        public DictionaryId EventStatusNotSupport { get; set; }
        public DictionaryId EventType { get; set; }
        public DictionaryId ResourceType { get; set; }

        public EncompassEventDbConfigValidationResult()
        {
            EncompassEventResponse = new AddEncompassEventResponse();
            EncompassSource = new DictionaryId();
            EventStatusNotStart = new DictionaryId();
            EventStatusNotSupport = new DictionaryId();
            EventType = new DictionaryId();
            ResourceType = new DictionaryId();
        }
    }
}
