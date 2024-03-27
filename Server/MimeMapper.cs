namespace StationeersWebApi.Server
{
    /// <summary>
    /// Gets mime types for file extensions.
    /// </summary>
    public static class MimeMapper
    {
        /// <summary>
        /// Gets the mime type for the given file extension.
        /// </summary>
        /// <param name="extension">The file extension, starting with dot.</param>
        /// <returns>The mime type for this file.</returns>
        public static string GetMimeType(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                return "application/octet-stream";
            }

            // Ensure the extension is in the format ".ext"
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            switch (extension.ToLower())
            {
                case ".html":
                case ".htm":
                    return "text/html";

                case ".css":
                    return "text/css";

                case ".js":
                    return "application/javascript";

                case ".json":
                    return "application/json";

                case ".xml":
                    return "application/xml";

                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";

                case ".png":
                    return "image/png";

                case ".gif":
                    return "image/gif";

                case ".svg":
                    return "image/svg+xml";

                case ".ico":
                    return "image/x-icon";

                case ".woff":
                    return "application/font-woff";

                case ".woff2":
                    return "application/font-woff2";

                case ".ttf":
                    return "application/font-ttf";

                case ".eot":
                    return "application/vnd.ms-fontobject";

                case ".otf":
                    return "font/otf";

                case ".webp":
                    return "image/webp";

                default:
                    return "application/octet-stream"; // Default to binary stream
            }
        }
    }
}
