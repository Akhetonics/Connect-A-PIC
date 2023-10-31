using System.Collections.Generic;

namespace AtetDataFormats.OpenEPDA
{
    /// <summary>
    /// openEPDA uPDK (tm) SBB schema description.
    /// Describes a single row in the schema.
    /// Reference: https://openepda.org/updk/0.4/format_specification.html
    /// </summary>
    public class BlockSchema
    {
        /// <summary>
        /// Creates a row of BlockSchema description.
        /// </summary>
        public BlockSchema(int level, string label, string type, bool required, string documentation, 
                           object defaultValue, string allowedValues, string example)
        { 
            this.Level = level;
            this.Label = label;
            this.Type = type;
            this.Required = required;
            this.Documentation = documentation;
            this.DefaultValue = defaultValue;
            this.AllowedValues = allowedValues;
            this.Example = example;
        }
        
        /// <value>Hierachy by indentation (YAML).</value>
        public int Level
        { get; set; }
        /// <value>Label of the entry.</value>
        public string Label
        { get; set; }
        /// <value>
        /// The datatype of each label. A datatype is int, float, str, object, or 
        /// subschema. See the SSB metadata schema for more detail.
        /// </value>
        public string Type
        { get; set; }
        /// <value> 
        /// Indicates that a label must be present in the SBB to have a minimum set of 
        /// data to describe the block. If not present the default value must be assumed. 
        /// If there is no default the schema is incomplete.
        /// </value>
        public bool Required
        { get; set; }
        /// <value>Describes the purpose of the label.</value>
        public string Documentation
        { get; set; }
        /// <value>Default value where applicable. If a value is missing the default must be assumed.</value>
        public object DefaultValue
        { get; set; }
        /// <value>List of allowed values, if applicable.</value>
        public string AllowedValues
        { get; set; }
        /// <value>Example data.</value>
        public string Example
        { get; set; }

        /// <summary>
        /// Parses a list-type default value found
        /// in the Block Schema.
        /// </summary>
        /// <returns>
        /// The default value as a List of strings.
        /// </returns>
        public static List<string> parseDefaultValue(string defaultValue)
        {
            /*
            Some default values (list) are of the form:
            ['um', 'um', 'deg']

            We can always assume these are well formatted, so we can do a simple parse here.
            */

            if (string.IsNullOrEmpty(defaultValue) || defaultValue.Length < 3)
            {
                return null;
            }

            List<string> returnList = new List<string>();

            // Substring out the brackets and parse through each element seperated by a comma.
            // We ignore that the strings could have commas in them for now, but this might be important
            // in the future.
            foreach(string listElement in defaultValue.Substring(1, defaultValue.Length-2).Split(','))
            {
                if (listElement.Length > 2)
                {
                    // We substring out the first and last quote and add it:
                    returnList.Add(listElement.Substring(1, listElement.Length-2));
                }
            }

            return returnList;
        }

        /// <value>None object indicating that the default value is truly "none" instead of null.</value>
        public static object None
        { get; } = new object();

