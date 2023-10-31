using System.Collections.Generic;
using AtetDataFormats.OpenEPDA.Labels.Subschemas;
using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.XSections
{
    /// <summary>
    /// Define a cross section reference.
    /// </summary>
    public class LabelXSection
    {
        /// <value>Define the default width of a structure in this xsection.</value>
        [YamlMember(Alias = "width")]
        public float Width
        { get; set; }
        /// <value>Define the minimum width of a structure in this xsection.</value>
        [YamlMember(Alias = "width_min")]
        public float WidthMin
        { get; set; }
        /// <value>Define the default radius of a structure in this xsection.</value>
        [YamlMember(Alias = "radius")]
        public float Radius
        { get; set; }
        /// <value>Define the minimum radius of a structure in this xsection.</value>
        [YamlMember(Alias = "radius_min")]
        public float RadiusMin
        { get; set; }
        /// <value>Contains zero or more compact models.</value>
        [YamlMember(Alias = "models")]
        public LabelModel Models
        { get; set; } = null;
    }
}
