using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Subschemas
{
    /// <summary>
    /// Describe ip-block data.
    /// </summary>
    public class LabelIPBlock
    {
        /// <value>License information.</value>
        [YamlMember(Alias = "license")]
        public string License
        { get; set; }
        /// <value>IP_Block owner.</value>
        [YamlMember(Alias = "owner")]
        public string Owner
        { get; set; }
        /// <value>Name of pgp encrypted ip_block file.</value>
        [YamlMember(Alias = "pgp_file")]
        public string PGPFile
        { get; set; }
        /// <value>Hash of public pgp key used to encrypt the IP-Block.</value>
        [YamlMember(Alias = "pgp_key")]
        public string PGPKey
        { get; set; }
        /// <value>MD5 hash of decrypted pgp ip_block.</value>
        [YamlMember(Alias = "md5")]
        public string MD5
        { get; set; }
    }
}
