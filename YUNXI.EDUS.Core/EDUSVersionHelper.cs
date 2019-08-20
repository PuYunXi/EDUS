using System;
using System.IO;

namespace YUNXI.EDUS
{
    /// <summary>
    ///     Central point for application version.
    /// </summary>
    public class EDUSVersionHelper
    {
        public const string AppName = " EDUS ";

        /// <summary>
        ///     Gets current version of the application.
        ///     All project's assembly versions are changed when this value is changed.
        ///     It's also shown in the web page.
        /// </summary>
        public const string Version = "1.0.0";

        /// <summary>
        ///     Gets release (last build) date of the application.
        ///     It's shown in the web page.
        /// </summary>
        public static DateTime ReleaseDate
        {
            get
            {
                var fileName = typeof(EDUSVersionHelper).Assembly.Location;
                return new FileInfo(fileName).LastWriteTime;
            }
        }
    }
}
