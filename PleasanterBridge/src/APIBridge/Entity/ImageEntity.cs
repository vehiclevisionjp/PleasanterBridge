namespace PleasanterBridge.src.APIBridge.Entity
{
    public class ImageEntity
    {
        public readonly string Base64;
        public readonly bool HeadNewLine;
        public readonly bool EndNewLine;
        public readonly int Position;
        public readonly string Alt;
        public readonly string Extension;

        public ImageEntity(
            string base64,
            bool headNewLine = false,
            bool endNewLine = false,
            int position = -1,
            string alt = "image",
            string extension = ".png"
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
