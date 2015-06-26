using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.Execution.Results;
using Tricentis.Automation.AutomationInstructions.TestActions;
using System.Diagnostics;
using System.Threading;

namespace BKR.Test.ToscaAPI.WaitForProcess
{
    [SpecialExecutionTaskName("WaitForProcess")]
    public class WaitForProcess : SpecialExecutionTaskEnhanced
    {
        private const short DEFAULTWAITTIME = 30;

        public WaitForProcess(Validator validator)
            : base(validator)
        { }

        public override void ExecuteTask(ISpecialExecutionTaskTestAction testAction)
        {
            IParameter IPprocess = testAction.GetParameter("Process", false, new[] { ActionMode.Input });
            IParameter IPwaittimesecs = testAction.GetParameter("WaitTimeSecs", false, new[] { ActionMode.Input });

            string process = IPprocess.GetAsInputValue().Value;
            string waittimesecsString = IPwaittimesecs.GetAsInputValue().Value;

            short waittimesecs = 0;
            bool success = Int16.TryParse(waittimesecsString, out waittimesecs);
            if (!success)
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Failed, "WaitTimeSecs has to be an integer");
                return;
            }

            if (waittimesecs == 0)
                waittimesecs = DEFAULTWAITTIME;

            Process[] pname = Process.GetProcessesByName(process);
            if (pname.Length == 0)
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Ok, string.Format("Proces {0} already ended or not started", process));
                return;
            }

            short counter = 0;
            while (counter < waittimesecs)
            {
                counter++;
                Thread.Sleep(1000);
                pname = Process.GetProcessesByName(process);
                if (pname.Length == 0)
                    break;
            }

            testAction.SetResult(SpecialExecutionTaskResultState.Ok, string.Format("Proces {0} has ended after {1} seconds", process, counter));
        }
    }
}