using Godot;
using System;

namespace ConnectAPIC.Scripts.Debuggers
{
    public static class TreePrinter 
    {
     
        public static void PrintTree(Node node, int level)        {
            
            if(typeof(RichTextLabel).IsAssignableFrom(node.GetType())) return;

            var NodeInfo = GetIndentation(level * 2) + "|-- " + node.Name + " (Type: " + node.GetType().Name;
            if(typeof(Node2D).IsAssignableFrom(node.GetType()))
            {
                NodeInfo += ", Visible: " + ((Node2D)node).Visible + ")";
            }
            if(typeof(AnimatedSprite2D).IsAssignableFrom(node.GetType()))
            {
                NodeInfo += ", Playing: " + ((AnimatedSprite2D)node).IsPlaying() + ")";
            }
            CustomLogger.PrintLn(NodeInfo, true);

            foreach (Node child in node.GetChildren())
            {
                PrintTree(child, level + 1);
            }
        }

        public static string GetIndentation(int count)
        {
            return new String(' ', count);
        }
    }
}
