﻿namespace Crabwise.CommandWrap
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Security;

    /// <summary>
    /// Specifies a set of values that are used when you execute a command.
    /// </summary>
    /// <remarks>
    /// Properties of this class are passed to an instance of <see cref="System.Diagnostics.ProcessStartInfo"/> when 
    /// spawning the process for this command.
    /// </remarks>
    public sealed class CommandStartInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandStartInfo"/> class with empty properties.
        /// </summary>
        public CommandStartInfo()
        {
            this.EnvironmentVariables = new StringDictionary();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandStartInfo"/> class using a path.
        /// </summary>
        /// <param name="path">The path to use for executing a command.</param>
        public CommandStartInfo(string path)
            : this()
        {
            this.Path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandStartInfo"/> class using a 
        /// <see cref="ProcessStartInfo"/> object to initialize its properties.
        /// </summary>
        /// <param name="processStartInfo">The <see cref="ProcessStartInfo"/> object to use.</param>
        public CommandStartInfo(ProcessStartInfo processStartInfo)
            : this()
        {
            this.CreateNoWindow = processStartInfo.CreateNoWindow;
            this.Domain = processStartInfo.Domain;
            this.LoadUserProfile = processStartInfo.LoadUserProfile;
            this.Password = processStartInfo.Password;
            this.RedirectStandardInput = processStartInfo.RedirectStandardInput;
            this.UserName = processStartInfo.UserName;
            this.WindowStyle = processStartInfo.WindowStyle;
            this.WorkingDirectory = processStartInfo.WorkingDirectory;

            foreach (var environmentVariable in processStartInfo.EnvironmentVariables)
            {
                var dictEntry = (DictionaryEntry)environmentVariable;
                this.EnvironmentVariables.Add(dictEntry.Key.ToString(), dictEntry.Value.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CreateNoWindow property is set when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.CreateNoWindow"/>
        public bool CreateNoWindow { get; set; }

        /// <summary>
        /// Gets or sets the Domain property to use when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.Domain"/>
        public string Domain { get; set; }

        /// <summary>
        /// Gets the EnvironmentVariables property to use when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.EnvironmentVariables"/>
        public StringDictionary EnvironmentVariables { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the LoadUserProfile property is set when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.LoadUserProfile"/>
        public bool LoadUserProfile { get; set; }

        /// <summary>
        /// Gets or sets the Password property to use when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.Password"/>
        public SecureString Password { get; set; }

        /// <summary>
        /// Gets or sets the path to use for executing a command.
        /// </summary>
        /// <remarks>
        /// This property overrides the DefaultPath attribute on any commands. It can be used to set the path of the 
        /// command if it isn't known at compile time.
        /// </remarks>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the RedirectStandardInput property is set when executing the 
        /// command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.RedirectStandardInput"/>
        public bool RedirectStandardInput { get; set; }

        /// <summary>
        /// Gets or sets the UserName property to use when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.UserName"/>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the WindowStyle property to use when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.WindowStyle"/>
        public ProcessWindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Gets or sets the working directory in which the command will execute.
        /// </summary>
        /// <remarks>
        /// This property overrides the DefaultWorkingDirectory attribute on any commands. It can be used to set the
        /// working directory of the command if it isn't known at compile time.
        /// </remarks>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Returns a <see cref="System.Diagnostics.ProcessStartInfo"/> object which has matching properties of this
        /// <see cref="CommandStartInfo"/> object.
        /// </summary>
        /// <param name="fileName">The file name to use when creating the <see cref="ProcessStartInfo"/> object.</param>
        /// <returns><see cref="System.Diagnostics.ProcessStartInfo"/> object with matching properties.</returns>
        /// <remarks>
        /// Specifically, the returned <see cref="System.Diagnostics.ProcessStartInfo"/> object that is returned has 
        /// the same properties except for <see cref="CommandStartInfo.Path"/> and 
        /// <see cref="CommandStartInfo.WorkingDirectory"/>.
        /// </remarks>
        internal ProcessStartInfo GetProcessStartInfo(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("The given file name was empty.", "fileName");
            }

            var workingDirectory = this.WorkingDirectory;
            var fullFileName = fileName;
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = Environment.ExpandEnvironmentVariables(workingDirectory);
            }

            if (!string.IsNullOrEmpty(this.Path))
            {
                fullFileName = System.IO.Path.Combine(this.Path, fileName);
            }

            fullFileName = Environment.ExpandEnvironmentVariables(fullFileName);

            var processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = this.CreateNoWindow,
                Domain = this.Domain,
                FileName = fullFileName,
                LoadUserProfile = this.LoadUserProfile,
                Password = this.Password,
                RedirectStandardInput = this.RedirectStandardInput,
                UserName = this.UserName,
                WindowStyle = this.WindowStyle,
                WorkingDirectory = workingDirectory
            };

            foreach (var environmentVariable in this.EnvironmentVariables)
            {
                var dictEntry = (DictionaryEntry)environmentVariable;
                processStartInfo.EnvironmentVariables.Add(dictEntry.Key.ToString(), dictEntry.Value.ToString());
            }

            return processStartInfo;
        }
    }
}