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
using BKR.Test.ToscaAPI.Shared;

namespace BKR.Test.ToscaAPI.CompareFiles
{
    [SpecialExecutionTaskName("CompareFiles")]
    public class CompareFiles : SpecialExecutionTaskEnhanced 
    {
        public CompareFiles(Validator validator)
            : base(validator)
        { }

        public override void ExecuteTask(ISpecialExecutionTaskTestAction testAction)
        {
            IParameter expectedFilePathParameter = testAction.GetParameter("ExpectedFile", false, new[] { ActionMode.Input });
            IParameter actualFilePathParameter = testAction.GetParameter("ActualFile", false, new[] { ActionMode.Input });

            string expectedFilePath = Environment.ExpandEnvironmentVariables(expectedFilePathParameter.GetAsInputValue().Value);
            string actualFilePath = Environment.ExpandEnvironmentVariables(actualFilePathParameter.GetAsInputValue().Value);
            string result = String.Empty;
            try
            {
                result = FileComparer.Compare(expectedFilePath, actualFilePath);
            }
            catch (FileNotFoundException exc)
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Failed, exc.Message);
                return;
            }
            
            if (result == String.Empty)
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Ok, "The contents of the files are identical.");
            }
            else
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Failed, result);
            }
        }

    }
}
