using ApiCodeMaze.Dominio.Entidades;
using ApiCodeMaze.Dominio.Repositorios;
using ApiCodeMaze.Repositorio.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApiCodeMaze.Repositorio.Repositorios
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected Context _context { get; set; }

        public Repository(Context context)
        {
            _context = context;
        }

        public IQueryable<T> FindAll()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}
