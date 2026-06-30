namespace PleasanterBridge.src.APIBridge.Factory.Share
{
    public interface IApiBridgeFactory<TBridge, TSettings>
        where TBridge : class
        where TSettings : class
    {
        TBridge Create(TSettings settings);
    }
}