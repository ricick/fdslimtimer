using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;
using System.IO;
using PluginCore;

namespace SlimTimer.control
{
    class ChangeFileCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            ITabbedDocument file = notification.Body as ITabbedDocument;
            SettingsProxy settingsProxy = Facade.RetrieveProxy(SettingsProxy.NAME) as SettingsProxy;
            StatusProxy statusProxy = Facade.RetrieveProxy(StatusProxy.NAME) as StatusProxy;
            TaskProxy taskProxy = Facade.RetrieveProxy(TaskProxy.NAME) as TaskProxy;


            base.Execute(notification); 
            //log("onChangeFile");
            if (statusProxy.IgnoredProject)
            {
                //log("ignoredProject");
                return;
            }
            if (taskProxy.CurrentTimeEntry == null)
            {
                //log("no timeentry");
                return;
            }
            if (settingsProxy.FileComments)
            {
                //string fileName = PathHelper.GetShortPathName(PluginBase.MainForm.CurrentDocument.FileName);
                //string fileName = PluginBase.MainForm.CurrentDocument.FileName;
                string fileName = file.FileName;
                //fileName = fileName.Substring(fileName.LastIndexOf("\\"),fileName.Length);
                //log("fileName " + fileName);
                if (taskProxy.Comments.IndexOf(fileName) != -1)
                {
                    //log("already contains task");
                    return;
                }
                taskProxy.Comments += fileName + "\n\r";
            }
            /*
            if (timeEntry.Tags.Contains(fileName))
            {
               log("already contains task");
                return;
            }
            timeEntry.Tags.Add(fileName);
             */
        }

    }
}
