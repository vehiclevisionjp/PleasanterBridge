namespace PleasanterBridge.src.APIBridge.Mapping
{
    public interface IPleasanterUrlProvider
    {
        public string GetItemCreateUrl(long siteId);

        public string GetBulkUpsertUrl(long siteId);

        public string GetItemUpdateUrl(long recordId);

        public string GetItemDeleteUrl(long recordId);
    }
}
