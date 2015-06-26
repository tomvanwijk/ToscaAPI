using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.Execution.Results;

namespace BKR.Test.ToscaAPI.FileExists
{
    [SpecialExecutionTaskName("FileExists")]
    public class FileExists : SpecialExecutionTaskEnhanced
    {
        public FileExists(Validator validator)
            : base(validator)
        { }
        public override void ExecuteTask(ISpecialExecutionTaskTestAction testAction)
        {
            IParameter IPfile = testAction.GetParameter("File", false, new[] { ActionMode.Input });
            IParameter IPinverse = testAction.GetParameter("Exists", false, new[] { ActionMode.Input });

            string file = IPfile.GetAsInputValue().Value;
            string inverseString = IPinverse.GetAsInputValue().Value;

            bool fileExists; // if this is true, the file should exists. Default value should be true
            bool result;

            bool success = Boolean.TryParse(inverseString, out fileExists);
            if (!success)
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Failed, "Inverse has to be an boolean");
                return;
            }

            if (FileOrDirectoryExists(file))
            {
                result = fileExists; //if the file is found, the value of fileExists will be returned. 
            }
            else
            {
                result = !fileExists; //if the file is not found, the inverse of fileExists will be returned.
            }

            if (result)
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Ok, "Correct");
                return;
            }

            testAction.SetResult(SpecialExecutionTaskResultState.Failed, "Incorrect");
        }

        internal static bool FileOrDirectoryExists(string name)
        {
            return (Directory.Exists(name) || File.Exists(name));
        }
    }
}
