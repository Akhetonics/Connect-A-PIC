﻿using CAP_Core.Components;
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
                string key = $"{PinParameterIdentifier}{pin.PinNumber}".ToUpper(); // e.g. "PIN1"
                map[key] = pin.IDInFlow; // we are about to calculate the outflow in that equation, therefore we need to map the IDInFlow here.
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
            if (Double.TryParse(realOrFormula, out _) == false)
            {
                return ConvertToDelegate(realOrFormula, CreateMapFromPins(allPinsInGrid), CreateMapFromSliders(allSlidersInGrid));
            }
            return null;
        }

        public static Dictionary<string,Guid> ExtractParameterGuids(string expressionDraft,
            Dictionary<string, Guid> pinParameterNameToGuidMap, Dictionary<string, Guid> sliderParameterNameToGuidMap)
        {
            var regex = new Regex(@"[A-Z]+[0-9]+");
            var matches = regex.Matches(expressionDraft);

            Dictionary<string,Guid> usedParameterGuids = new();
            foreach (Match match in matches) 
            {
                string parameterName = match.Value;
                if (pinParameterNameToGuidMap.TryGetValue(parameterName, out Guid parameterGuid) ||
                    sliderParameterNameToGuidMap.TryGetValue(parameterName, out parameterGuid))
                {
                    usedParameterGuids.TryAdd(parameterName , parameterGuid);
                }
                else
                {
                    throw new InvalidOperationException($"Parameter name '{parameterName}' could not be found in any provided parameter name to Guid map.");
                }
            }

            return usedParameterGuids;
        }
        public static ConnectionFunction ConvertToDelegate(string expressionDraft,
                Dictionary<string, Guid> pinParameterNameToGuidMap, Dictionary<string, Guid> sliderParameterNameToGuidMap)
        {
            expressionDraft = MakePlaceHoldersUpperCase(expressionDraft);
            var expression = new Expression(expressionDraft);
            ComplexMath.RegisterComplexFunctions(expression);

            // create a list of Guids for all used parameters in the correct order so that the caller can later provide the correct values from his dictionaries
            var usedParameterGuidMap = ExtractParameterGuids(expressionDraft, pinParameterNameToGuidMap, sliderParameterNameToGuidMap);
            // add all parameters to the expression for reference
            foreach(var key in usedParameterGuidMap.Keys)
            {
                expression.Parameters.Add(key, null);
            }
            
            var connectionFunction = new ConnectionFunction(
                (freshlyInsertedParameters) =>
                {
                    return ExecuteExpressionFromDraft(expressionDraft, freshlyInsertedParameters, expression);
                },
                expressionDraft,
                usedParameterGuidMap.Select(p=>p.Value).ToList()
            );

            return connectionFunction;
        }

        private static Complex ExecuteExpressionFromDraft(string expressionDraft, List<object> freshlyInsertedParameters, Expression expression)
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
        }

        public static string MakePlaceHoldersUpperCase(string expression)
        {
            expression = Regex.Replace(expression, PinParameterIdentifier.ToLower(), PinParameterIdentifier, RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, SliderParameterIdentifier.ToLower(), SliderParameterIdentifier, RegexOptions.IgnoreCase);
            return expression;
        }
    }
}