        /// <value>Defines the standard v0.4 Block Schema.</value>
        public readonly static BlockSchema[] StandardBlockSchema = new BlockSchema[]
        {
            /* 
            This was generated using the following "filthy" Excel snippet applied to the Schema CSV file:
            
            ="new BlockSchema("&A2&", """&LEFT(B2,LEN(B2)-1)&""", """&C2&""", "&IF(ISBLANK(D2),"false","true")&", """&E2&""", 
             "&IF(EXACT(F2,"None"),"BlockSchema.None",IF(ISBLANK(F2),"null",IF(EXACT("str",C2),""""&F2&"""",
              IF(EXACT("list",C2),"BlockSchema.parseDefaultValue("""&F2&""")","null"))))&", """&G2&""", """&H2&"""),"

              NOTE: This will break in the future if the default values (list) are not just a List of Strings.
            */
            new BlockSchema(1, "header", "object", true, "Contains licensing and usage information",null, "", ""),
            new BlockSchema(2, "description", "str", true, "Short info on the purpose of this scheme","schema to describe a uPDK.", "", ""),
            new BlockSchema(2, "file_version", "str", true, "version of the SBB file","1", "", ""),
            new BlockSchema(2, "openEPDA", "object", true, "openEPDA related information.",null, "", ""),
            new BlockSchema(3, "version", "str", true, "openEPDA version of this scheme","openEPDA-uPDK-SBB-v0.4", "", ""),
            new BlockSchema(3, "link", "str", true, "Link to openEPDA site.","https://www.openEPDA.org", "", ""),
            new BlockSchema(2, "schema_license", "object", true, "License information for using this superschema and the derived schema",null, "", ""),
            new BlockSchema(3, "license", "str", true, "CC BY-SA 4.0 - Mandatory Creative Commons license condition","CC BY-SA 4.0", "", ""),
            new BlockSchema(3, "attribution", "str", true, "openEPDA--uPDK-SBB-v0.4 - Mandatory attribution required under the Creative Commons license.","openEPDA-uPDK-SBB-v0.4", "", ""),
            new BlockSchema(2, "pdk_license", "str", true, "License conditions of the content in the YAML.",null, "", "under NDA, Joe & sons #123-1999"),
            new BlockSchema(1, "xsections", "object", false, "Contains zero or more cross section definitions.",null, "", ""),
            new BlockSchema(2, "<xsection_name>", "object", false, "Define a cross section reference named <xsection_name>.",null, "", ""),
            new BlockSchema(3, "width", "float", false, "Define the default width of a structure in this xsection <xsection_name>.",null, "", ""),
            new BlockSchema(3, "width_min", "float", false, "Define the minimum width of a structure in this xsection <xsection_name>.",null, "", ""),
            new BlockSchema(3, "radius", "float", false, "Define the default radius of a structure in this xsection <xsection_name>.",null, "", ""),
            new BlockSchema(3, "radius_min", "float", false, "Define the minimum radius of a structure in this xsection <xsection_name>.",null, "", ""),
            new BlockSchema(3, "models", "object", false, "Contains zero or more compact models for xsection <xsection_name>.",null, "", ""),
            new BlockSchema(4, "models", "subschema", false, "models",null, "", ""),
            new BlockSchema(1, "blocks", "object", true, "Contains zero or more BB definitions.",null, "", ""),
            new BlockSchema(2, "<block_name>", "object", false, "Define a BB reference named <block_name>.",null, "", ""),
            new BlockSchema(3, "id", "str", false, "Reference to the unique ID used for this block across PDK version.",BlockSchema.None, "", ""),
            new BlockSchema(3, "version", "str", false, "BB version number set by the foundry.",null, "", ""),
            new BlockSchema(3, "license", "str", true, "Licensing conditions of this BB.","Block may be licensed.", "", "Licensed by foundry X under Y."),
            new BlockSchema(3, "cellname", "str", false, "Cellname of the BB. If no cellname label is found, the <block_name> is the cell name.",null, "", ""),
            new BlockSchema(3, "doc", "str", true, "Short sentence to describe the BB to the user.",null, "", ""),
            new BlockSchema(3, "bbox", "list", true, "Array of points (x, y) defining the bbox outline as a polygon. The polygon does not have to be closed.",BlockSchema.None, "", "[[0, 0], [10, 0], [10, 5], [0, 5]]"),
            new BlockSchema(3, "bb_metal_outline", "list", false, "List of polygons defining metal pads of the BB. Each polygon is an array of points (x, y). The polygons do not have to be closed. If present, this item should contain at least one polygon.",null, "", "[[[0, 0], [10, 0], [10, 5], [0, 5]]]"),
            new BlockSchema(3, "bb_width", "float", false, "Width of the BB cell in um.",null, "", ""),
            new BlockSchema(3, "bb_length", "float", false, "Length of the BB cell in um.",null, "", ""),
            new BlockSchema(3, "pin_in", "str", false, "Name of default input pin of the BB.",null, "", ""),
            new BlockSchema(3, "pin_out", "str", false, "Name of default output pin of the BB.",null, "", ""),
            new BlockSchema(3, "pins", "object", true, "Contains one or more pin definitions for <block_name>.",null, "", ""),
            new BlockSchema(4, "<pin_name>", "object", true, "Define a pin named <pin_name>.",null, "", ""),
            new BlockSchema(5, "id", "int", false, "unique identifier",null, "", ""),
            new BlockSchema(5, "width", "float", true, "Width of the pin.",null, "", "2"),
            new BlockSchema(5, "width_unit", "str", false, "Unit of the pin width.","um", "list of allowed values", "um"),
            new BlockSchema(5, "xsection", "str", true, "Cross section name.",null, "", "WAVEGUIDE"),
            new BlockSchema(5, "alias", "str", false, "Alias for <pin_name>.",null, "", "input1"),
            new BlockSchema(5, "doc", "str", true, "Short description of the pin.",null, "", "optical input"),
            new BlockSchema(5, "xya", "list", true, "Pin coordinate (x, y, a) with respect to <block_name> origin.",null, "", "[0, 0, 0]"),
            new BlockSchema(5, "xya_unit", "list", false, "Units of the (x, y, a) coordinate",BlockSchema.parseDefaultValue("['um', 'um', 'deg']"), "", ""),
            new BlockSchema(5, "direction", "str", false, "pin direction of xya w.r.t. the block; outward 'out' or inward 'in'.","out", "['in', 'out']", ""),
            new BlockSchema(5, "radius", "float", false, "radius of curvature at pin (0 or null is no curvature).",null, "", ""),
            new BlockSchema(3, "models", "object", false, "Define zero or more compact models.",BlockSchema.None, "", ""),
            new BlockSchema(4, "models", "subschema", false, "models",null, "", ""),
            new BlockSchema(3, "drc", "object", true, "Define zero or more DRC rules.",BlockSchema.None, "", ""),
            new BlockSchema(4, "drc_rules", "subschema", false, "drc rules",null, "", ""),
            new BlockSchema(3, "parameters", "object", true, "Pcell only. Contains one or more BB parameter definitions.",null, "", ""),
            new BlockSchema(4, "<parameter_name>", "object", false, "Define a BB parameter.",null, "", ""),
            new BlockSchema(5, "doc", "str", true, "Short parameter description.","No documentation provided", "", ""),
            new BlockSchema(5, "type", "str", true, "Data type",null, "['float', 'int', 'str']", ""),
            new BlockSchema(5, "unit", "str", true, "Unit of the parameter",null, "list of allowed values", "['um']"),
            new BlockSchema(5, "min", "see type", true, "Minimum value.",BlockSchema.None, "", ""),
            new BlockSchema(5, "max", "see type", true, "Maximum value.",BlockSchema.None, "", ""),
            new BlockSchema(5, "value", "see type", true, "Default value.",BlockSchema.None, "list of allowed values if applicable", ""),
            new BlockSchema(5, "alias", "str", false, "Alias for <parameter_name>.",null, "", ""),
            new BlockSchema(3, "keywordparameters", "list", false, "List of <parameter_name> used in the BB function call (subset of parameters). If the label is not present, all parameters are considered to be keyword parameters.",null, "", "['a', 'b', 'c']"),
            new BlockSchema(3, "cellnameparameters", "list", false, "List of <parameter_name> for more descriptive cell names (subset of keywordparameters).",null, "", "b, x"),
            new BlockSchema(3, "call", "str", false, "name of function call that creates the BB",null, "", ""),
            new BlockSchema(3, "groupname", "str", false, "Name for grouping BB.",null, "", ""),
            new BlockSchema(3, "ip_block", "object", false, "Define ip_block data as ip_block",null, "", ""),
            new BlockSchema(4, "ip_block", "subschema", false, "i- block information",null, "", ""),
            new BlockSchema(3, "icon", "object", false, "Define an icon for <block_name>.",null, "", ""),
            new BlockSchema(4, "function", "str", false, "Name of the function that returns a cell with the icon.",null, "", ""),
            new BlockSchema(4, "parameters", "object", false, "Parameters for which default will be overridden.",null, "", ""),
            new BlockSchema(5, "bufx", "float", false, "Buffer in the x-direction in um.",null, "", ""),
            new BlockSchema(5, "bufy", "float", false, "Buffer in the y-direction in um.",null, "", ""),
            new BlockSchema(5, "length", "float", false, "Icon length in the x-direction in um.",null, "", ""),
            new BlockSchema(5, "width", "float", false, "Icon length in the y-direction in um.",null, "", ""),
            new BlockSchema(1, "subschemas", "object", false, "Describe zero or more subschemas",null, "", ""),
            new BlockSchema(2, "drc-rules", "object", false, "Describe one or more <<drc-rules>>",null, "", ""),
            new BlockSchema(3, "angle", "object", false, "angle DRC rule for instantiation w.r.t the mask",null, "", ""),
            new BlockSchema(4, "values_and_domains", "subschema", false, "values and domains",null, "", ""),
            new BlockSchema(3, "angle_mirror", "object", false, "angle DRC rule for instantiation w.r.t the mask with mirroring status",null, "", ""),
            new BlockSchema(4, "flip", "object", false, "group rules that apply for a flip=true state",null, "", ""),
            new BlockSchema(5, "values_and_domains", "subschema", false, "values and domains",null, "", ""),
            new BlockSchema(4, "noflip", "object", false, "group rules that apply for a flip=false state",null, "", ""),
            new BlockSchema(5, "values_and_domains", "subschema", false, "values and domains",null, "", ""),
            new BlockSchema(2, "values_and_domains", "object", false, "Describe one or more <<values_and_domains>>.",null, "", ""),
            new BlockSchema(3, "values", "list", false, "list of allowed angles",null, "", "[0, 90, 270]"),
            new BlockSchema(3, "domains", "list", false, "list of allowed angle domains",null, "", "[[0, 90], [180, 270]]"),
            new BlockSchema(2, "models", "object", false, "Describe one or models.",null, "", ""),
            new BlockSchema(3, "<model_name>", "object", false, "Define a compact model reference named <model_name>.",null, "", "model_1"),
            new BlockSchema(4, "id", "int", false, "compact model unique identifier.",null, "", ""),
            new BlockSchema(4, "name", "str", false, "Reference to a compact model description.",null, "", ""),
            new BlockSchema(4, "parameters", "object", false, "Contains one or more parameter assignments.",null, "", "{'a': 4.0, 'x': 10}"),
            new BlockSchema(5, "<parameter_name>", "str", false, "Assign a value to <parameter_name>.",null, "", ""),
            new BlockSchema(2, "ip_block", "object", false, "Describe ip-block data.",null, "", ""),
            new BlockSchema(3, "license", "str", false, "License information",null, "", "CC BY-SA 4.0"),
            new BlockSchema(3, "owner", "str", false, "IP_Block owner",null, "", "Bright Photonics"),
            new BlockSchema(3, "pgp_file", "str", false, "Name of pgp encrypted ip_block file.",null, "", ""),
            new BlockSchema(3, "pgp_key", "str", false, "Hash of public pgp key used to encrypt the IP-Block.",null, "", ""),
            new BlockSchema(3, "md5", "str", false, "md5 hash of decrypted pgp ip_block.",null, "", ""),
        };
    }

}
