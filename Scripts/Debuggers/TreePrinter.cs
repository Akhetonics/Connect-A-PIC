using Godot;
using System;
using System.Text;

namespace ConnectAPIC.Scripts.Debuggers
{
    public static class TreePrinter 
    {

        public static string PrintTree(Node node, int level)
        {
            StringBuilder result = new StringBuilder();

            if (typeof(RichTextLabel).IsAssignableFrom(node.GetType())) return "";

            result.Append(GetIndentation(level * 2) + "|-- " + node.Name + " (Type: " + node.GetType().Name);

            if (typeof(Node2D).IsAssignableFrom(node.GetType()))
            {
                result.Append(", Visible: " + ((Node2D)node).Visible);
            }

            if (typeof(AnimatedSprite2D).IsAssignableFrom(node.GetType()))
            {
                result.Append(", Playing: " + ((AnimatedSprite2D)node).IsPlaying());
            }

            result.AppendLine(")");

            foreach (Node child in node.GetChildren())
            {
                result.Append(PrintTree(child, level + 1));
            }

            return result.ToString();
        }

        public static string GetIndentation(int count)
        {
            return new String(' ', count);
        }
    }
}
