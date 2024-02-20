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
        public static int ExtractIdentifierNumber(string Identifier)
        {
            // Regular expression pattern: 
            // - ^[a-zA-Z]+ for one or more letters at the beginning of the string
            // - (\d+)$ for capturing one or more digits at the end of the string
            var match = Regex.Match(Identifier, @"^[a-zA-Z]+(\d+)$");

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
                string key = $"{ParameterIdentifiers.PinParameterIdentifier}{pin.PinNumber}".ToUpper(); // e.g. "PIN1"
                map[key] = pin.IDInFlow; // we are about to calculate the outflow in that equation, therefore we need to map the IDInFlow here.
            }
            return map;
        }

        public static Dictionary<string, Guid> CreateMapFromSliders(List<Slider> sliders)
        {
            var map = new Dictionary<string, Guid>();
            foreach (var slider in sliders)
            {
                string key = $"{ParameterIdentifiers.SliderParameterIdentifier}{slider.Number}".ToUpper(); // e.g. "SLIDER1"
                map[key] = slider.ID;
            }
            return map;
        }

        public static ConnectionFunction? ConvertToDelegate(string realOrFormula, List<Pin> allPinsInComponent, List<Slider> allSlidersInComponent)
        {
            // check if it is a formula (nonLinear Connection) or just a double (linear connection)
            if (Double.TryParse(realOrFormula, out _) == false)
            {
                return ConvertToDelegate(realOrFormula, CreateMapFromPins(allPinsInComponent), CreateMapFromSliders(allSlidersInComponent));
            }
            return null;
        }

        public static Dictionary<string, Guid> ExtractParameterGuids(string expressionDraft,
            Dictionary<string, Guid> pinParameterNameToGuidMap, Dictionary<string, Guid> sliderParameterNameToGuidMap, out bool IsPinsInvolved)
        {
            var regex = new Regex(@"[A-Z]+[0-9]+");
            var matches = regex.Matches(expressionDraft);

            Dictionary<string, Guid> usedParameterGuids = new();
            foreach (Match match in matches)
            {
                string parameterName = match.Value;
                if (pinParameterNameToGuidMap.TryGetValue(parameterName, out Guid parameterGuid))
                {
                    usedParameterGuids.TryAdd(parameterName, parameterGuid);
                    IsPinsInvolved = true;
                }
                else if (sliderParameterNameToGuidMap.TryGetValue(parameterName, out parameterGuid))
                {
                    usedParameterGuids.TryAdd(parameterName, parameterGuid);
                    IsPinsInvolved = false;
                }
                else
                {
                    throw new InvalidOperationException($"Parameter name '{parameterName}' could not be found in any provided parameter name to Guid map.");
                }
            }
            IsPinsInvolved = false;
            return usedParameterGuids;
        }
        public static ConnectionFunction ConvertToDelegate(string expressionDraft,
                Dictionary<string, Guid> pinParameterNameToGuidMap, Dictionary<string, Guid> sliderParameterNameToGuidMap)
        {
            expressionDraft = MakePlaceHoldersUpperCase(expressionDraft);
            var expression = ComplexMath.CreateExpressionWithCustomFunctions(expressionDraft);
            
            // create a list of Guids for all used parameters in the correct order so that the caller can later provide the correct values from his dictionaries
            var usedParameterGuidMap = ExtractParameterGuids(expressionDraft, pinParameterNameToGuidMap, sliderParameterNameToGuidMap, out bool IsPinsInvolved);
            // add all parameters to the expression for reference
            foreach (var key in usedParameterGuidMap.Keys)
            {
                expression.Parameters.Add(key, null);
            }

            var connectionFunction = new ConnectionFunction(
                (freshlyInsertedParameters) => ExecuteExpressionFromDraft(expressionDraft, freshlyInsertedParameters, expression.Parameters.Keys.ToList()),
                expressionDraft,
                usedParameterGuidMap.Select(p => p.Value).ToList(),
                IsPinsInvolved
            );

            return connectionFunction;
        }

        private static Complex ExecuteExpressionFromDraft(string expressionDraft, List<object> freshlyInsertedParameters, List<string> expressionVariableKeys)
        {
            var expression = ComplexMath.CreateExpressionWithCustomFunctions(expressionDraft);
            // check if number of parameters is correct
            if (expressionVariableKeys.Count != freshlyInsertedParameters.Count)
                throw new InvalidOperationException($"Parameter count mismatch. Expected {expressionVariableKeys.Count}, but got {freshlyInsertedParameters.Count} in function {expressionDraft}");

            // assign parameter values
            int index = 0;
            foreach (string parameterName in expressionVariableKeys)
            {
                expression.Parameters[parameterName] = freshlyInsertedParameters[index];
                index++;
            }

            // run the expression
            var result = expression.Evaluate();
            if (result == null)
                throw new InvalidOperationException("Formula cannot be computed: " + expressionDraft);

            return (Complex)result;
        }

        public static string MakePlaceHoldersUpperCase(string expression)
        {
            foreach (var identifier in ParameterIdentifiers.All)
            {
                expression = Regex.Replace(expression, identifier.ToLower(), identifier, RegexOptions.IgnoreCase);
            }
            return expression;
        }
    }
}