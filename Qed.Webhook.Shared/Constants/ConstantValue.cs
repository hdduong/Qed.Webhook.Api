namespace Qed.Webhook.Api.Shared.Constants
{
    public class ConstantValue
    {
        public const int DefaultPaginationOffset = 0;
        public const int DefaultPaginationLimit = 50;
        public const int DefaultWorkerId = 1;
        public const int DefaultDownloadBufferSizeBytes = 1048576; // 1Mb
        public const int LimitBytesFlushToDisk = 4194304; // 4Mb
        public const int DefaultSagaTimeoutInSeconds = 600;
        public const int MaximumSagaTimeoutInMinutes = 15;
    }
}
