using Domain.Controller;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class DomainFacade
    {
        private RequestSummaryDomainController _requestSummaryDomainController { get; set; }
        private UserSummaryDomainController _userSummaryDomainController { get; set; }
        private SystemDomainController _systemDomainController { get; set; }
        private ServerDomainController _serverDomainController { get; set; }
        private EventTypeDomainController _eventTypeDomainController { get; set; }
        private EventDomainController _eventDomainController { get; set; }
        private EmailRecipentDomainController _emailRecipentDomainController { get; set; }
        private NotificationDomainController _notificationDomainController { get; set; }

        private RequestDomainController _requestDomainController { get; set; }
    private SettingDomainController _settingDomainController { get; set; }
        
        private ActiveUsersDomainController _activeUsersDomainController { get; set; }
        
        private ServerDetailDomainController _serverDetailDomainController { get; set; }
        private ServerDetailCurrentDomainController _serverDetailCurrentDomainController { get; set; }

        public EmailRecipentDomainController EmailRecipentDomainController =>
          _emailRecipentDomainController ?? (_emailRecipentDomainController = new EmailRecipentDomainController());
        public ServerDetailCurrentDomainController ServerDetailCurrentDomainController =>
          _serverDetailCurrentDomainController ?? (_serverDetailCurrentDomainController = new ServerDetailCurrentDomainController());

        public NotificationDomainController NotificationDomainController =>
          _notificationDomainController ?? (_notificationDomainController = new NotificationDomainController());
        public RequestDomainController RequestDomainController =>
          _requestDomainController ?? (_requestDomainController = new RequestDomainController());

        public EventDomainController EventDomainController =>
           _eventDomainController ?? (_eventDomainController = new EventDomainController());

        public SettingDomainController SettingDomainController =>
           _settingDomainController ?? (_settingDomainController = new SettingDomainController());

        public ServerDomainController ServerDomainController =>
            _serverDomainController ?? (_serverDomainController = new ServerDomainController());
        public ServerDetailDomainController ServerDetailDomainController =>
           _serverDetailDomainController ?? (_serverDetailDomainController = new ServerDetailDomainController());

        public ActiveUsersDomainController ActiveUsersDomainController =>
           _activeUsersDomainController ?? (_activeUsersDomainController = new ActiveUsersDomainController());

        public EventTypeDomainController EventTypeDomainController =>
         _eventTypeDomainController ?? (_eventTypeDomainController = new EventTypeDomainController());

        public RequestSummaryDomainController RequestSummaryDomainController =>
            _requestSummaryDomainController ?? (_requestSummaryDomainController = new RequestSummaryDomainController());
        public UserSummaryDomainController UserSummaryDomainController =>
            _userSummaryDomainController ?? (_userSummaryDomainController = new UserSummaryDomainController());
        public SystemDomainController SystemDomainController =>
            _systemDomainController ?? (_systemDomainController = new SystemDomainController());

        private static DomainFacade _instance;
        public static DomainFacade Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DomainFacade();
                }
                return _instance;
            }
        }
        private DomainFacade()
        {        }

   
    
       
    }
}
