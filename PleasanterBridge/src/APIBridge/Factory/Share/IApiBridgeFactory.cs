namespace PleasanterBridge.src.APIBridge.Factory.Share
{
    // TBridge: 生成される具体的なブリッジの型 (IPleasanterApiBridgeなど)
    // TSettings: 初期化に必要な設定の型 (IPleasanterSettingsなど)
    public interface IApiBridgeFactory<TBridge, TSettings>
        where TBridge : class
        where TSettings : class
    {
        // 汎用的なCreateメソッド: 設定を受け取り、初期化済みのTBridgeインスタンスを返す
        TBridge Create(TSettings settings);
    }
}