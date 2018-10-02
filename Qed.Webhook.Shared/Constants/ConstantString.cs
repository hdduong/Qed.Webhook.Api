namespace Qed.Webhook.Api.Shared.Constants
{
    public class ConstantString
    {
        // common
        public static string EmptyConfiguration = "Configuration for {0} is empty";
        public static string DefaultEncompassSdkName = "Correspondent";
        public static string Exception = "Exception";

        // repository
        public static string StagingKeyConfig = "Database:ConnectionString:Staging";
        public static string MagnusKeyConfig = "Database:ConnectionString:Magnus";

        // service
        public static string EventSourceCacheKey = "EventSrc";
        public static string EventStatusCacheKey = "EventStatus";
        public static string EventTypeCacheKey = "EventType";
        public static string ResourceTypeCacheKey = "ResourceType";
        public static string DocumentStatusKey = "DocumentStatus";

        // service token
        public static string GrantTypeKey = "grant_type";
        public static string GrantTypeValue = "password";
        public static string SmartClientUserKey = "username";
        public static string SmartClientUserValue = "{0}@encompass:{1}";
        public static string SmartClientPasswordKey = "password";
        public static string ClientIdKey = "client_id";
        public static string ClientSecretKey = "client_secret";

        // service headers for business request
        public const string JsonContentTypeValue = "application/json";
        public static string AuthorizationKey = "Authorization";
        public static string AuthorizationValue = "Bearer {0}";
        // service uri endpoint
        public static string OAuthTokenSuffix = "/oauth2/v1/token";
        public static string SubscriptionSuffix = "/webhook/v1/subscriptions";
        public static string AttachmentSuffix = "/encompass/v1/loans/{0}/attachments";
        public static string AttchmentUrlSuffix = "/encompass/v1/loans/{0}/attachments/{1}/url";

        // service exception
        public static string EncompassSdkUnavailableConfig = "EncompassSdk configuration is missing for {0}";
        public static string ErrorWhileGettingAttachments = "Error while getting attachments {0}";

        // api
        public static string DefaultNLogConfigFileName = "nlog.config";
        public static string AspNetCoreEnvVarName = "ASPNETCORE_ENVIRONMENT";
        public static string EncompassSdkCacheKey = "EncSdk";
        public static string EncompassSubscriptionKeyConfig = "Encompass:Subscriptions";
        public static string EnumValueNotFound = "Enum values for {0} has not been set corectly. Missing value is {1}";

        // web api
        public static string ApiKeyConfig = "Encompass:Endpoint";
        public static string RedisApiConfig = "Amerihome:EncompassRedisCacheApi:Endpoint";
        public static string ApiProjectName = "Qed.Encompass.Webhook.Api";
        public static string WebApiProjectName = "Qed.Encompass.Webhook.WebApi";
        public static string RedisCacheApiProjectName = "Qed.Encompass.Webhook.RedisCache.Api";
        public static string AmeriHomeEndpointKeyConfig = "AmeriHome:Endpoint";
        public static string EncompassWebhookSecretKeyConfig = "Encompass:WebhookSecretKey";
        public static string EncompassUriKeyConfig = "AmeriHome:EncompassUri";
        public static string EncompassWebApiUnavailableConfig = "Configuration is missing for {0}";
        public static string EllieSignatureRequired = "elli-signature is empty in the request";
        public static string EllieSignatureNotMatched = "elli-signature does not match with hashed body string, body {0}, elli-signature {1}";
        public static string EncompassWebApiAuthCacheKey = "EncWebApiAuth";
        public static string EllieSignatureHeaderKey = "Elli-Signature";

        // web api redundancy
        public static string WebApiRedundancyProjectName = "Qed.Encompass.Redundancy.WebApi";
        public static string EncompassEventDynamoDbTableNameKey = "DynamoDb:TableName";
        public static string DynamoDbUnixDtTmColumn = "eventUnixDtTm";
        public static string DynamoDbEventIdColumn = "eventId";
        public static string DynamoDbMsgTxtColumn = "msgTxt";
        public static string DynamoDbUnixDtIndex = "eventUnixDtTm-index";
        public static string DynamoDbResourceidUnixDtTmIndex = "resourceId-eventUnixDtTm-index";
        public static string RedundancyApiInvalidEmptyRequest = "Either EventId, ResourceId, StartDtTm or EndDtTm is needed";
        public static string RedundancyApiInvalidOffset = "Invalid Offset";

        // web api endpoint
        public const string AmeriHomeEncompassUri = "/api/encompass";
        public static string AbsoluteStartupPath = "/";
        public const string EncompassEventUri = "/api/encompassevent";
        public const string RedisCacheUri = "/api/rediscache";
        public const string RedisCacheWorkerUri = "resources/{resourceId}/worker";
        public const string ReplacingResourceId = "{resourceId}";
        public const string RedisCacheDocumentUri = "resources/{resourceId}/attachments";

        // httpClient name 
        public const string EncompassApiHttpClientName = "EncompassApi";
        public const string RedisCacheApiHttpClientName = "RedisCacheApi";
        public const string HttpClientInputParameterName = "httpClient";

        // redisCache Api
        public const string NumberOfWorkerConfig = "NumberOfWorkers";
        public const string LastAssignedWorkerId = "LastAssignedWorkerId";
        public const string RedisServerAddressConfig = "Redis:ServerAddress";
        public const string DefaultWorkerId = "1";

        // jobpicker worker
        public const string WindowsServiceText = "services";
        public const string JobMasterIdConfig = "Amerihome:Master:Id";
        public const string ProdEnv = "Prod";
        public const string NoEventInQueue = "There is no event waiting to process";
        public const string CorrelationIdKey = "CorrelationId";

        // document worker
        public const string DownloadPathConfig = "DocumentWorker:DownloadPath";
        public static string TimeoutConfigKey = "DocumentWorker:TimeoutSeconds";

        public const string AllDocumentDowloaded = "All documents have been download succesfully.";
        public const string CacheIssueAllDocumentDowloaded = "All documents have been download succesfully but there is issue while sync up with cache.";
        public static string JobMasterEndpoint = "JobMasterEndpoint";
        public static string DocumentWorkerEndpoint = "DocumentWorkerEndpoint";
        public static string ErrorTableName = "EncompassWebhookError";
        public static string AuditTableName = "EncompassWebhookAudit";
        public static string SagaEndpoint = "EncompassWebhookSagaEndpoint";
        public const string BusSchema = "Bus";
        public const string PersistenceDocumentPrefix = "DocumentWorker";
        public const string PersistenceSagaPrefix = "Saga";
    }
}

