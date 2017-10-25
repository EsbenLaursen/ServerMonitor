using Model;
using ServerMonitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class SummaryLogic
    {
        //protected static readonly DomainFacade Facade = DomainFacade.Instance;
        //public void ProcessRequest(Request request) {
        //    int interval = Facade.GetSetupInterval();
        //    bool onlinePastMinutes = false;
        //    bool onlineOnWebAndMobilePastMinutes = false;
        //
        //    //for calculation the users online the past hour
        //    int mobileLastHour = 0;
        //    int webLastHour = 0;
        //    List<UserSummary> lastMinutesUserSummaries = Facade.GetUserSummariesLastMinutes(interval);
        //
        //    foreach (var u in lastMinutesUserSummaries)
        //    {
        //        if (u.UserId == request.UserId) //Same user has logged logged in
        //        {
        //            onlinePastMinutes = true;
        //            //checking if user has changed device
        //            if (u.Device == (int)MyEnums.DeviceTypes.Mobile && request.Device == (int)MyEnums.DeviceTypes.Web
        //                || u.Device == (int)MyEnums.DeviceTypes.Web && request.Device == (int)MyEnums.DeviceTypes.Mobile)
        //            {
        //                onlineOnWebAndMobilePastMinutes = true;
        //            }
        //
        //        }
        //        if (u.Device == (int)MyEnums.DeviceTypes.Web) { webLastHour++; }
        //        if (u.Device == (int)MyEnums.DeviceTypes.Mobile) { mobileLastHour++; }
        //    }
        //    if (onlinePastMinutes)
        //    {
        //        if (!onlineOnWebAndMobilePastMinutes)
        //        {
        //            UserSummary userSummary = Facade.UserSummaryDomainController.GetUserSummeryByIdLastMinutes(interval, request.UserId, request.Device);
        //            userSummary.Request++;
        //
        //            Facade.UpdateUserSummary(userSummary);
        //
        //        }
        //        else
        //        {
        //            // Try and get usersummaries for the last x minutes for the specific user
        //            UserSummary userSummaryWeb = Facade.UserSummaryDomainController.GetUserSummeryByIdLastMinutes(interval, request.UserId, (int)MyEnums.DeviceTypes.Web);
        //            UserSummary userSummaryMobile = Facade.GetUserSummeryByIdLastMinutes(interval, request.UserId, (int)MyEnums.DeviceTypes.Mobile);
        //
        //            if (request.Device == (int)MyEnums.DeviceTypes.Web)
        //            {
        //                //if found, then update
        //                if (userSummaryWeb != null)
        //                {
        //                    userSummaryWeb.Request++;
        //                    Facade.UpdateUserSummary(userSummaryWeb);
        //                } //if not, then create
        //                else
        //                {
        //                    webLastHour++;
        //                    RequestSummary r = CreateRequestSummery(mobileLastHour, webLastHour);
        //                    CreateUserSummery(request, r);
        //                }
        //            }
        //            if (request.Device == (int)MyEnums.DeviceTypes.Mobile)
        //            {
        //                if (userSummaryMobile != null)
        //                {
        //                    userSummaryMobile.Request++;
        //                    Facade.UpdateUserSummary(userSummaryMobile);
        //                }
        //                else
        //                {
        //                    mobileLastHour++;
        //                    RequestSummary r = CreateRequestSummery(mobileLastHour, webLastHour);
        //                    CreateUserSummery(request, r);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //
        //        if (request.Device == (int)MyEnums.DeviceTypes.Web) { webLastHour++; }
        //        if (request.Device == (int)MyEnums.DeviceTypes.Mobile) { mobileLastHour++; }
        //
        //        RequestSummary requestMade = CreateRequestSummery(mobileLastHour, webLastHour);
        //        CreateUserSummery(request, requestMade);
        //    }
        //
        //}
        //private void CreateUserSummery(Request request, RequestSummary requestSummeryMade)
        //{
        //    //new user summary
        //    UserSummary userSummary = new UserSummary();
        //    userSummary.Device = request.Device;
        //    userSummary.UserId = request.UserId;
        //    userSummary.Request = 1;
        //    userSummary.RequestSummaryId = requestSummeryMade.Id;
        //
        //    Facade.CreateUserSummary(userSummary);
        //}
        //
        //private RequestSummary CreateRequestSummery(int mobileLastHour, int webLastHour)
        //{
        //    RequestSummary newRequestSummary = new RequestSummary();
        //    // newRequestSummary.DatetimeStart = request.DateCreated == null ? DateTime.Now : request.DateCreated;
        //    newRequestSummary.DateTimeCreated = DateTime.Now;
        //    newRequestSummary.MobileUsers = mobileLastHour;
        //    newRequestSummary.WebUsers = webLastHour;
        //    RequestSummary requestMade = Facade.CreateRequestSummary(newRequestSummary);
        //    return requestMade;
        //}
    }
}
