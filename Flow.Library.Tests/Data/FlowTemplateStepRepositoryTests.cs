﻿using System;
using System.Data.SqlClient;
using System.Linq;
using Flow.Library.Data;
using Flow.Library.Data.Abstract;
using Flow.Library.Data.Repositories;
using Xunit;
using FlowTemplateStep = Flow.Library.Core.FlowTemplateStep;

namespace Flow.Library.Tests.Data
{
    public class FlowTemplateStepRepositoryTests : IDisposable
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        private const string LocalConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=""|DataDirectory|\Sample Data\LocalDbTests.mdf"";Integrated Security=True";
        
        private readonly IRepository<IFlowTemplateStep> _repository;
        private readonly FlowDataContext _context;

        public FlowTemplateStepRepositoryTests()
        {
            _connection = new SqlConnection(LocalConnectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            using (var command = new SqlCommand(@"INSERT INTO FlowTemplate (Id, Name) VALUES (1, 'Example Template 1');", _connection, _transaction))
            {
                command.ExecuteNonQuery();
            }
            using (var command = new SqlCommand(@"INSERT INTO FlowTemplateStep (Id, FlowTemplateId, StepTypeId, Name) VALUES (1, 1, 1, 'Example Step 1'), (2, 1, 1, 'Example Step 2');", _connection, _transaction))
            {
                command.ExecuteNonQuery();
            }

            _transaction.Save("insert");
            _context = new FlowDataContext(_connection) {Transaction = _transaction};
            _repository = new FlowTemplateStepRepository(_context);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transaction.Rollback();
                _connection.Close();
            }
        }

        ~FlowTemplateStepRepositoryTests()
        {
            Dispose(false);
        }

        [Fact]
        public void Should_return_correct_amount_of_items_from_database()
        {
            var sut = _repository.Get().ToArray();

            Assert.Equal(2, sut.Count());
            Assert.Equal(2, sut[1].Id);
            Assert.Equal("Example Step 2", sut[1].Name);
            Assert.Equal(1, sut[1].FlowTemplateId);
        }

        [Fact]
        public void Should_return_template_steps()
        {
            var sut = _repository.Get(2);

            Assert.Equal(2, sut.Id);
            Assert.Equal("Example Step 2", sut.Name);
            Assert.Equal(1, sut.FlowTemplateId);
        }

        [Fact]
        public void Should_return_null_if_template_step_does_not_exist()
        {
            Assert.Null(_repository.Get(-1));
        }

        [Fact]
        public void Should_set_id_when_inserting_template_step()
        {
            var instance = new FlowTemplateStep { Name = "Example Template 2"};

            _repository.Add(instance);
            _repository.Save();

            Assert.Equal(3, instance.Id);
        }

        [Fact]
        public void Should_update_row_with_new_data()
        {
            _repository.Update(2, new FlowTemplateStep { Name = "Updated", FlowTemplateId = 1 });
            _repository.Save();

            var r = _context.FlowTemplateSteps;
            var sut = r.Where(o => o.Id == 2).ToList().Last();

            Assert.Equal("Updated", sut.Name);
            Assert.Equal(2, sut.Id);
        }

        [Fact]
        public void Should_throw_exception_when_updating_if_row_does_not_exist()
        {
            Assert.Throws<InvalidOperationException>(() => _repository.Update(-1, new FlowTemplateStep {Name = "Updated "}));
        }

        [Fact]
        public void Should_remove_row_from_database()
        {
            _repository.Delete(1);
            _repository.Save();
            Assert.Equal(1, _context.FlowTemplates.Count());
        }
    } 
}