// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|
// |**| Source Code for the FPS Operation Mango     |**|
// |**| Opcon|OperationMango|**|Opcon|OperationMango|**|

using System.IO;
using System.Collections.Generic;

namespace Revolution.Core
{
    public delegate void Handler(RenamedEventArgs e);

	public class FileWatcher
    {
        #region Public Members

        /// <summary>
        /// The path to the folder being watched
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// The filter to be applied
        /// Use * for wildcards, *.* for everything
        /// </summary>
        public string Filter
        {
            get;
            private set;
        }

        /// <summary>
        /// The events to be called
        /// </summary>
        public List<Handler> Delegates;
        #endregion

        #region Private Members

        /// <summary>
        /// The FileSystemWatcher object
        /// </summary>
		protected FileSystemWatcher m_FileSystemWatcher;

        #endregion

        #region Constructors
        //------------------------------------------------------------------------------------------------		
		
        /// <summary>
        /// Creates a new filewatcher at the specified folder with the specified filter
        /// </summary>
        /// <param name="p">The folder to watch</param>
        /// <param name="f">The specified filter</param>
        public FileWatcher (string p, string f)
		{
			Path = p;
			Filter = f;
			m_FileSystemWatcher = new FileSystemWatcher(Path, Filter);
			SetupFileChangeHandler ();
		}
		
        /// <summary>
        /// Creates a new filewatcher at the specified folder
        /// </summary>
        /// <param name="p">The folder to watch</param>
		public FileWatcher (string p)
		{
			Path = p;
			m_FileSystemWatcher = new FileSystemWatcher(Path);
			SetupFileChangeHandler ();
		}
		
        //public FileWatcher ()
        //{
        //    m_FileSystemWatcher = new FileSystemWatcher();
        //    SetupFileChangeHandler ();
        //}
        #endregion

        #region Private Methods
        //------------------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes the FileWatcher
		/// </summary>
        protected void SetupFileChangeHandler ()
		{
			m_FileSystemWatcher.Changed += delegate(object sender, FileSystemEventArgs e) 
			{
				CallFileChangeHandler(e);
			};
			m_FileSystemWatcher.EnableRaisingEvents = true;
            m_FileSystemWatcher.Renamed += new RenamedEventHandler(m_FileSystemWatcher_Renamed);
            Delegates = new List<Handler>();
		}

        /// <summary>
        /// Called when a file is renamed.
        /// Executes each handler in the list Delegates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Data for the Renamed event</param>
        void m_FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            foreach (Handler h in Delegates)
            {
                h(e);
            }
        }

        /// <summary>
        /// Called when something other than a rename event is handled.
        /// Executes each handler in the list Delegates
        /// </summary>
        /// <param name="e"></param>
        protected void CallFileChangeHandler(FileSystemEventArgs e)
        {
            foreach (Handler h in Delegates)
            {
                System.Console.WriteLine(System.IO.Path.GetDirectoryName(e.FullPath));
                RenamedEventArgs args = new RenamedEventArgs(e.ChangeType, System.IO.Path.GetDirectoryName(e.FullPath), e.Name, e.Name);
                h(args);
            }
        }
        #endregion

        #region Public Methods
        //------------------------------------------------------------------------------------------------
		
        /// <summary>
        /// Sets the FileWatcher filter to the specified string.
        /// Use * as a wildcard, *.* to watch everything
        /// </summary>
        /// <param name="f">The specified filter</param>
        public void SetFilter(string f)
		{
			Filter = f;
			m_FileSystemWatcher.Filter = Filter;
		}
		
        /// <summary>
        /// Sets the folder to watch
        /// </summary>
        /// <param name="p">The path to the folder to watch</param>
		public void SetDirectory(string p)
		{
			Path = p;
			m_FileSystemWatcher.Path = Path;
        }

        /// <summary>
        /// Adds a new file change handler to the list Delegates
        /// </summary>
        /// <param name="h">The Handler to add</param>
        public void AddFileChangeHandler(Handler h)
        {
            Delegates.Add(h);
        }
        #endregion
    }
}
