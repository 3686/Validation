﻿using System.Collections.Generic;
using System.Linq;
using Flow.Library.Actions;
using Flow.Library.Core;
using Flow.Library.Steps;
using Xunit;

namespace Flow.Console.Tests
{
    public class FlowInstanceResumeTests
    {
        private static FlowTemplate GetFlowTemplate()
        {
            var template = new FlowTemplate { Id = 100, Name = "Example Flow" };
            template.Variables.Add("Name", string.Empty);
            template.Variables.Add("Age", (int?)null);
            template.Steps = new List<StepBase>
            {
                new StartStepBase {Id = 0, MaximumNodes = int.MaxValue, MinimumNodes = 1, Name = "Start"},
                new DataCollectionStep {Id = 1, MaximumNodes = int.MaxValue, Name = "Collect Data", VersionId = 11 },
                new DataCollectionStep {Id = 2, MaximumNodes = int.MaxValue, Name = "Collect Data 2", VersionId = 22 },
                new StoreDataStep {Id = 3, MaximumNodes = int.MaxValue, MinimumNodes = 1, Name = "Store Collected Data", VersionId = 33 },
                new StepBase {Id = 4, MaximumNodes = int.MaxValue, Name = "Send Notification Email", VersionId = 44 },
                new StopStepBase {Id = 5, MaximumNodes = int.MaxValue, Name = "Stop"}
            };
            return template;
        }

        [Fact]
        public void step_notifies_instance_when_step_complete()
        {
            // assemble
            var template = GetFlowTemplate();
            var instance = new FlowInstance(template);
            var variables = new Dictionary<string, object>();

            // act
            var listOfCompletedSteps = new List<CompletedStep> { new CompletedStep { StepId = 1, StepVersion = 11 } };
            instance.Resume(listOfCompletedSteps, variables);

            var completedSteps = instance.CompletedSteps();
            var todoSteps = instance.NextSteps();
            Assert.Equal(2, completedSteps.Count);
            Assert.Equal(1, todoSteps.Count);
            Assert.Equal(2, todoSteps.First().Id);
            Assert.Equal(22, todoSteps.First().VersionId);

            var step = instance.NextSteps().First();
            step.Initialise();
            step.Process(instance, null);
            
            completedSteps = instance.CompletedSteps();
            todoSteps = instance.NextSteps();
            Assert.Equal(3, completedSteps.Count);
            Assert.Equal(1, todoSteps.Count);
            Assert.Equal(3, todoSteps.First().Id);
            Assert.Equal(33, todoSteps.First().VersionId);
        }

        [Fact]
        public void instance_resumed()
        {
            // assemble
            var template = GetFlowTemplate();
            var variables = new Dictionary<string, object>();
            var instance = new FlowInstance(template);

            // act
            var listOfCompletedSteps = new List<CompletedStep> { new CompletedStep { StepId = 0 }, new CompletedStep { StepId = 1, StepVersion = 11 } };
            instance.Resume(listOfCompletedSteps, variables);

            // assert
            var completedSteps = instance.CompletedSteps();
            var todoSteps = instance.NextSteps();
            Assert.Equal(2, completedSteps.Count);
            Assert.Equal(1, todoSteps.Count);
            Assert.Equal(2, todoSteps.First().Id);
            Assert.Equal(22, todoSteps.First().VersionId);
        }
    }
}
