using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Subschemas
{
    /// <summary>
    /// Define a compact model reference.
    /// </summary>
    public class LabelModel
    {
        /// <value>Compact model unique identifier.</value>
        [YamlMember(Alias = "id")]
        public int ID
        { get; set; }
        /// <value>Reference to a compact model description.</value>
        [YamlMember(Alias = "name")]
        public string Name
        { get; set; }
        /// <value>Contains one or more parameter assignments.</value>
        [YamlMember(Alias = "parameters")]
        public Dictionary<string, string> Parameters
        { get; set; }
    }
}
