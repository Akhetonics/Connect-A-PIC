using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Subschemas
{
    /// <summary>
    /// Describes one or more values and domains.
    /// </summary>
    public class LabelValuesAndDomains
    {
        /// <value>List of allowed angles.</value>
        [YamlMember(Alias = "values")]
        public List<float> Values
        { get; set; }
        /// <value>List of allowed angle domains.</value>
        [YamlMember(Alias = "domains")]
        public List<string> Domains
        { get; set; }
    }
}
