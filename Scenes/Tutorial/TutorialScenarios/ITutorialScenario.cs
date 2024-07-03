using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


interface ITutorialScenario
{
    public void SetupTutorial();
    public void GoToNext();
    public void QuitTutorial();
    public void ResetTutorial();
    public bool IsNextConditionSatisfied();
    public bool GoToNextIfNextConditionSatisfied();
}

