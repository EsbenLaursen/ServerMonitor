using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Controller
{
    public class UserSummaryDomainController : CrudDomainController<UserSummary>
    {
        public List<UserSummary> GetUserSummaries()
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var userSummaries = ctx.UserSummaries.ToList();
                    return userSummaries;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetUserSummaries: " + e);
                throw;
            }

        }

        public void CreateUserSummary(UserSummary userSummary)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    userSummary.DateTimeCreated = DateTime.Now;
                    ctx.UserSummaries.InsertOnSubmit(userSummary);
                    ctx.SubmitChanges();
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - CreateUserSummary: " + e);
                throw;
            }
        }

        public List<UserSummary> GetUserSummariesByRange(DateTime from, DateTime to)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var userSummaries = ctx.UserSummaries.Where(x => x.DateTimeCreated > from && x.DateTimeCreated < to).ToList();
                    return userSummaries;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetUserSummariesByRange: " + e);
                throw;
            }
        }

        public List<UserSummary> GetUserSummariesByRequestSummary(RequestSummary requestSummary)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var userSummaries = ctx.UserSummaries.Where(x=>x.RequestSummaryId == requestSummary.Id).ToList();
                    return userSummaries;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetUserSummaries: " + e);
                throw;
            }
        }

        public void UpdateUserSummary(UserSummary userSummary)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    UserSummary s = ctx.UserSummaries.FirstOrDefault(x => x.Id == userSummary.Id);
                    if (s != null)
                    {
                        foreach (PropertyInfo prop in typeof(UserSummary).GetProperties())
                        {
                            if (UpdateAllowed(prop))
                            {
                                prop.SetValue(s, prop.GetValue(userSummary, null));
                            }

                        }
                        ctx.SubmitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - UpdateUserSummary: " + e);
                throw;
            }
        }


        

        public UserSummary GetUserSummeryByUserAndRequestSummary(int userId, int device, int requstSummaryId)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    return ctx.UserSummaries.Where(k => requstSummaryId == k.RequestSummaryId)
                        .FirstOrDefault(x => x.UserId == userId && x.Device == device);
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetUserSummeryByUserAndRequestSummary: " + e);
                throw;
            }
        }

        public UserSummary GetUserSummeryByIdLastMinutes(int interval, int userId, int device)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var userSummery = ctx.UserSummaries.Where(k => k.DateTimeCreated > DateTime.Now.AddMinutes(-interval))
                                                        .FirstOrDefault(x => x.UserId == userId && x.Device == device);

                    return userSummery;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetUserSummeryByIdLastMinutes: " + e);
                throw;
            }
        }

        public List<UserSummary> GetAllUserSummeriesLastMinutes(int interval)
        {
            try
            {
                using (var ctx = new DatabaseModelDataContext())
                {
                    var userSummery = ctx.UserSummaries.Where(k => k.DateTimeCreated > DateTime.Now.AddMinutes(-interval)).ToList(); 
                    return userSummery;
                }
            }
            catch (Exception e)
            {
                Log.Error("Database error - GetUserSummeryByIdLastMinutes: " + e);
                throw;
            }
        }
    }
}
