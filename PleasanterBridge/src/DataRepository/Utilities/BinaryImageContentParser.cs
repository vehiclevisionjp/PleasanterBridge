using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Service;
using PleasanterBridge.src.DataRepository.Service.Implementation;
using System.Text.RegularExpressions;

namespace PleasanterBridge.src.DataRepository.Utilities
{
    internal class BinaryImageContentParser
    {
        private const string ImageReferencePattern = @"!\[image\]\(/binaries/([a-f0-9]{32})/show\)";
        private readonly IGetBinariesProcess _getBinariesProcess;

        public BinaryImageContentParser(IConnectionSettings settings)
        {
            _getBinariesProcess = new GetBinariesProcess(settings);
        }

        public List<string> ParseContentWithBinaryReferences(string? text)
        {
            var segments = new List<string>();

            if (string.IsNullOrEmpty(text))
            {
                return segments;
            }

            // 正規表現でマッチ情報を取得
            MatchCollection matches = Regex.Matches(text, ImageReferencePattern, RegexOptions.IgnoreCase);

            if (matches.Count == 0)
            {
                // 画像参照がない場合は、テキスト全体を返す
                segments.Add(text);
                return segments;
            }

            int lastIndex = 0;

            foreach (Match match in matches)
            {
                // マッチ前のテキストを追加
                if (match.Index > lastIndex)
                {
                    string beforeText = text.Substring(lastIndex, match.Index - lastIndex);
                    if (!string.IsNullOrEmpty(beforeText))
                    {
                        segments.Add(beforeText);
                    }
                }

                // GUIDを追加（重複していても元の順序のまま含める）
                if (match.Groups.Count > 1)
                {
                    string guid = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(guid))
                    {
                        segments.Add(guid);
                    }
                }

                lastIndex = match.Index + match.Length;
            }

            // 最後のマッチ後のテキストを追加
            if (lastIndex < text.Length)
            {
                string remainingText = text.Substring(lastIndex);
                if (!string.IsNullOrEmpty(remainingText))
                {
                    segments.Add(remainingText);
                }
            }

            return ReplaceBinaryGuidsWithDataAsync(segments);
        }

        public static List<string> ExtractGuidsFromSegments(List<object> segments)
        {
            var guids = new List<string>();
            var seenGuids = new HashSet<string>();

            foreach (var segment in segments)
            {
                if (segment is string str && IsValidGuid(str))
                {
                    if (!seenGuids.Contains(str))
                    {
                        guids.Add(str);
                        seenGuids.Add(str);
                    }
                }
            }

            return guids;
        }

        private static bool IsValidGuid(string value)
        {
            return !string.IsNullOrEmpty(value) &&
                   value.Length == 32 &&
                   value.All(c => char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
        }

        private List<string> ReplaceBinaryGuidsWithDataAsync(List<string> segments)
        {
            var result = new List<string>();
            var guidCache = new Dictionary<string, string>();

            foreach (var segment in segments)
            {
                if (segment is string str && IsValidGuid(str))
                {
                    // 同じGUIDは1度だけ取得（キャッシュを使用）
                    if (!guidCache.TryGetValue(str, out string? binaryData))
                    {
                        try
                        {
                            binaryData = _getBinariesProcess.GetImage(str);
                            guidCache[str] = binaryData;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Failed to fetch binary data for GUID: {str}", ex);
                        }
                    }
                    // キャッシュされたバイナリデータをリストに追加（重複を保持）
                    result.Add(guidCache[str]);
                }
                else
                {
                    result.Add(segment);
                }
            }

            return result;
        }
    }
}