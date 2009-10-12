using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using SlimTimer.control;
namespace SlimTimer
{
    class ApplicationFacade : Facade
    {
        public static String APPLICATION_STARTUP = "APPLICATION_STARTUP";
        public static String APPLICATION_SHUTDOWN = "APPLICATION_SHUTDOWN";
        public static String SET_CURRENT_TASK = "SET_CURRENT_TASK";
        public static String GET_TASKS = "GET_TASKS";
        public static String LOGIN = "LOGIN";
        public static String MODEL_PREP = "MODEL_PREP";
        public static String PAUSE = "PAUSE";
        public static String RESUME = "RESUME";
        public static String SAVE_TIME_ENTRY = "SAVE_TIME_ENTRY";
        public static String SELECT_TRACKING = "SELECT_TRACKING";
        public static String RESET_TIMEOUT = "RESET_TIMEOUT";
        protected static ApplicationFacade instance;
        protected ApplicationFacade() { }

        static ApplicationFacade()
        {
            instance = new ApplicationFacade();
        }
        public static ApplicationFacade getInstance()
        {
            return instance as ApplicationFacade;
        }

        protected override void  InitializeController()
        {
            base.InitializeController();
            RegisterCommand(APPLICATION_STARTUP, typeof(ApplicationStartupCommand));
            RegisterCommand(APPLICATION_SHUTDOWN, typeof(ApplicationShutdownCommand));
            RegisterCommand(SET_CURRENT_TASK, typeof(SetCurrentTaskCommand));
            RegisterCommand(GET_TASKS, typeof(GetTasksCommand));
            RegisterCommand(LOGIN, typeof(LoginCommand));
            RegisterCommand(MODEL_PREP, typeof(ModelPrepCommand));
            RegisterCommand(PAUSE, typeof(PauseCommand));
            RegisterCommand(RESUME, typeof(ResumeCommand));
            RegisterCommand(SAVE_TIME_ENTRY, typeof(SaveTimeEntryCommand));
            RegisterCommand(SELECT_TRACKING, typeof(SelectTracking));
            RegisterCommand(RESET_TIMEOUT, typeof(ResetTimeoutCommand));
        }
    }
}
