//using System.Collections.Generic;
//using System.Linq;
//using Flow.Library.flowTemplateActions;
//using Flow.Library.Core;
//using Flow.Library.Steps;

//namespace Flow.Library.Runners
//{
//    public class WebApiFlowRunner : FlowRunner
//    {
//        public new string Name
//        {
//            get { return "Web API runner"; }
//        }
//        public WebApiFlowRunner(FlowInstance instance) : base(instance)
//        {
//            Types.Add(typeof(CollectDataStep));
//            Types.Add(typeof(StoreDataStep));
//        }

//        public List<StepBase> NotCompleteSteps()
//        {
//            return (from o in FlowInstance.Steps where o.IsComplete == false select o).ToList();
//        }

//        public new ActionBase ProcessSteps()
//        {
//            ActionBase result = null;

//            while (result == null && NotCompleteSteps().Any())
//            {
//                // a flow instance would have just been loaded from the database
//                // so it would know it's current state

//                // if we can't handle this type of flow, we return Unhandlable
//                var step = (from o in FlowInstance.Steps where o.IsComplete == false select o).First();
//                if (!CanProcess(step.GetType()))
//                    return new UnhandlableAction { Step = step };

//                if (step.GetType() == typeof(CollectDataStep))
//                {
//                    result = HandleDataCollectionStep(step);
//                }
//                else if(step.GetType() == typeof(StoreDataStep))
//                {
//                    result = HandleStoreDataStep(step);
//                }
//                else
//                {
//                    if (!step.IsInitialized)
//                        step.Initialise();

//                    // fall back handling
//                    step.Process(FlowInstance, this);
//                }
//            }

//            if(result == null)
//                result = new NoAction();

//            return result;
//        }

//        private ActionBase HandleDataCollectionStep(StepBase step)
//        {
//            var dataCollectionStep = (CollectDataStep)step;

//            if (!dataCollectionStep.IsInitialized)
//                dataCollectionStep.Initialise();

//            if (!dataCollectionStep.IsProcessed)
//                dataCollectionStep.Process(FlowInstance, this);

//            // TODO: fix annoying cast
//            if (!dataCollectionStep.CanContinue)
//                return new CollectDataAction { Step = step };

//            return null;
//        }

//        private ActionBase HandleStoreDataStep(StepBase step)
//        {
//            var storeDataStep = (StoreDataStep) step;

//            if(!storeDataStep.IsInitialized)
//                storeDataStep.Initialise();

//            if(!storeDataStep.IsProcessed)
//                storeDataStep.Process(FlowInstance, this);

//            if (!storeDataStep.CanContinue)
//                return new StoreDataAction {Step = step};

//            return null;
//        }
//    }
//}