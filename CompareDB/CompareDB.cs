using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tricentis.Automation.Creation;
using Tricentis.Automation.Engines.SpecialExecutionTasks;
using Tricentis.Automation.Engines.SpecialExecutionTasks.Attributes;
using Tricentis.Automation.AutomationInstructions.TestActions;
using Tricentis.Automation.AutomationInstructions.Dynamic.Values;
using Tricentis.Automation.Execution.Results;
using System.Data;
using BKR.Test.ToscaAPI.Shared;
using Tricentis.Automation.AutomationInstructions.Configuration;

namespace BKR.Test.ToscaAPI.CompareDB
{
    [SpecialExecutionTaskName("CompareDB")]
    public class CompareDB : SpecialExecutionTaskEnhanced
    {
        public CompareDB(Validator validator) : base(validator) { }

        public override void ExecuteTask(ISpecialExecutionTaskTestAction testAction)
        {
            const string BEGINWHERE = " where 1=1 ";
            const string BEGINSELECT = " select ";
            const string BEGINORDER = " order by ";
            const string ORDERDIRECTION = " desc ";

            var connStringParam = testAction.SearchQuery.XParameters.ConfigurationParameters.FirstOrDefault( xParam => xParam.Name == "ConnectionString");
            string connString = connStringParam.UnparsedValue;

            var tableNameParam = testAction.SearchQuery.XParameters.ConfigurationParameters.FirstOrDefault(xParam => xParam.Name == "TableName");
            string tableName = tableNameParam.UnparsedValue;

            var orderAttribParam = testAction.SearchQuery.XParameters.ConfigurationParameters.FirstOrDefault(xParam => xParam.Name == "OrderAttribute");
            string orderAttrib = orderAttribParam.UnparsedValue;

            string queryWhere = BEGINWHERE;
            string queryBegin = BEGINSELECT;
            string query = string.Empty;
            string queryOrder = string.Format("{0} {1} {2}", BEGINORDER, orderAttrib, ORDERDIRECTION);
            int count = 0;

            var constraints = testAction.Parameters.FindAll(p => p.ActionMode == ActionMode.Constraint);
            if (constraints.Count > 0)
            {
                foreach (var parameter in constraints)
                {
                    IInputValue value = parameter.Value as IInputValue;
                    queryWhere += string.Format(" and {0} = '{1}' ", parameter.Name, value.Value);
                }
            }

            var buffersVerifies = testAction.Parameters.FindAll(p => p.ActionMode == ActionMode.Verify || p.ActionMode == ActionMode.Buffer);

            
            if (buffersVerifies.Count == 0 )
            {
                // No use in continuing if there is nothing to verify or buffer
                return;
            }
           
            queryBegin += String.Join(",", buffersVerifies.Select(p => p.Name).ToArray());
            
            // Generate db query
            query = string.Format("{0} FROM {1} {2} {3}", queryBegin, tableName, queryWhere, queryOrder);

            // Connect to the database
            Datalink dl = new Datalink();
            dl.ConnectionString = connString;
            if (!dl.IsValidSqlConnectionString()) 
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Failed, string.Format("Cannot connect to the SQL database with the following connection string: {0}", connString));
                return;
            }

            DataRow dr = dl.GetRowFromDb(query);
            // If there is not a result from the database, set the testresult to failed
            if (dr == null)
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Failed, "No records found in with selected searchcriteria" );
                return;
            }

            foreach (IParameter parameter in buffersVerifies.Where(p => p.ActionMode == ActionMode.Verify))
            {                      
                IInputValue value = parameter.Value as IInputValue;
                if (value.Value == dr[parameter.Name].ToString())
                {
                    testAction.SetResultForParameter(parameter, SpecialExecutionTaskResultState.Ok, string.Format("{0}: Expected value {1} matches actual value {2}.", parameter.Name, value.Value, dr[parameter.Name].ToString()));
                }
                else
                {
                    count++;
                    testAction.SetResultForParameter(parameter, SpecialExecutionTaskResultState.Failed, string.Format("Error {0}: Expected value {1} does not match actual value {2}.", parameter.Name, value.Value, dr[parameter.Name].ToString()));
                }
            }

            foreach (IParameter parameter in buffersVerifies.Where(p => p.ActionMode == ActionMode.Buffer))
            {
                IInputValue buffer = parameter.Value as IInputValue;
                Buffers.Instance.SetBuffer(buffer.Value, dr[parameter.Name].ToString());
                testAction.SetResultForParameter(parameter, SpecialExecutionTaskResultState.Ok, string.Format("Buffer {0} set to value {1}.", buffer.Value, dr[parameter.Name].ToString()));
            }

            if (count == 0)
            {
                testAction.SetResult(SpecialExecutionTaskResultState.Ok, "All values match.");
            }
            else
            {
                int numberOfVerifies = buffersVerifies.Count(p => p.ActionMode == ActionMode.Verify);
                testAction.SetResult(SpecialExecutionTaskResultState.Failed, string.Format("{0} out of {1} values match.", numberOfVerifies - count, numberOfVerifies));
            }
        }
    }
}
