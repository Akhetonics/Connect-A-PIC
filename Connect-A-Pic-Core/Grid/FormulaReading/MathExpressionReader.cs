using CAP_Core.Components;
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

        public static int ExtractPinNumber(string PinIdentifier)
        {
            // Regular expression pattern: 
            // - (?i) for case-insensitivity
            // - pin for matching the word "pin" or what is in PinParameterIdentifier
            // - (\d+) for capturing one or more digits
            var match = Regex.Match(PinIdentifier, @$"(?i){PinParameterIdentifier}(\d+)");

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            else
            {
                throw new InvalidParameterException();
            }
        }

        public static ConnectionFunction? ConvertToDelegate(string realOrFormula, List<Pin> allPins)
        {
            // create the non-linear function out of the string Formula
            if (Double.TryParse(realOrFormula, out double realValue) == false)
            {
                // first get all parameters that are in the formula as string
                var PinNumbersAsString = MathExpressionReader.FindParametersInExpression(realOrFormula)
                    .Select(p => p.Name)
                    .ToList();

                // then map those strings to the Guids of the actual pins and create the delegate function
                var stringToGuidMapper = new Dictionary<string, Guid>();
                foreach (var param in PinNumbersAsString)
                {
                    if (param == null) continue;
                    stringToGuidMapper.Add(param, allPins.Single(p => p.PinNumber == MathExpressionReader.ExtractPinNumber(param)).IDInFlow);
                }
                return MathExpressionReader.ConvertToDelegate(realOrFormula, stringToGuidMapper);
            }
            return null;
        }

        // Example: dynamically invoking with values
        // var values = new object[] { 1.0, 4.0 };
        // double result = (double)compiledLambda.DynamicInvoke(values);
        //    return result;
        // this function is being called after loading the component from JSON, so PIN1 is in the list of PinDrafts
        public static ConnectionFunction ConvertToDelegate(string expressionDraft, Dictionary<string, Guid> PinPlaceHolderToGuids)
        {
            expressionDraft = FixPlaceHolderNames(expressionDraft); // to make small letter pins also be capital letters..
            PinPlaceHolderToGuids = FixPlaceHolderNames(PinPlaceHolderToGuids); // also fix the keys here
            var parameters = FindParametersInExpression(expressionDraft);
            // set the culture to en-US so that points get parsed properly
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US"); 
            var expression = new Expression(expressionDraft); //DynamicExpressionParser.ParseLambda(parameters.ToArray(), null, expressionDraft);
            ComplexMath.RegisterComplexFunctions(expression);
            
            var connectionFunction = new ConnectionFunction(
                    (complexParameters) =>
                    {
                        // Ensure we have the correct number of parameters
                        if (parameters.Count != complexParameters.Count)
                            throw new InvalidOperationException($"Parameter count mismatch. Should be {expression.Parameters.Count}, but is {complexParameters.Count} in function {expressionDraft}");

                        // transfer the parameters into the expression
                        int index = 0;
                        foreach(var parameterName in PinPlaceHolderToGuids.Keys)
                        {
                            expression.Parameters[parameterName] = complexParameters[index];
                            index++;
                        }

                        // return the result
                        var result = expression.Evaluate();
                        if(result == null)
                        {
                            throw new InvalidOperationException("the formula cannot be computed. maybe you use operators that only support double but not complex numbers? " + expressionDraft);
                        }
                        return (Complex) result;
                    },
                    expressionDraft,
                    parameters.Select(p => PinPlaceHolderToGuids[p.Name]).ToList()
                );
            return connectionFunction;
        }

        public static List<System.Linq.Expressions.ParameterExpression> FindParametersInExpression(string expression)
        {
            // Regular expression to find placeholders like {P1}, {P2}, etc.
            var placeholderRegex = new Regex(@$"{PinParameterIdentifier}\d+");
            var matches = placeholderRegex.Matches(expression);

            var parameters = new List<System.Linq.Expressions.ParameterExpression>();
            foreach (Match match in matches)
            {
                // Extract the placeholder name without curly braces
                string placeholder = match.Value;

                // Check if a parameter with this name already exists to avoid duplicates
                if (!parameters.Any(p => p.Name == placeholder))
                {
                    parameters.Add(System.Linq.Expressions.Expression.Parameter(typeof(double), placeholder));
                }
            }
            return parameters;
        }

        public static Dictionary<string, Guid> FixPlaceHolderNames(Dictionary<string, Guid> originalDict)
        {
            // Create a new dictionary with the same capacity as the original for efficiency
            var fixedDict = new Dictionary<string, Guid>(originalDict.Count);

            foreach (var kvp in originalDict)
            {
                // Capitalize the key and add the key-value pair to the new dictionary
                fixedDict[kvp.Key.ToUpper()] = kvp.Value;
            }

            return fixedDict;
        }

        public static string FixPlaceHolderNames(string expression)
        {
            return Regex.Replace(expression, PinParameterIdentifier.ToLower(), PinParameterIdentifier, RegexOptions.IgnoreCase);
        }
    }
}