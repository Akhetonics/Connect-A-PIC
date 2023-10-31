using System.Collections.Generic;
using AtetDataFormats.OpenEPDA.Labels;
using AtetDataFormats.OpenEPDA.Labels.XSections;
using AtetDataFormats.OpenEPDA.Labels.Blocks;
using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA
{
    /// <summary>
    /// Implementation of the openEPDA uPDK (tm) Standard Black Blocks (SBB).
    /// Version 0.4 (latest, draft) - https://openepda.org/updk/0.4/format_specification.html
    /// </summary>
    public class SBB
    {
        /// <value>Top label for the header.</value>
        [YamlMember(Alias = "header")]
        public LabelHeader Header { get; set; }
        /// <value>Top label for all xsections (optional).</value>
        [YamlMember(Alias = "xsections")]
        public Dictionary<string, LabelXSection> XSections { get; set; }
        /// <value>Top label for all blocks.</value>
        [YamlMember(Alias = "blocks")]
        public Dictionary<string, LabelBlock> Blocks { get; set; }
        /// <value>Top label for all subschemas.</value>
        [YamlMember(Alias = "subschemas")]
        public LabelSubschemas Subschemas { get; set; }
    }
}
