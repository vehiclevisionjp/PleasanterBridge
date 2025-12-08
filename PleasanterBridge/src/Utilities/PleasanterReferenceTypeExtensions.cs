namespace PleasanterBridge.src.Utilities
{
    internal class PleasanterReferenceTypeExtensions
    {
        public static PleasanterReferenceType ParseReferenceType(string dbValue)
        {
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
                throw new ArgumentException($"Invalid reference type received from DB: {dbValue}", nameof(dbValue));
            }
        }
    }
}
