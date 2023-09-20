using System.IO;

namespace AtetDataFormats
{
    /// <summary>
    /// Handles embedded data.
    /// </summary>
    public static class EmbeddedData
    {
        /// <summary>
        /// Read an entire embedded file.
        /// </summary>
        /// <side-effects>
        /// Reads from an embedded file.
        /// </side-effects>   
        /// <returns>
        /// The entire file as a byte array.
        /// </returns>   
        public static byte[] ReadEmbeddedAssetBytes(System.Reflection.Assembly callerAssembly, string name)
        {
            using (Stream stream = callerAssembly.GetManifestResourceStream(name))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }

        /// <summary>
        /// Read an entire embedded file.
        /// </summary>
        /// <side-effects>
        /// Reads from an embedded file.
        /// </side-effects>   
        /// <returns>
        /// The entire file as a string.
        /// </returns>   
        public static string ReadEmbeddedAssetString(System.Reflection.Assembly callerAssembly, string name)
        {
            using (Stream stream = callerAssembly.GetManifestResourceStream(name))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
