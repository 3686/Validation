﻿using Flow.Library.Core;
using Flow.Library.Data.Abstract;
using Flow.Library.Steps;
using Flow.Library.Validation;
using Flow.Library.Validation.Rules;
using System.Collections.Generic;

namespace Flow.Library.Data.Repositories
{
    public class ExampleFlowInstanceRepository : IFlowInstanceRepository
    {
        public FlowInstance GetFlow(int flowId, int instanceId)
        {
            var template = new FlowTemplate();
            template.Variables.Add("yourName", string.Empty);
            template.Steps.Add(new DataCollectionStep
            {
                ExitRules = new List<ValidationRule> { new ValidationRule { Key = "yourName", Validator = new StringRequired() } }
            });
            template.Steps.Add(new StoreDataStep());
            return new FlowInstance(template);;
        }
    }
}