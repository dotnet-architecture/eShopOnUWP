using System;

namespace eShop.WPF
{
    static public class ContentTypes
    {
        static public string GetExtensionFromContentType(string contentType)
        {
            contentType = contentType ?? "";
            switch (contentType.ToLower())
            {
                case "image/png":
                    return ".png";
                case "image/gif":
                    return ".gif";
                case "image/jpeg":
                    return ".jpg";
                case "image/bmp":
                    return ".bmp";
                case "image/tiff":
                    return ".tiff";
                case "image/wmf":
                    return ".wmf";
                case "image/jp2":
                    return ".jp2";
                case "image/svg+xml":
                    return ".svg";
                default:
                    return "";
            }
        }

        static public string GetContentTypeFromExtension(string extension)
        {
            switch (extension)
            {
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".bmp":
                    return "image/bmp";
                case ".tiff":
                    return "image/tiff";
                case ".wmf":
                    return "image/wmf";
                case ".jp2":
                    return "image/jp2";
                case ".svg":
                    return "image/svg+xml";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
