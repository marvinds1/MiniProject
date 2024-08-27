using System;
using Microsoft.EntityFrameworkCore;
using Persistence.DatabaseContext;

namespace Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly TableContext _context;

    public GenericRepository(TableContext context)
    {
        _context = context;
    }

    public List<T> GetAll()
    {
        return _context.Set<T>().ToList();
    }

    public T? GetById(Guid id)
    {
        return _context.Set<T>().Find(id);
    }

    public async Task<List<T>> AddBatch(List<T> entities)
    {
        foreach (T entity in entities)
        {
            _context.Set<T>().Add(entity);
        }

        await _context.SaveChangesAsync();

        return entities;
    }
}