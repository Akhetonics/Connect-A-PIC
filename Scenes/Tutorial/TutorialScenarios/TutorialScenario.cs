using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


abstract partial class TutorialScenario : Node
{
    public abstract void SetupTutorial();
    public abstract void GoToNext();
    public abstract void QuitTutorial();
    public abstract void ResetTutorial();
    public abstract bool IsNextConditionSatisfied();
    public abstract bool GoToNextIfNextConditionSatisfied();
}

