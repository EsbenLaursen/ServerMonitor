using log4net;
using Model;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public class CrudDomainController<T> : ICrudDomainController<T> where T : class
    {
        public virtual List<T> GetAll()
        {
            try
            {

                using (var ctx = new DatabaseModelDataContext())
                {
                       ITable<T> table = (ITable<T>)ctx.GetTable(typeof(T));
                    var query = table.Select(x => x);
                    List<T> list = query.ToList();
                    return list;
                }
              
                
            }
            catch (Exception e)
            {
                Log.Error("Database error - CrudDomainController - GetAll: " + e);
                throw;
            }

        }
        public T Create(T entity)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    ITable<T> table = (ITable<T>)ctx.GetTable(typeof(T));
                    table.InsertOnSubmit(entity);
                    ctx.SubmitChanges();
                    return entity;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - CrudDomainController - Create: ", e);
                throw;
            }
        }


        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity) //TODO
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    
                    ctx.SubmitChanges();
                    //ITable<T> table = (ITable<T>)ctx.GetTable(typeof(T));
                    //table.Select(i => i).Select(i => i).FirstOrDefault(x => x. == source.Id);
                    //            GetTargetFromDb(ctx, source);
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - CrudDomainController - Update: ", e);
                throw;
            }
            
        }




        protected ILog Log
        {
            get { return LogManager.GetLogger(GetType()); }
        }

        protected bool UpdateAllowed(PropertyInfo prop)
        {
            bool primitive = new[] {typeof(string),
                                    typeof(char),
                                    typeof(byte),
                                    typeof(sbyte),
                                    typeof(ushort),
                                    typeof(short),
                                    typeof(uint),
                                    typeof(int),
                                    typeof(int?),
                                    typeof(ulong),
                                    typeof(long),
                                    typeof(float),
                                    typeof(double),
                                    typeof(decimal),
                                    typeof(bool),
                                    typeof(DateTime),
                                    typeof(DateTime?),
                                    typeof(TimeSpan),
                                    typeof(TimeSpan?),
                                    typeof(Guid),
                                    typeof(Guid?),
                                    typeof(decimal?)}.Contains(prop.PropertyType);

            return prop.CanWrite
                && primitive
                && !prop.Name.Equals("Id")
                && !prop.Name.Equals("DateTimeCreated")
                && !prop.Name.Equals("Created");
        }


        
    }
}
