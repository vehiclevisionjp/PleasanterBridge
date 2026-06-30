namespace PleasanterBridge.src.Utilities
{
    internal class PleasanterReferenceTypeExtensions
    {
        public static PleasanterReferenceType? ParseReferenceType(string dbValue)
        {
            // 空文字列やnullの場合はnullを返す
            if (string.IsNullOrEmpty(dbValue))
            {
                return null;
            }

            PleasanterReferenceType resultType;

            bool success = Enum.TryParse(
                dbValue,
                ignoreCase: true, // 大文字/小文字を無視してマッチング
                out resultType
            );

            if (success)
            {
                // 成功した場合、変換されたEnum値を返す
                return resultType;
            }
            else
            {
                // 失敗した場合はnullを返す
                return null;
            }
        }
    }
}
