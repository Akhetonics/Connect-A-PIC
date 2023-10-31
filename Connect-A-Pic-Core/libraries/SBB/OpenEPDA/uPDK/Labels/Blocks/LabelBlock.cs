using System.Collections.Generic;
using AtetDataFormats.OpenEPDA.Labels.Subschemas;
using YamlDotNet.Serialization;

namespace AtetDataFormats.OpenEPDA.Labels.Blocks
{
    /// <summary>
    /// Defines a BB reference.
    /// </summary>
    public class LabelBlock
    {
        /// <value>Reference to the unique ID used for this block across PDK version.</value>
        [YamlMember(Alias = "id")]
        public string ID
        { get; set; } = "";
        /// <value>BB version number set by the foundry.</value>
        [YamlMember(Alias = "version")]
        public string Version
        { get; set; }
        /// <value>Licensing conditions of this BB.</value>
        [YamlMember(Alias = "license")]
        public string License
        { get; set; } = "Block may be licensed.";
        /// <value>Cellname of the BB. If no cellname label is found, the block's name is the cell name.</value>
        [YamlMember(Alias = "cellname")]
        public string CellName
        { get; set; }
        /// <value>Short sentence to describe the BB to the user.</value>
        [YamlMember(Alias = "doc")]
        public string Doc
        { get; set; }
        /// <value>
        /// Array of points (x, y) defining the bbox outline as a polygon. 
        /// The polygon does not have to be closed.
        /// </value>
        [YamlMember(Alias = "bbox")]
        public List<List<string>> BBox
        { get; set; } = new List<List<string>>();
        /// <value>
        /// List of polygons defining metal pads of the BB. Each polygon 
        /// is an array of points (x, y). The polygons do not have to be 
        /// closed. If present, this item should contain at least one polygon.
        /// </value>
        [YamlMember(Alias = "bb_metal_outline")]
        public List<List<List<string>>> BBMetalOutline
        { get; set; }
        /// <value>Width of the BB cell in um.</value>
        [YamlMember(Alias = "bb_width")]
        public float BBWidth
        { get; set; }
        /// <value>Length of the BB cell in um.</value>
        [YamlMember(Alias = "bb_length")]
        public float BBLength
        { get; set; }
        /// <value>Name of default input pin of the BB.</value>
        [YamlMember(Alias = "pin_in")]
        public string PinIn
        { get; set; }
        /// <value>Name of default output pin of the BB.</value>
        [YamlMember(Alias = "pin_out")]
        public string PinOut
        { get; set; }
        /// <value>Contains one or more pin definitions for this block.</value>
        [YamlMember(Alias = "pins")]
        public Dictionary<string, LabelPin> Pins
        { get; set; }
        /// <value>Contains zero or more compact models.</value>
        [YamlMember(Alias = "models")]
        public LabelModel Models
        { get; set; } = null;
        /// <value>Define zero or more DRC rules.</value>
        [YamlMember(Alias = "drc")]
        public LabelDRCRules DRC
        { get; set; } = null;
        /// <value>Pcell only. Contains one or more BB parameter definitions.</value>
        [YamlMember(Alias = "parameters")]
        public Dictionary<string, LabelParameter> Parameters
        { get; set; }
        /// <value>
        /// List of parameter names used in the BB function call (subset of parameters). 
        /// If the label is not present, all parameters are considered to be keyword parameters..
        /// </value>
        [YamlMember(Alias = "keywordparameters")]
        public List<string> KeywordParameters
        { get; set; }
        /// <value>
        /// List of parameter names for more descriptive cell names (subset of keywordparameters).
        /// </value>
        [YamlMember(Alias = "cellnameparameters")]
        public List<string> CellNameParameters
        { get; set; }
        /// <value>Name of function call that creates the BB.</value>
        [YamlMember(Alias = "call")]
        public string Call
        { get; set; }
        /// <value>Name for grouping BB.</value>
        [YamlMember(Alias = "groupname")]
        public string GroupName
        { get; set; }
        /// <value>Define ip_block data as ip_block.</value>
        [YamlMember(Alias = "ip_block")]
        public LabelIPBlock IPBlock
        { get; set; }
        /// <value>Define an icon for the block.</value>
        [YamlMember(Alias = "icon")]
        public LabelIcon Icon
        { get; set; }
    }
}
