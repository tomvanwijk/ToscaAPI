using BKR.Test.ToscaAPI.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.Automation.AutomationInstructions.Configuration;
using Tricentis.Automation.AutomationInstructions.Dynamic.Values;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.Execution.Results;

namespace BKR.Test.ToscaAPI.ExecuteQueryDB
{
    [SpecialExecutionTaskName("ExecuteQueryDB")]
    public class ExecuteQueryDB : SpecialExecutionTaskEnhanced
    {

        public ExecuteQueryDB( Validator validator) : base(validator) { }

        public override void ExecuteTask(ISpecialExecutionTaskTestAction testAction)
        {
            IParameter IPconnString = testAction.GetParameter("ConnectionString", false, new[] { ActionMode.Input });
            IParameter IPquery = testAction.GetParameter("Query", false, new[] { ActionMode.Input });
            IParameter IPresult = testAction.GetParameter("Result", true, new[] { ActionMode.Verify, ActionMode.Buffer });
            
            string connString = IPconnString.GetAsInputValue().Value;
            string query = IPquery.GetAsInputValue().Value;
            

            if (File.Exists(query))
            {
                query = File.ReadAllText(query);
            }

            Datalink dl = new Datalink(connString);

            if (!dl.IsValidSqlConnectionString())
            {
                throw new InvalidOperationException("Connectionstring is not valid or the database cannot be reached");
            }

            string result = dl.Execute(query);
            if (IPresult != null )
            {
                IInputValue expectedResult = IPresult.Value as IInputValue;

                if (IPresult.ActionMode == ActionMode.Buffer)
                {
                    Buffers.Instance.SetBuffer(expectedResult.Value, result);
                    testAction.SetResult(SpecialExecutionTaskResultState.Ok, string.Format("Buffer {0} set to value {1}.",expectedResult.Value, result));
                    return;
                }
                
                if (expectedResult.Value == result)
                {
                    testAction.SetResult(SpecialExecutionTaskResultState.Ok, "Actual rows match expected number of rows.");
                }
                else
                {
                    testAction.SetResult(SpecialExecutionTaskResultState.Failed, string.Format("Expected result: {0}. Actual result {1}. Incorrect", expectedResult.Value, result.ToString()));
                }
            }
            else
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Ok, "Query executed");
            }

        }
    }
}
