using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Factory.share
{
    public abstract class ApiBridgeFactoryBase<TSettings>
    where TSettings : class
    {
        // 抽象メソッド: 継承クラスに固有の設定（ヘッダーなど）の適用を強制
        protected abstract void ApplySpecificSettings(TSettings settings, HttpClient client);

        // 💡 テンプレートメソッド: 共通の HttpClient の生成と初期化を実行
        protected HttpClient CreateInitializedHttpClient(TSettings settings)
        {
            // 1. 設定の検証 (共通)
            ValidateSettings(settings);

            // 2. HttpClientの生成と共通設定 (共通)
            var client = new HttpClient();

            // 3. 固有の設定（APIキーヘッダーなど）を適用 (Template Method)
            ApplySpecificSettings(settings, client);

            return client;
        }

        // --- 共通の検証処理 ---
        protected virtual void ValidateSettings(TSettings settings)
        {
            // 例: TSettingsがnullでないことをチェックなど、汎用的な検証
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "設定オブジェクトは必須です。");
            }
        }
    }
}