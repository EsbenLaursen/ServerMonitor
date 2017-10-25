using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public interface ICrudDomainController<T> where T : class
    {
        T Create(T entity);
        void Delete(T entity);
        void Update(T entity);
       List<T> GetAll();
    }
}
