﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Flow.Library.Data;
using Flow.Library.Data.Abstract;
using Flow.Library.Steps;
using Flow.Library.Validation;
using Flow.Web.Dto;
using FlowTemplate = Flow.Library.Core.FlowTemplate;

namespace Flow.Web.Controllers.Api
{
    public class FlowTemplatesController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FlowTemplateService _flowTemplateService;

        public FlowTemplatesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _flowTemplateService = new FlowTemplateService();
        }

        [NullResponseIs404]
        public FlowTemplateDto Get(int id)
        {
            var flow = _flowTemplateService.GetFlowTemplate(_unitOfWork, id);
            if (flow == null)
                return null;

            var mappedDtoFlowTemplate = Map<FlowTemplateDto>(flow);
            return mappedDtoFlowTemplate;
        }

        public HttpResponseMessage Post(FlowTemplateDto flowTemplateDto)
        {
            var steps = new List<IStep>();
            if (flowTemplateDto.Steps != null && flowTemplateDto.Steps.Any())
            {
                steps = flowTemplateDto.Steps.Select(Map<IStep>).ToList();
            }

            var template = new FlowTemplate
            {
                Id = flowTemplateDto.Id,
                Name = flowTemplateDto.Name,
                Steps = steps
            };

            var id = _flowTemplateService.Add(_unitOfWork, template);
            return Request.CreateResponse(HttpStatusCode.Created, new {Id = id});
        }

        protected static T Map<T>(object source)
        {
            try
            {
                return AutoMapper.Mapper.Map<T>(source);
            }
            catch (AutoMapper.AutoMapperMappingException ex)
            {
                throw new NotSupportedException("Step is unknown", ex);
            }
        }

        public void Put(FlowTemplateDto flowTemplateDto)
        {

            var steps = new List<IStep>();
            if (flowTemplateDto.Steps != null && flowTemplateDto.Steps.Any())
            {
                foreach (var step in flowTemplateDto.Steps)
                {
                    var mappedStep = Map<IStep>(step);
                    if (step.Id > 0)
                    {
                        var match = _unitOfWork.FlowTemplateSteps.Get(mappedStep.Id);
                        if (match == null)
                        {
                            throw new ValidationException(String.Format("Step with Id {0} does not exist. Cannot update step.", step.Id));
                        }
                        mappedStep.IsDirty = true;
                    }
                    steps.Add(mappedStep);
                }
            }

            var templateResult = new FlowTemplate
            {
                Id = flowTemplateDto.Id,
                Name = flowTemplateDto.Name,
                Steps = steps,
            };

            _flowTemplateService.Update(_unitOfWork, templateResult);
        }

        public HttpStatusCodeResult Delete(int id)
        {
            if(_unitOfWork.FlowTemplates.Get(id) == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _flowTemplateService.Delete(_unitOfWork, new FlowTemplate { Id = id });
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public IEnumerable<FlowTemplateDto> Get()
        {
            var flowTemplates = _flowTemplateService.GetFlowTemplates(_unitOfWork).ToList();
            var mapped = flowTemplates.Select(AutoMapper.Mapper.Map<FlowTemplateDto>).ToList();
            return mapped;
        }

    }
}
