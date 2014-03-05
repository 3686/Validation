﻿using Flow.Library.Data.Abstract;

namespace Flow.Library.Core
{
    public class FlowInstanceFactory
    {
        private readonly IFlowTemplateRepository _templateRepository;
        private readonly IFlowInstanceRepository _instanceRepository;

        public FlowInstanceFactory(IFlowTemplateRepository templateRepository, IFlowInstanceRepository instanceRepository)
        {
            _templateRepository = templateRepository;
            _instanceRepository = instanceRepository;
        }

        public FlowInstance Create(int templateId)
        {
            var template = _templateRepository.GetTemplate(templateId);
            var instance = new FlowInstance {Template = template, Variables = template.Variables};
            return instance;
        }

        public FlowInstance Restore(int flowId)
        {
            var instance = _instanceRepository.GetFlow(flowId);
            var template = _templateRepository.GetTemplate(instance.Template.Id);
            instance.Template = template;

            foreach (var variable in template.Variables)
            {
                if (!instance.Variables.ContainsKey(variable.Key))
                {
                    instance.Variables.Add(variable.Key, variable.Value);
                }
            }

            return instance;
        }
    }
}