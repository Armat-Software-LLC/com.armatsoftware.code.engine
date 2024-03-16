using System;
using System.IO;
using System.Security;
using System.Text;
using ArmatSoftware.Code.Engine.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace ArmatSoftware.Code.Engine.Logger.File
{
    /// <summary>
    /// Simple file logger for code engine
    /// </summary>
    // public class CodeEngineFileLogger : ILogger
    // {
    //     public const string LogFilePathKey = "ASCE_LOG_FILE_PATH";
    //     
    //     private TextWriter _logFileWriter;
    //     
    //     /// <summary>
    //     /// Read the necessary configuration from the supplied parameter and initialize logger
    //     /// with the file path set in ARCE_LOG_FILE_PATH
    //     /// </summary>
    //     /// <param name="configuration"></param>
    //     /// <exception cref="ApplicationException"></exception>
    //     public CodeEngineFileLogger(IConfiguration configuration)
    //     {
    //         Console.WriteLine($"config is null: {null == configuration}");
    //         // fail on invalid configuration
    //         if (null == configuration)
    //         {
    //             throw new ArgumentNullException(nameof(configuration));
    //         }
    //
    //         // fail on missing or empty config value
    //         var logFilePathSection = configuration.GetSection(LogFilePathKey);
    //         if (!logFilePathSection.Exists())
    //         {
    //             throw new ApplicationException($"Configuration section {LogFilePathKey} not found or empty");
    //         }
    //     
    //         Initialize(logFilePathSection.Value);
    //     }
    //
    //     /// <summary>
    //     /// Initialize log file at the supplied path
    //     /// </summary>
    //     /// <param name="logFilePath">Target log file path</param>
    //     public CodeEngineFileLogger(string logFilePath)
    //     {
    //         Initialize(logFilePath);
    //     }
    //
    //     /// <summary>
    //     /// Initialize logger with a ready to write to stream
    //     /// </summary>
    //     /// <param name="writerStream">Open and ready stream</param>
    //     public CodeEngineFileLogger(FileStream writerStream)
    //     {
    //         Initialize(writerStream);
    //     }
    //
    //     public CodeEngineFileLogger(TextWriter logWriter)
    //     {
    //         Initialize(logWriter);
    //     }
    //
    //     /// <summary>
    //     /// Validate the supplied file path and initialize text writer
    //     /// </summary>
    //     /// <param name="logFilePath"></param>
    //     public void Initialize(string logFilePath)
    //     {
    //         try
    //         {
    //             var writerStream = new FileStream(logFilePath, FileMode.OpenOrCreate, FileAccess.Write,
    //                 FileShare.ReadWrite);
    //             Initialize(writerStream);
    //         }
    //         catch (ArgumentNullException e)
    //         {
    //             Console.WriteLine($"Log file path is null or empty: {logFilePath}", e);
    //             throw;
    //         }
    //         catch (FileNotFoundException e)
    //         {
    //             Console.WriteLine($"Log file not found: {logFilePath}", e);
    //             throw;
    //         }
    //         catch (NotSupportedException e)
    //         {
    //             Console.WriteLine($"Log file type not supported: {logFilePath}", e);
    //             throw;
    //         }
    //         catch (SecurityException e)
    //         {
    //             Console.WriteLine($"Insufficient access to the log file: {logFilePath}", e);
    //             throw;
    //         }
    //         catch (DirectoryNotFoundException e)
    //         {
    //             Console.WriteLine($"Log file path invalid: {logFilePath}", e);
    //             throw;
    //         }
    //         catch (UnauthorizedAccessException e)
    //         {
    //             Console.WriteLine($"Log file access unauthorized: {logFilePath}", e);
    //             throw;
    //         }
    //         catch (IOException e)
    //         {
    //             Console.WriteLine($"Error creating or accessing log file: {logFilePath}", e);
    //             throw;
    //         }
    //         catch (Exception e)
    //         {
    //             Console.WriteLine($"Error initializing stream for {logFilePath}", e);
    //             throw;
    //         }
    //     }
    //     
    //     public void Initialize(FileStream writerStream)
    //     {
    //         if (!writerStream.CanWrite)
    //         {
    //             throw new ApplicationException("Log file stream is not writable");
    //         }
    //
    //         try
    //         {
    //             var streamWriter = new StreamWriter(writerStream, Encoding.UTF8);
    //             Initialize(streamWriter);
    //         }
    //         catch (ArgumentNullException e)
    //         {
    //             Console.WriteLine($"{nameof(writerStream)} stream or encoding is null");
    //             Console.WriteLine(e.StackTrace);
    //             throw;
    //         }
    //         catch (ArgumentException e)
    //         {
    //             Console.WriteLine($"Cannot write to stream {nameof(writerStream)}");
    //             Console.WriteLine(e.StackTrace);
    //             throw;
    //         }
    //         catch (Exception e)
    //         {
    //             Console.WriteLine($"Error initializing stream {nameof(writerStream)}");
    //             Console.WriteLine(e.StackTrace);
    //             throw;
    //         }
    //     }
    //
    //     public void Initialize(TextWriter logWriter)
    //     {
    //         _logFileWriter = logWriter ?? throw new ArgumentNullException(nameof(logWriter));
    //     }
    //
    //     public void Info(string message)
    //     {
    //         WriteToFile($"INFO - {message}");
    //     }
    //
    //     public void Warning(string message)
    //     {
    //         WriteToFile($"WARN - {message}");
    //     }
    //
    //     public void Error(string message, Exception ex = null)
    //     {
    //         WriteToFile($"ERROR - {message}");
    //         WriteToFile(ex?.StackTrace);
    //     }
    //
    //     private void WriteToFile(string message)
    //     {
    //         try
    //         {
    //             _logFileWriter.WriteLine($"{Strings.FormatDateTime(DateTime.UtcNow, DateFormat.LongDate & DateFormat.ShortTime)} - {message}");
    //             _logFileWriter.Flush();
    //         }
    //         catch (ObjectDisposedException e)
    //         {
    //             Console.WriteLine($"The log writer is closed or invalid - {e.Message}");
    //             Console.WriteLine(e.StackTrace);
    //             throw;
    //         }
    //         catch (IOException e)
    //         {
    //             Console.WriteLine($"Error writing to the log stream - {e.Message}");
    //             Console.WriteLine(e.StackTrace);
    //             throw;
    //         }
    //     }
    // }
}