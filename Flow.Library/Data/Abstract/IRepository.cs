﻿using System.Collections.Generic;

namespace Flow.Library.Data.Abstract
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Get();
        T Get(int id);
        void Add(T item);
        void Update(int id, T instance);
        void Delete(int id);
    }

}