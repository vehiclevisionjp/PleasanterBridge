namespace PleasanterBridge.src.APIBridge.Mapping
{
    public interface IPleasanterUrlProvider
    {
        public string GetItemCreateUrl(int siteId);

        public string GetBulkUpsertUrl(int siteId);

        public string GetItemUpdateUrl(int recordId);

        public string GetItemDeleteUrl(int recordId);
    }
}
