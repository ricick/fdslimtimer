using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Inikus.SlimTimer
{
    /// <summary>
    /// SlimTimer API Class
    /// Wraps the HTTP calls to SlimTimer in a C# class
    /// http://www.slimtimer.com/help/api
    /// Copyright (C) 2007 Inikus Consulting (http://www.inikus.com)
    /// 
    /// This library is free software; you can redistribute it and/or
    /// modify it under the terms of the GNU Lesser General Public
    /// License as published by the Free Software Foundation; either
    /// version 2.1 of the License, or (at your option) any later version.
    /// 
    /// This library is distributed in the hope that it will be useful,
    /// but WITHOUT ANY WARRANTY; without even the implied warranty of
    /// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    /// Lesser General Public License for more details.
    /// 
    /// You should have received a copy of the GNU Lesser General Public
    /// License along with this library; if not, write to the Free Software
    /// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
    /// 
    /// Send any feedback about this API to slimtimer@inikus.com
    /// </summary>
    public class SlimTimerApi
    {
        #region Declarations

        [Flags]
        public enum TaskFilters {Owner = 1, Coworker = 2, Reporter = 4};
        public enum RequestType { Get, Post, Put, Delete };
        public enum ContentType { Xml, Yaml };
        public enum ShowCompletedTask {Yes, No, Only};

        private string _ApiKey = "";
        private string _UserName;
        private string _Password;
        private string _AccessToken;
        private string _UserID;
        private string _Error;
        private int _BufferSize = 1024;
        private const string BASE_URL = "http://www.slimtimer.com";
        private CookieCollection _CC;

        #endregion

        #region Properties

        /// <summary>
        /// Returns any error messages from the last call
        /// </summary>
        public string ErrorMessage
        {
            get { return _Error; }
        }

        /// <summary>
        /// Sets the API Key
        /// </summary>
        public string ApiKey
        {
            get { return _ApiKey; }
            set { _ApiKey = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="userName">SlimTimer UserName (email)</param>
        /// <param name="password">SlimTimer Password</param>
        public SlimTimerApi(string userName, string password)
        {
            _UserName = userName;
            _Password = password;
        }

        /// <summary>
        /// Constructor with the API Key parameter
        /// </summary>
        /// <param name="userName">SlimTimer UserName (email)</param>
        /// <param name="password">SlimTimer Password</param>
        /// <param name="ApiKey">Developers SlimTimer API Key</param>
        public SlimTimerApi(string userName, string password, string apiKey)
        {
            _UserName = userName;
            _Password = password;
            _ApiKey = apiKey;
        }

        #endregion

        #region GetHttpPage
        /// <summary>
        /// Navigates to a HTTP page
        /// </summary>
        /// <param name="url">web url to retrieve</param>
        /// <returns>HTML response from server</returns>
        public string GetHttpPage(Uri url)
        {
            return GetHttpPage(url, RequestType.Get, ContentType.Xml, null);
        }

        /// <summary>
        /// Navigates to a HTTP Page
        /// </summary>
        /// <param name="url">web url to retrieve</param>
        /// <param name="requestType">Request Type, GET/POST/PUT/DELETE</param>
        /// <param name="contentType">This is for POST's only, either XML or YAML</param>
        /// <param name="postData">Data to POST, if a POST is being performed, otherwise this is ignored</param>
        /// <returns>HTML response from server</returns>
        public string GetHttpPage(Uri url, RequestType requestType, ContentType contentType, string postData)
        {

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.Timeout = 15000;
            wr.ServicePoint.Expect100Continue = false;
            wr.KeepAlive = false;
            wr.Accept = "application/xml";
            wr.ReadWriteTimeout = 35000;
            wr.Method = requestType.ToString().ToUpper();

            wr.CookieContainer = new CookieContainer();
            if (_CC != null)
                wr.CookieContainer.Add(_CC);

            // if we have data to post, then be sure to send the data properly
            if (postData != null && postData.Length > 0)
            {
                Encoding encoding = Encoding.UTF8;
                byte[] outByte = encoding.GetBytes(postData);
                // Set the content type of the data being posted.
                if (contentType == ContentType.Xml)
                    wr.ContentType = "application/xml";
                else if (contentType == ContentType.Yaml)
                    wr.ContentType = "application/x-yaml";
                // Set the content length of the string being posted.
                wr.ContentLength = outByte.Length;
                Stream outStream = wr.GetRequestStream();
                outStream.Write(outByte, 0, outByte.Length);

                // Close the Stream object.
                outStream.Flush();
                outStream.Close();
            }

            // get the response back from the server
            HttpWebResponse response = null;
            string Result = string.Empty;
            try
            {
                response = (HttpWebResponse)wr.GetResponse();
                if ((wr.CookieContainer.Count > 0))
                    _CC = wr.CookieContainer.GetCookies(wr.RequestUri);
                Result = ReadResponseStream(response);
            }
            catch (WebException webEx)
            {
                _Error = ReadResponseStream((HttpWebResponse)webEx.Response);
                _Error = ParseError(_Error);
                // re-throw the error so the user knows there was an issue
                throw;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return Result;
        }

        private string ReadResponseStream(HttpWebResponse response)
        {
            StringBuilder Result = new StringBuilder(1024);
            Stream st = response.GetResponseStream();
            try
            {
                byte[] b = new byte[_BufferSize];
                int count;
                do
                {
                    count = st.Read(b, 0, b.Length);
                    Result.Append(ASCIIEncoding.ASCII.GetString(b, 0, count));
                } while (count > 0);
            }
            finally
            {
                st.Flush();
                st.Close();
            }
            return Result.ToString();
        }

        #endregion

        #region Logon

        /// <summary>
        /// Logs into SlimTimer using the UserName and Password passed in on object creation
        /// </summary>
        /// <returns>True if login is successfull, otherwise False</returns>
        public bool Logon()
        {
            if (_AccessToken != null && _AccessToken.Length > 0)
                return true;

            // create the login xml data
            string postData = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<request>\n" +
                "<user>\n" +
                "<email>" + _UserName + "</email>\n" +
                "<password>" + _Password + "</password>\n" +
                "</user>\n" +
                "<api-key>" + _ApiKey + "</api-key>\n" +
                "</request>";

            // perform the POST to SlimTimer to login the user
            string Result = GetHttpPage(new Uri(BASE_URL + "/users/token"), RequestType.Post, ContentType.Xml, postData);

            // now parse the results
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(Result);
            XmlNode node = xml.SelectSingleNode("/response/access-token");
            if (node != null)
            {
                _AccessToken = node.InnerText;
                _UserID = xml.SelectSingleNode("/response/user-id").InnerText;
                return true;
            }
            else
            {
                _Error = xml.SelectSingleNode("/errors/error").Value;
                return false;
            }
        }

        #endregion

        #region Tasks

        /// <summary>
        /// Get's all tasks from SlimTimer that match the passed in parameters
        /// </summary>
        /// <param name="completedTasks">Should we grab completed tasks? Yes/No/Only</param>
        /// <param name="filter">Who should we show tasks for? Owner/Reporter/CoWorker (or any combination)</param>
        /// <returns>List of Task objects that matched the passed in parameters</returns>
        public Collection<Task> ListTasks(ShowCompletedTask completedTasks, TaskFilters filter)
        {
            if (!PrepareRequestCall())
                return null;

            string url = GetBaseURL() + "/tasks" + GetURLParams() +
                "&filter[show_completed]=" + completedTasks.ToString().ToLower() + "&filter[role]=";

            if ((filter & TaskFilters.Coworker) > 0)
            {
                url += TaskFilters.Coworker.ToString().ToLower() + ",";
            }
            if ((filter & TaskFilters.Owner) > 0)
            {
                url += TaskFilters.Owner.ToString().ToLower() + ",";
            }
            if ((filter & TaskFilters.Reporter) > 0)
            {
                url += TaskFilters.Reporter.ToString().ToLower();
            }
            Uri uri = new Uri(url.TrimEnd(','));
            string Response = GetHttpPage(uri);

            return ParseTasks(Response);

        }

        /// <summary>
        /// Returns a specific task based on the task ID
        /// </summary>
        /// <param name="taskId">ID of the task to retrieve</param>
        /// <returns>Task that matches the ID, or null if it isn't found</returns>
        public Task GetTask(string taskId)
        {
            // ensure all required arguments have data
            if (taskId == null || taskId.Length == 0)
            {
                throw new ArgumentNullException("taskId", "Task ID cannot be null or blank.");
            }

            if (!PrepareRequestCall())
                return null;

            string url = GetBaseURL() + "/tasks/" + taskId + GetURLParams();

            string Response = GetHttpPage(new Uri(url));

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(Response);
            //XmlNodeList nodes = xml.SelectNodes("/task");

            Collection<Task> result = ParseTasks(Response);

            if (result.Count > 0)
                return result[0];

            _Error = "Couldn't find Task on SlimTimer with ID: " + taskId;
            return null;

        }

        /// <summary>
        /// Updates or Inserts a task to SlimTimer
        /// If the task ID is null, an insert will be performed, otherwise an update
        /// </summary>
        /// <param name="task">Task to be updated/inserted</param>
        /// <returns>Task result from update/insert.  If an insert was performed, this will contain the new ID.
        /// A null result means an error was encountered</returns>
        public Task UpdateTask(Task task)
        {
            // verify the required fields have data so we don't get an error back from SlimTimer
            if (task == null || task.Name == null || task.Name.Length == 0)
            {
                throw new ArgumentNullException("Task.Name", "Task name must have a value");
            }

            if (!PrepareRequestCall())
                return null;

            // build the URL
            string url = GetBaseURL() + "/tasks";
            RequestType request = RequestType.Post;

            // if the task has an Id we are updating
            if (task.Id != null && task.Id.Length > 0)
            {
                url += "/" + task.Id;
                request = RequestType.Put;
            }
            url += GetURLParams();

            // create the XML data to pass to SlimTimer
            XmlDocument data = new XmlDocument();
            data.CreateXmlDeclaration("1.0", "UTF-8", "");
            XmlNode root = data.CreateNode(XmlNodeType.Element, "task", "");
            data.AppendChild(root);
            XmlNode n = data.CreateNode(XmlNodeType.Element, "name", "");
            n.InnerText = task.Name;
            root.AppendChild(n);

            if (task.Tags.Count > 0)
            {
                n = data.CreateNode(XmlNodeType.Element, "tags", "");
                foreach (string s in task.Tags)
                {
                    n.InnerText += s + ",";
                }
                n.InnerText = n.InnerText.TrimEnd(',');
                root.AppendChild(n);
            }
            if (task.ReporterEmails.Count > 0)
            {
                n = data.CreateNode(XmlNodeType.Element, "reporter_emails", "");
                foreach (string s in task.ReporterEmails)
                {
                    n.InnerText += s + ",";
                }
                n.InnerText = n.InnerText.TrimEnd(',');
                root.AppendChild(n);
            }
            if (task.CoworkerEmails.Count > 0)
            {
                n = data.CreateNode(XmlNodeType.Element, "coworker_emails", "");
                foreach (string s in task.CoworkerEmails)
                {
                    n.InnerText += s + ",";
                }
                n.InnerText = n.InnerText.TrimEnd(',');
                root.AppendChild(n);
            }
            if (task.CompletedOn != null)
            {
                n = data.CreateNode(XmlNodeType.Element, "completed_on", "");
                n.InnerText += Convert.ToDateTime(task.CompletedOn).ToString("s", CultureInfo.InvariantCulture);
                root.AppendChild(n);
            }

            string Response = GetHttpPage(new Uri(url), request, ContentType.Xml, data.InnerXml);
            Collection<Task> tasks = ParseTasks(Response);
            if (tasks.Count > 0)
                return tasks[0];

            _Error = "Unable to update/insert task";
            return null;
        }

        /// <summary>
        /// Deletes a task from SlimTimer
        /// </summary>
        /// <param name="taskId">ID of task to delete</param>
        /// <returns>True if delete was successful, else false</returns>
        public bool DeleteTask(string taskId)
        {
            // ensure all required arguments have data
            if (taskId == null || taskId.Length == 0)
            {
                throw new ArgumentNullException("taskId", "Task ID cannot be null or blank.");
            }

            if (!PrepareRequestCall())
                return false;

            string url = GetBaseURL() + "/tasks/" + taskId;
            url += GetURLParams();

            GetHttpPage(new Uri(url), RequestType.Delete, ContentType.Xml, null);
            return true;
        }

        /// <summary>
        /// Parses an xml string and returns a List of Tasks
        /// </summary>
        /// <param name="xmlResponse">XML data to parse tasks</param>
        /// <returns>List of Tasks parsed from input XML string</returns>
        private static Collection<Task> ParseTasks(string xmlResponse)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlResponse);
            XmlNodeList nodes = xml.SelectNodes("/tasks/task");

            if (nodes == null || nodes.Count == 0)
            {
                nodes = xml.SelectNodes("/task");
            }

            Collection<Task> result = new Collection<Task>();
            foreach (XmlNode n in nodes)
            {
                result.Add(ParseTask(n));
            }
            return result;
        }

        /// <summary>
        /// Parses an xml node to get a single Task
        /// </summary>
        /// <param name="n">XmlNode to parse</param>
        /// <returns>Task parsed from XML data in XmlNode</returns>
        private static Task ParseTask(XmlNode n)
        {
            Task t = new Task(GetNodeText(n, "name"));
            t.Id = GetNodeText(n, "id");
            //t.Hours = float.Parse(GetNodeText(n, "hours"));
            t.Hours = float.Parse(GetNodeText(n, "hours"), CultureInfo.InvariantCulture);
            t.Role = GetNodeText(n, "role");
            if (GetNodeText(n, "tags").Length > 0)
                t.Tags.AddRange(GetNodeText(n, "tags").Split(','));
            t.UpdatedTime = GetNodeText(n, "updated-at").Length > 0 ? DateTime.Parse(GetNodeText(n, "updated-at"), CultureInfo.InvariantCulture) : DateTime.MinValue;
            t.CreatedTime = GetNodeText(n, "created-at").Length > 0 ? DateTime.Parse(GetNodeText(n, "created-at"), CultureInfo.InvariantCulture) : DateTime.MinValue;
            t.CompletedOn = GetNodeText(n, "completed-on").Length > 0 ? DateTime.Parse(GetNodeText(n, "completed-on"), CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture) : null;

            return t;
        }

        #endregion

        #region TimeEntries

        /// <summary>
        /// Retrieves all Time Entries from SlimTimer that fall between the passed in date range
        /// </summary>
        /// <param name="rangeStart">First date to search for Time Entries</param>
        /// <param name="rangeEnd">Last date to search for Time Entries</param>
        /// <returns>List of TimeEntry objects retrieved from SlimTimer</returns>
        public Collection<TimeEntry> ListTimeEntries(DateTime rangeStart, DateTime rangeEnd)
        {
            if (!PrepareRequestCall())
                return null;

            // Build the URL string
            string url = GetBaseURL() + "/time_entries" + GetURLParams() +
                "&filter[range_start]=" + rangeStart.ToString("s", CultureInfo.InvariantCulture) + "&filter[range_end]=" + rangeEnd.ToString("s", CultureInfo.InvariantCulture);

            string Response = GetHttpPage(new Uri(url));

            return ParseTimeEntries(Response);
        }

        /// <summary>
        /// Retrieves the Time Entry for the passed in ID
        /// </summary>
        /// <param name="entryId">ID of Time Entry to retrieve</param>
        /// <returns>TimeEntry object or null if the TimeEntry could not be found</returns>
        public TimeEntry GetTimeEntry(string entryId)
        {
            // ensure all required arguments have data
            if (entryId == null || entryId.Length == 0)
            {
                throw new ArgumentNullException("entryId", "Entry ID cannot be null or blank.");
            }

            if (!PrepareRequestCall())
                return null;

            // Build the URL string
            string url = GetBaseURL() + "/time_entries/" + entryId + GetURLParams();

            string Response = GetHttpPage(new Uri(url));

            Collection<TimeEntry> result = ParseTimeEntries(Response);

            if (result.Count > 0)
                return result[0];

            _Error = "Could not retrieve Time Entry with ID: " + entryId;
            return null;
        }

        /// <summary>
        /// Updates or Inserts a Time Entry to SlimTimer
        /// The method determines if an insert is needed by looking at the ID field.
        /// If the ID is null, an insert will be performed, otherwise it will do an update.
        /// </summary>
        /// <param name="entry">TimeEntry to update/insert</param>
        /// <returns>TimeEntry returned from SlimTimer, this will return the new ID for inserted entries</returns>
        public TimeEntry UpdateTimeEntry(TimeEntry entry)
        {
            // verify the required fields have data so we don't get an error back from SlimTimer
            TimeSpan ts = entry.EndTime.Subtract(entry.StartTime);
            if (ts.TotalSeconds < entry.Duration)
            {
                throw new ArgumentOutOfRangeException("Entry.Duration", entry.Duration, "Duration cannot be longer than difference between the StartTime and EndTime");
            }
            if (entry.Duration <= 0)
            {
                throw new ArgumentNullException("TimeEntry.Duration", "Duration must be greater than 0");
            }
            if (entry.RelatedTask == null || entry.RelatedTask.Id == null || entry.RelatedTask.Id.Length == 0)
            {
                throw new ArgumentNullException("TimeEntry.RelatedTask", "The related task is either null or doesn't have a SlimTimer ID");
            }

            if (!PrepareRequestCall())
                return null;

            string url = GetBaseURL() + "/time_entries";
            RequestType request = RequestType.Post;

            // if the task has an Id we are updating
            if (entry.Id != null && entry.Id.Length > 0)
            {
                url += "/" + entry.Id;
                request = RequestType.Put;
            }
            url += GetURLParams();

            // Create the XML data to send to SlimTimer
            XmlDocument data = new XmlDocument();
            data.CreateXmlDeclaration("1.0", "UTF-8", "");
            XmlNode root = data.CreateNode(XmlNodeType.Element, "time-entry", "");
            data.AppendChild(root);
            XmlNode n = data.CreateNode(XmlNodeType.Element, "start-time", "");
            n.InnerText = entry.StartTime.ToString("s", CultureInfo.InvariantCulture);
            root.AppendChild(n);
            n = data.CreateNode(XmlNodeType.Element, "end-time", "");
            n.InnerText = entry.EndTime.ToString("s", CultureInfo.InvariantCulture);
            root.AppendChild(n);
            n = data.CreateNode(XmlNodeType.Element, "duration-in-seconds", "");
            n.InnerText = entry.Duration.ToString(CultureInfo.InvariantCulture);
            root.AppendChild(n);
            n = data.CreateNode(XmlNodeType.Element, "task-id", "");
            n.InnerText = entry.RelatedTask.Id;
            root.AppendChild(n);

            if (entry.Tags.Count > 0)
            {
                n = data.CreateNode(XmlNodeType.Element, "tags", "");
                foreach (string s in entry.Tags)
                {
                    n.InnerText += s + ",";
                }
                n.InnerText = n.InnerText.TrimEnd(',');
                root.AppendChild(n);
            }
            n = data.CreateNode(XmlNodeType.Element, "comments", "");
            n.InnerText = entry.Comments;
            root.AppendChild(n);
            n = data.CreateNode(XmlNodeType.Element, "in-progress", "");
            n.InnerText = entry.InProgress.ToString(CultureInfo.InvariantCulture);
            root.AppendChild(n);

            string Response = GetHttpPage(new Uri(url), request, ContentType.Xml, data.InnerXml);
            Collection<TimeEntry> result = ParseTimeEntries(Response);
            if (result.Count > 0)
                return result[0];

            _Error = "Unable to update/insert TimeEntry";
            return null;
        }

        /// <summary>
        /// Deletes a TimeEntry from SlimTimer
        /// </summary>
        /// <param name="entryId">ID of entry to delete</param>
        /// <returns>true if entry successfully deleted, else false</returns>
        public bool DeleteTimeEntry(string entryId)
        {
            // ensure all required arguments have data
            if (entryId == null || entryId.Length == 0)
            {
                throw new ArgumentNullException("entryId", "Entry ID cannot be null or blank.");
            }

            if (!PrepareRequestCall())
                return false;

            string url = GetBaseURL() + "/time_entries/" + entryId;
            url += GetURLParams();

            GetHttpPage(new Uri(url), RequestType.Delete, ContentType.Xml, null);
            return true;
        }

        /// <summary>
        /// Parses an XML string to retrieve the TimeEntry data
        /// </summary>
        /// <param name="xmlResponse">XML string that contains the TimeEntry</param>
        /// <returns>List of TimeEntries parsed from input XML</returns>
        private static Collection<TimeEntry> ParseTimeEntries(string xmlResponse)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlResponse);
            XmlNodeList nodes = xml.SelectNodes("/time-entries/time-entry");

            if (nodes == null || nodes.Count == 0)
            {
                nodes = xml.SelectNodes("/time-entry");
            }

            Collection<TimeEntry> result = new Collection<TimeEntry>();
            foreach (XmlNode n in nodes)
            {
                result.Add(ParseTimeEntry(n));
            }
            return result;
        }

        /// <summary>
        /// Parses a single TimeEntry from an XmlNode
        /// </summary>
        /// <param name="n">XmlNode to parse TimeEntry from</param>
        /// <returns>TimeEntry parsed from input XmlNode</returns>
        private static TimeEntry ParseTimeEntry(XmlNode n)
        {
            XmlNode taskNode = n.SelectSingleNode("task");
            TimeEntry te = new TimeEntry(ParseTask(taskNode));

            te.Id = GetNodeText(n, "id");
            te.Duration = float.Parse(GetNodeText(n, "duration-in-seconds"), CultureInfo.InvariantCulture);
            te.Comments = GetNodeText(n, "comments");
            if (GetNodeText(n, "tags").Length > 0)
                te.Tags.AddRange(GetNodeText(n, "tags").Split(','));
            te.UpdatedTime = GetNodeText(n, "updated-at").Length > 0 ? DateTime.Parse(GetNodeText(n, "updated-at"), CultureInfo.InvariantCulture) : DateTime.MinValue;
            te.CreatedTime = GetNodeText(n, "created-at").Length > 0 ? DateTime.Parse(GetNodeText(n, "created-at"), CultureInfo.InvariantCulture) : DateTime.MinValue;
            te.EndTime = GetNodeText(n, "end-time").Length > 0 ? DateTime.Parse(GetNodeText(n, "end-time"), CultureInfo.InvariantCulture) : DateTime.MinValue;
            te.StartTime = GetNodeText(n, "start-time").Length > 0 ? DateTime.Parse(GetNodeText(n, "start-time"), CultureInfo.InvariantCulture) : DateTime.MinValue;
            te.InProgress = bool.Parse(GetNodeText(n, "in-progress"));

            return te;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Parses the xml of an error response
        /// </summary>
        /// <param name="xmlResponse">XML error returned from slimtimer</param>
        /// <returns>text description of the error</returns>
        private string ParseError(string xmlResponse)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlResponse);
            return xml.InnerText;
        }

        /// <summary>
        /// Performs all necessary actions to prepare for a HTTP request.
        /// </summary>
        /// <returns>True if preperation was successful, else false</returns>
        private bool PrepareRequestCall()
        {
            // reset the error message
            _Error = String.Empty;

            // try to login before any requests
            // if we are already logged in the login function will just return true
            return Logon();
        }

        /// <summary>
        /// Builds the Base URL for API calls
        /// </summary>
        /// <returns>Base URL for API calls</returns>
        private string GetBaseURL()
        {
            return BASE_URL + "/users/" + _UserID;
        }

        /// <summary>
        /// Builds the standard params for all API calls
        /// </summary>
        /// <returns>URL Params necessary for all calls</returns>
        private string GetURLParams()
        {
            return "?api_key=" + _ApiKey + "&access_token=" + _AccessToken;
        }

        /// <summary>
        /// Returns the text out of an XML node
        /// </summary>
        /// <param name="node">XmlNode to retrieve text from</param>
        /// <param name="xPath">XPath query</param>
        /// <returns>Text from XmlNode</returns>
        private static string GetNodeText(XmlNode node, string xPath)
        {
            XmlNode n = node.SelectSingleNode(xPath);
            if (n == null)
                return "";

            return n.InnerText;
        }

        #endregion
    }


    #region Task & TimeEntry Classes

    /// <summary>
    /// Class to hold Task data.  The only required field is Name.
    /// </summary>
    public class Task
    {
        #region Declarations

        string _Id;
        string _Name;
        DateTime _UpdatedTime;
        // this is an object so we can set it to null if necessary
        object _CompletedOn;
        StringCollection _Tags = new StringCollection();
        StringCollection _ReporterEmails = new StringCollection();
        StringCollection _CoworkerEmails = new StringCollection();
        string _Role;
        float _Hours;
        DateTime _CreatedTime;

        #endregion

        #region Properties

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public DateTime UpdatedTime
        {
            get { return _UpdatedTime; }
            set { _UpdatedTime = value; }
        }
        public object CompletedOn
        {
            get { return _CompletedOn; }
            set { _CompletedOn = value; }
        }
        public StringCollection Tags
        {
            get { return _Tags; }
        }
        public StringCollection ReporterEmails
        {
            get { return _ReporterEmails; }
        }
        public StringCollection CoworkerEmails
        {
            get { return _CoworkerEmails; }
        }
        public string Role
        {
            get { return _Role; }
            set { _Role = value; }
        }
        public float Hours
        {
            get { return _Hours; }
            set { _Hours = value; }
        }
        public DateTime CreatedTime
        {
            get { return _CreatedTime; }
            set { _CreatedTime = value; }
        }

        #endregion

        public Task(string name)
        {
            _Name = name;
            _CreatedTime = DateTime.Now;
        }
    }

    /// <summary>
    /// Class to hold TimeEntry data.  The required fields are StartTime, EndTime, Duration, and RelatedTask.
    /// </summary>
    public class TimeEntry
    {
        #region Declarations

        string _Id;
        DateTime _StartTime;
        DateTime _EndTime;
        float _Duration;
        StringCollection _Tags = new StringCollection();
        string _Comments;
        bool _InProgress;
        Task _RelatedTask;
        DateTime _UpdatedTime;
        DateTime _CreatedTime;

        #endregion

        #region Properties

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public DateTime StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        public DateTime EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
        public float Duration
        {
            get { return _Duration; }
            set { _Duration = value; }
        }
        public StringCollection Tags
        {
            get { return _Tags; }
        }
        public string Comments
        {
            get { return _Comments; }
            set { _Comments = value; }
        }
        public bool InProgress
        {
            get { return _InProgress; }
            set { _InProgress = value; }
        }
        public Task RelatedTask
        {
            get { return _RelatedTask; }
            set { _RelatedTask = value; }
        }
        public DateTime UpdatedTime
        {
            get { return _UpdatedTime; }
            set { _UpdatedTime = value; }
        }
        public DateTime CreatedTime
        {
            get { return _CreatedTime; }
            set { _CreatedTime = value; }
        }

        #endregion

        public TimeEntry(Task relatedTask) 
        {
            // give the required fields default values
            StartTime = DateTime.Now;
            EndTime = DateTime.Now.AddSeconds(1);
            Duration = 1;
            RelatedTask = relatedTask;
            CreatedTime = DateTime.Now;
        }
        public TimeEntry(DateTime startTime, DateTime endTime, int duration, Task relatedTask)
        {
            StartTime = startTime;
            EndTime = endTime;
            Duration = duration;
            RelatedTask = relatedTask;
            CreatedTime = DateTime.Now;
        }
    }
    #endregion
}
