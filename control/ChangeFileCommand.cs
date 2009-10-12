using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;
using PureMVC.Interfaces;
using SlimTimer.model;

namespace SlimTimer.control
{
    class ChangeFileCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification); 
            //log("onChangeFile");
            if (ignoredProject)
            {
                log("ignoredProject");
                return;
            }
            if (timeEntry == null)
            {
                log("no timeentry");
                return;
            }
            if (fileComments)
            {
                //string fileName = PathHelper.GetShortPathName(PluginBase.MainForm.CurrentDocument.FileName);
                string fileName = PluginBase.MainForm.CurrentDocument.FileName;
                //fileName = fileName.Substring(fileName.LastIndexOf("\\"),fileName.Length);
                log("fileName " + fileName);
                if (comments.IndexOf(fileName) != -1)
                {
                    log("already contains task");
                    return;
                }
                comments = comments + fileName + "\n\r";
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
