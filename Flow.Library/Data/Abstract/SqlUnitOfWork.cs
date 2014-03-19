﻿using System;
using System.Data;

namespace Flow.Library.Data.Abstract
{
    public class SqlUnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbConnection _connection;
        private readonly IDbTransaction _transaction;

        public SqlUnitOfWork(IDbConnection connection)
        {
            _connection = connection;
            _transaction = connection.BeginTransaction();
        }

        public IRepository<Core.FlowTemplate> FlowTemplates { get; set; }

        public IRepository<Core.FlowTemplateStep> FlowTemplateSteps { get; set; }

        public IRepository<Core.FlowTemplateStepRule> FlowTemplateStepRules { get; set; }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~SqlUnitOfWork()
        {
            Dispose(false);
        }

        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                _transaction.Dispose();
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}