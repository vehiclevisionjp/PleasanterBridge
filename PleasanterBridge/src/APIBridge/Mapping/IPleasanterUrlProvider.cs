namespace PleasanterBridge.src.APIBridge.Mapping
{
    public interface IPleasanterUrlProvider
    {
        public string GetItemCreateUrl(string siteId);

        public string GetBulkUpsertUrl(string siteId);

        public string GetItemUpdateUrl(string recordId);

        public string GetItemDeleteUrl(string recordId);
    }
}
