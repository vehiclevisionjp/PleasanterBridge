// updateするEntityというより、Updateに送る形式を固定するEntity
// user的には、UpdateKeyとValueを設定していく
// 問題はUpdateKeyは複数存在することだが…
// resIdをKeyにしない場合だけこれ？
// そもそも、resultId以外をKeyにする場合、常にBulkupdateの可能性があるわけだが…

namespace PleasanterAPIBridge.src.PleasanterAPIBridge.Entity.Implementation
{
    public class UpdateEntity<T>
    {
        public List<Dictionary<string, T>> UpdateKeys { get; set; }
        public List<Dictionary<string, T>> UpdateValues { get; set; }

        public UpdateEntity(
            List<Dictionary<string, T>> updateKeys,
            List<Dictionary<string, T>> updateValues
        )
        {
            UpdateKeys = updateKeys;
            UpdateValues = updateValues;
        }
    }
}