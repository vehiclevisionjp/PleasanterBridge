namespace PleasanterBridge.src.APIBridge.Mapping
{
    internal class PleasanterUrlProvider : IPleasanterUrlProvider
    {
        private readonly string _pleasanterUrlBase;

        public PleasanterUrlProvider(string pleasanterUrlBase)
        {
            _pleasanterUrlBase = pleasanterUrlBase.TrimEnd('/');
        }

        public string GetItemCreateUrl(int siteId)
        {
            return $"{_pleasanterUrlBase}/api/items/{siteId}/create";
        }

        public string GetBulkUpsertUrl(int siteId)
        {
            return $"{_pleasanterUrlBase}/api/items/{siteId}/bulkupsert";
        }

        public string GetItemUpdateUrl(int recordId)
        {
            return $"{_pleasanterUrlBase}/api/items/{recordId}/update";
        }

        public string GetItemDeleteUrl(int recordId)
        {
            return $"{_pleasanterUrlBase}/api/items/{recordId}/delete";
        }
    }
}
