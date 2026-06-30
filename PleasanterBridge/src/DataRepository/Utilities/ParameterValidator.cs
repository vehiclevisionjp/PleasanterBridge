using System.Collections;

namespace PleasanterBridge.src.DataRepository.Utilities
{
    /// <summary>
    /// Pleasanterリポジトリのパラメータバリデーションを行うクラス
    /// </summary>
    internal static class ParameterValidator
    {
        /// <summary>
        /// <summary>
        /// IDが有効な範囲内であることを検証
        /// </summary>
        /// <param name="id">検証するID</param>
        /// <param name="parameterName">パラメータ名（エラーメッセージ用）</param>
        /// <exception cref="ArgumentException">IDが0以下の場合</exception>
        public static void ValidateId(long id, string parameterName = "id")
        {
            if (id <= 0)
            {
                throw new ArgumentException($"{parameterName}は正の整数である必要があります。値: {id}", parameterName);
            }
        }

        /// <summary>
        /// IDのリストが有効であることを検証
        /// </summary>
        /// <param name="ids">検証するIDのリスト</param>
        /// <param name="parameterName">パラメータ名（エラーメッセージ用）</param>
        /// <exception cref="ArgumentNullException">リストがnullの場合</exception>
        /// <exception cref="ArgumentException">リストが空、またはいずれかのIDが0以下の場合</exception>
        public static void ValidateIds(List<long> ids, string parameterName = "referenceIds")
        {
            if (ids == null)
            {
                throw new ArgumentNullException(parameterName, $"{parameterName}がnullです。");
            }

            if (ids.Count == 0)
            {
                throw new ArgumentException($"{parameterName}が空です。", parameterName);
            }

            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i] <= 0)
                {
                    throw new ArgumentException(
                        $"{parameterName}[{i}]は正の整数である必要があります。値: {ids[i]}",
                        parameterName
                    );
                }
            }
        }

        /// <summary>
        /// Dictionary&lt;string, object&gt;のobject値が許可された型であることを検証
        /// </summary>
        /// <param name="keyValue">検証する辞書</param>
        /// <param name="parameterName">パラメータ名（エラーメッセージ用）</param>
        /// <exception cref="ArgumentException">許可されていない型の値が含まれている場合</exception>
        public static void ValidateKeyValue(Dictionary<string, object>? keyValue, string parameterName = "keyValue")
        {
            if (keyValue == null)
            {
                return; // nullは許可
            }

            foreach (var kvp in keyValue)
            {
                if (kvp.Value == null)
                {
                    continue; // null値は許可
                }

                Type valueType = kvp.Value.GetType();

                // 許可される型のチェック
                if (!IsAllowedType(valueType, kvp.Value))
                {
                    throw new ArgumentException(
                        $"{parameterName}['{kvp.Key}']の型'{valueType.Name}'は許可されていません。" +
                        $"許可される型: string, int, long, decimal, bool, DateTime, string[], List<string>",
                        parameterName
                    );
                }
            }
        }

        /// <summary>
        /// 値が許可された型であるかチェック
        /// </summary>
        private static bool IsAllowedType(Type type, object value)
        {
            // プリミティブ型と基本的な型
            if (
                type == typeof(string) ||
                type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(decimal) ||
                type == typeof(bool) ||
                type == typeof(DateTime) ||
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(short) ||
                type == typeof(byte))
            {
                return true;
            }

            // Nullable型のチェック
            Type? underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                return IsAllowedType(underlyingType, value);
            }

            // 配列型のチェック
            if (type.IsArray)
            {
                Type? elementType = type.GetElementType();
                if (elementType == typeof(string))
                {
                    return true;
                }
                return false;
            }

            // List<string>のチェック
            if (type.IsGenericType)
            {
                Type genericTypeDef = type.GetGenericTypeDefinition();
                if (genericTypeDef == typeof(List<>))
                {
                    Type[] genericArgs = type.GetGenericArguments();
                    if (genericArgs.Length == 1 && genericArgs[0] == typeof(string))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
