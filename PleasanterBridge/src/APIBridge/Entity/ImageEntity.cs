namespace PleasanterBridge.src.APIBridge.Entity
{
    public class ImageEntity
    {
        public string Base64 { get; }
        public bool HeadNewLine { get; }
        public bool EndNewLine { get; }
        public int Position { get; }
        public string Alt { get; }
        public string Extension { get; }

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
