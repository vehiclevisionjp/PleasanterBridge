namespace PleasanterBridge.src.APIBridge.Mapping
{
    internal class PleasanterUrlProvider : IPleasanterUrlProvider
    {
        private readonly string _pleasanterUrlBase;

        public PleasanterUrlProvider(string pleasanterUrlBase)
        {
            _pleasanterUrlBase = pleasanterUrlBase.TrimEnd('/');
        }

        public string GetItemCreateUrl(string siteId)
        {
            return $"{_pleasanterUrlBase}/api/items/{siteId}/create";
        }

        public string GetBulkUpsertUrl(string siteId)
        {
            return $"{_pleasanterUrlBase}/api/items/{siteId}/bulkupsert";
        }

        public string GetItemUpdateUrl(string recordId)
        {
            return $"{_pleasanterUrlBase}/api/items/{recordId}/update";
        }

        public string GetItemDeleteUrl(string recordId)
        {
            return $"{_pleasanterUrlBase}/api/items/{recordId}/delete";
        }
    }
}
