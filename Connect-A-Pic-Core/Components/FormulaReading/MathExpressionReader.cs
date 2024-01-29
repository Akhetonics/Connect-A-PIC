using CAP_Core.Components;
using CAP_Core.Components.FormulaReading;
using MathNet.Numerics;
using NCalc;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Numerics;
using System.Text.RegularExpressions;
using YamlDotNet.Core;

namespace CAP_Core.Grid.FormulaReading
{
    public static partial class MathExpressionReader
    {
        public const string PinParameterIdentifier = "PIN";
        public const string SliderParameterIdentifier = "SLIDER";
        public static int ExtractIdentifierNumber(string Identifier)
        {
            // Regular expression pattern: 
            // - \D* for any non-digit text (any characters except digits)
            // - (\d+) for capturing one or more digits
            var match = Regex.Match(Identifier, @"\D*(\d+)");

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                throw new InvalidParameterException();
            }
        }
        public static Dictionary<string, Guid> CreateMapFromPins(List<Pin> pins)
        {
            var map = new Dictionary<string, Guid>();
            foreach (var pin in pins)
            {
                string key = $"{PinParameterIdentifier}{pin.PinNumber}".ToUpper(); // e.g. "PIN1"
                map[key] = pin.IDInFlow;
            }
            return map;
        }

        public static Dictionary<string, Guid> CreateMapFromSliders(List<Slider> sliders)
        {
            var map = new Dictionary<string, Guid>();
            foreach (var slider in sliders)
            {
                string key = $"{SliderParameterIdentifier}{slider.Number}".ToUpper(); // e.g. "SLIDER1"
                map[key] = slider.ID;
            }
            return map;
        }

        public static ConnectionFunction? ConvertToDelegate(string realOrFormula, List<Pin> allPinsInGrid, List<Slider>? allSlidersInGrid = null)
        {
            allSlidersInGrid ??= new();
            // check if it is a formula (nonLinear Connection) or just a double (linear connection)
            if (Double.TryParse(realOrFormula, out _ ) == false)
            {
                return MathExpressionReader.ConvertToDelegate(realOrFormula, CreateMapFromPins(allPinsInGrid), CreateMapFromSliders(allSlidersInGrid));
            }
            return null;
        }

        public static ConnectionFunction ConvertToDelegate(string expressionDraft,
            Dictionary<string, Guid> pinParameterNameToGuidMap, Dictionary<string, Guid> sliderParameterNameToGuidMap)
        {
            expressionDraft = MakePlaceHoldersUpperCase(expressionDraft);
            var expression = new Expression(expressionDraft);
            ComplexMath.RegisterComplexFunctions(expression);
            expression.Evaluate(); // this is needed to extract the parameters
            // create a list of Guids for all used parameters in the correct order so that the caller can later provide the correct values from his dictionaries
            List<Guid> usedParameterGuids = new();
            foreach (string parameterName in expression.Parameters.Keys)
            {
                if (pinParameterNameToGuidMap.TryGetValue(parameterName, out Guid parameterGuid) == true)
                    usedParameterGuids.Add(parameterGuid);
                if (sliderParameterNameToGuidMap.TryGetValue(parameterName, out parameterGuid) == false)
                    usedParameterGuids.Add(parameterGuid);
                throw new InvalidOperationException("ParameterName could not be found in any parameterName to Guid map provided");
            }

            var connectionFunction = new ConnectionFunction(
                (freshlyInsertedParameters) =>
                {
                    // check if number of parameters is correct
                    if (expression.Parameters.Count != freshlyInsertedParameters.Count)
                        throw new InvalidOperationException($"Parameter count mismatch. Expected {expression.Parameters.Count}, but got {freshlyInsertedParameters.Count} in function {expressionDraft}");

                    // assign parameter values
                    int index = 0;
                    foreach (string parameterName in expression.Parameters.Keys.ToList())
                    {
                        expression.Parameters[parameterName] = freshlyInsertedParameters[index];
                        index++;
                    }

                    // run the expression
                    var result = expression.Evaluate();
                    if (result == null)
                        throw new InvalidOperationException("Formula cannot be computed: " + expressionDraft);

                    return (Complex)result;
                },
                expressionDraft,
                usedParameterGuids
            );

            return connectionFunction;
        }

        public static List<System.Linq.Expressions.ParameterExpression> FindParametersInExpression(string expression, string Identifier = PinParameterIdentifier)
        {
            // Regular expression to find placeholders like {P1}, {P2}, etc.
            var placeholderRegex = new Regex(@$"{Identifier}\d+");
            var matches = placeholderRegex.Matches(expression);

            var parameters = new List<System.Linq.Expressions.ParameterExpression>();
            foreach (Match match in matches)
            {
                // Extract the placeholder name without curly braces
                string placeholder = match.Value;

                // Check if a parameter with this name already exists to avoid duplicates
                if (!parameters.Any(p => p.Name == placeholder))
                {
                    parameters.Add(System.Linq.Expressions.Expression.Parameter(typeof(Complex), placeholder));
                }
            }
            return parameters;
        }

        public static string MakePlaceHoldersUpperCase(string expression)
        {
            expression = Regex.Replace(expression, PinParameterIdentifier.ToLower(), PinParameterIdentifier, RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, SliderParameterIdentifier.ToLower(), SliderParameterIdentifier, RegexOptions.IgnoreCase);
            return expression;
        }
    }
}