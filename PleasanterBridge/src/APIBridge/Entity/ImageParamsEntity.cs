namespace PleasanterBridge.src.APIBridge.Entity
{
    public class ImageParamsEntity
    {
        public readonly bool HeadNewLine = false;
        public readonly bool EndNewLine = false;
        public readonly int Position = -1;
        public readonly string Alt = "image";
        public readonly string Extension = ".png";
        public readonly string Base64;

        public ImageParamsEntity(
            bool headNewLine,
            bool endNewLine,
            int position,
            string alt,
            string extension,
            string base64
        )
        {
            HeadNewLine = headNewLine;
            EndNewLine = endNewLine;
            Position = position;
            Alt = alt;
            Extension = extension;
            Base64 = base64;
        }
    }
}
