namespace PleasanterBridge.src.APIBridge.Entity
{
    public class ImageEntity: ImageParamsEntity
    {
        public readonly string Columns = "";

        public ImageEntity(
            string columns,
            bool headNewLine,
            bool endNewLine,
            int position,
            string alt,
            string extension,
            string base64
        ):base(
            headNewLine,
            endNewLine,
            position,
            alt,
            extension,
            base64
        )
        {
            Columns = columns;
        }
    }
}
