﻿namespace Gu.Settings.Core
{
    using System.Threading.Tasks;

    public interface IGenericAsyncRepository : IGenericRepository
    {
        /// <summary>
        /// <see cref="IRepository.ReadAsync{T}(T)"/>
        /// </summary>
        Task<T> ReadAsync<T>();

        /// <summary>
        /// <see cref="IRepository.SaveAsync{T}(T)"/>
        /// </summary>
        Task SaveAsync<T>(T item);
    }
}