using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Blocks
{
    /// <summary>
    /// Defines a pin.
    /// </summary>
    public class LabelPin
    {
        /// <value>Unique identifier.</value>
        [YamlMember(Alias = "id")]
        public int ID
        { get; set; }
        /// <value>Width of the pin.</value>
        [YamlMember(Alias = "width")]
        public float Width
        { get; set; }
        /// <value>Unit of the pin width.</value>
        [YamlMember(Alias = "width_unit")]
        public string WidthUnit
        { get; set; } = "um";
        /// <value>Cross section name.</value>
        [YamlMember(Alias = "xsection")]
        public string XSection
        { get; set; }
        /// <value>Alias for the pin.</value>
        [YamlMember(Alias = "alias")]
        public string Alias
        { get; set; }
        /// <value>Short description of the pin.</value>
        [YamlMember(Alias = "doc")]
        public string Doc
        { get; set; }
        /// <value>Pin coordinate (x, y, a) with respect to the block's origin.</value>
        [YamlMember(Alias = "xya")]
        public List<string> XYA
        { get; set; }
        /// <value>Units of the (x, y, a) coordinate.</value>
        [YamlMember(Alias = "xya_unit")]
        public List<string> XYAUnit
        { get; set; } = new List<string>() { "um", "um", "deg" };
        /// <value>pin direction of xya w.r.t. the block; outward 'out' or inward 'in'.</value>
        [YamlMember(Alias = "direction")]
        public string Direction
        { get; set; } = "out";
        /// <value>Radius of curvature at pin (0 or null is no curvature).</value>
        [YamlMember(Alias = "radius")]
        public float Radius
        { get; set; }
    }
}
