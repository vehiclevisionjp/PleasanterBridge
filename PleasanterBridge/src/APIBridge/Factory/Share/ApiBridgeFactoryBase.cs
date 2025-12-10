namespace PleasanterBridge.src.APIBridge.Factory.Share
{
    public abstract class ApiBridgeFactoryBase<TSettings>
    where TSettings : class
    {
        protected abstract void ApplySpecificSettings(TSettings settings, HttpClient client);

        protected HttpClient CreateInitializedHttpClient(TSettings settings)
        {
            ValidateSettings(settings);

            var client = new HttpClient();

            ApplySpecificSettings(settings, client);

            return client;
        }

        protected virtual void ValidateSettings(TSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "設定オブジェクトは必須です。");
            }
        }
    }
}