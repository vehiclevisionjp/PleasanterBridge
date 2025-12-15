namespace PleasanterBridge.src.APIBridge.Mapping
{
    internal class PleasanterUrlProvider : IPleasanterUrlProvider
    {
        private readonly string _url;

        public PleasanterUrlProvider(string url)
        {
            _url = url.TrimEnd('/');
        }

        public string GetItemCreateUrl(long siteId)
        {
            return $"{_url}/api/items/{siteId}/create";
        }

        public string GetBulkUpsertUrl(long siteId)
        {
            return $"{_url}/api/items/{siteId}/bulkupsert";
        }

        public string GetItemUpdateUrl(long recordId)
        {
            return $"{_url}/api/items/{recordId}/update";
        }

        public string GetItemDeleteUrl(long recordId)
        {
            return $"{_url}/api/items/{recordId}/delete";
        }
    }
}
