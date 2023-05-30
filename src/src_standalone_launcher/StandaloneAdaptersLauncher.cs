#region License
/*
* Copyright (c) Lightstreamer Srl
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#endregion License

using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Net.Sockets;

using Lightstreamer.DotNet.Server;
using Lightstreamer.Adapters.PortfolioDemo.Feed;

using NLog;


namespace Lightstreamer.Adapters.PortfolioDemo
{
    public class AdaptersLauncher
    {
        private static NLog.Logger _log = NLog.LogManager.GetLogger("Lightstreamer.Adapters.PortfolioDemo.AdaptersLauncher");

        public const string PREFIX1 = "-";
        public const string PREFIX2 = "/";

        public const char SEP = '=';

        public const string ARG_HELP_LONG = "help";
        public const string ARG_HELP_SHORT = "?";

        public const string ARG_HOST = "host";
        public const string ARG_METADATA_RR_PORT = "metadata_rrport";
        public const string ARG_DATA_RR_PORT = "data_rrport";
        public const string ARG_NAME = "name";

        public static void Main(string[] args)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "TestAdapter.log" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logfile);

            NLog.LogManager.Configuration = config;

            if (args.Length == 0) Help();

            _log.Info("Lightstreamer PortfolioDemo .NET Adapter Custom Server starting...");

            Server.SetLoggerProvider(new Log4NetLoggerProviderWrapper());

            IDictionary parameters = new Hashtable();
            string host = null;
            int rrPortMD = -1;
            int rrPortD = -1;
            string name = null;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg.StartsWith(PREFIX1) || arg.StartsWith(PREFIX2))
                {
                    arg = arg.Substring(1).ToLower();

                    if (arg.Equals(ARG_HELP_SHORT) || arg.Equals(ARG_HELP_LONG))
                    {
                        Help();
                    }
                    else if (arg.Equals(ARG_HOST))
                    {
                        i++;
                        host = args[i];

                        _log.Debug("Found argument: '" + ARG_HOST + "' with value: '" + host + "'");
                    }
                    else if (arg.Equals(ARG_METADATA_RR_PORT))
                    {
                        i++;
                        rrPortMD = Int32.Parse(args[i]);

                        _log.Debug("Found argument: '" + ARG_METADATA_RR_PORT + "' with value: '" + rrPortMD + "'");
                    }
                    else if (arg.Equals(ARG_DATA_RR_PORT))
                    {
                        i++;
                        rrPortD = Int32.Parse(args[i]);

                        _log.Debug("Found argument: '" + ARG_DATA_RR_PORT + "' with value: '" + rrPortD + "'");
                    }
                    else if (arg.Equals(ARG_NAME))
                    {
                        i++;
                        name = args[i];

                        _log.Debug("Found argument: '" + ARG_NAME + "' with value: '" + name + "'");
                    }

                }
                else
                {
                    int sep = arg.IndexOf(SEP);
                    if (sep < 1)
                    {
                        _log.Warn("Skipping unrecognizable argument: '" + arg + "'");

                    }
                    else
                    {
                        string par = arg.Substring(0, sep).Trim();
                        string val = arg.Substring(sep + 1).Trim();
                        parameters[par] = val;

                        _log.Debug("Found parameter: '" + par + "' with value: '" + val + "'");
                    }
                }
            }

            PortfolioFeedSimulator feed = new PortfolioFeedSimulator();
            // A reference to the feed simulator will be supplied
            // to both the Data and the Matedata Adapters.

            try
            {
                {
                    MetadataProviderServer server = new MetadataProviderServer();
                    Lightstreamer.Adapters.PortfolioDemo.Metadata.PortfolioMetadataAdapter adapter =
                        new Lightstreamer.Adapters.PortfolioDemo.Metadata.PortfolioMetadataAdapter();
                    // We complete the Metadata Adapter initialization by supplying
                    // a reference to the feed simulator through a custom method;
                    // for this reason, the Portfolio Demo Metadata Adapter
                    // does not support the basic DotNetServer.exe launcher
                    // provided by LS library,
                    adapter.SetFeed(feed);

                    server.Adapter = adapter;
                    server.AdapterParams = parameters;
                    // server.AdapterConfig not needed by PortfolioMetadataAdapter
                    if (name != null) server.Name = name;
                    _log.Debug("Remote Metadata Adapter initialized");

                    ServerStarter starter = new ServerStarter(host, rrPortMD);
                    starter.Launch(server);
                }

                {
                    DataProviderServer server = new DataProviderServer();
                    Lightstreamer.Adapters.PortfolioDemo.Data.PortfolioAdapter adapter =
                        new Lightstreamer.Adapters.PortfolioDemo.Data.PortfolioAdapter();
                    // We complete the Data Adapter initialization by supplying
                    // a reference to the feed simulator through a custom method;
                    // for this reason, the Portfolio Demo Data Adapter
                    // does not support the basic DotNetServer.exe launcher
                    // provided by LS library,
                    adapter.SetFeed(feed);

                    server.Adapter = adapter;
                    // server.AdapterParams not needed by PortfolioAdapter
                    // server.AdapterConfig not needed by PortfolioAdapter
                    if (name != null) server.Name = name;
                    _log.Debug("Remote Data Adapter initialized");

                    ServerStarter starter = new ServerStarter(host, rrPortD);
                    starter.Launch(server);
                }
            }
            catch (Exception e)
            {
                _log.Fatal("Exception caught while starting the server: " + e.Message + ", aborting...", e);
            }

            _log.Info("Lightstreamer PortfolioDemo .NET Adapter Custom Server running");
        }

        private static void Help()
        {
            _log.Fatal("Lightstreamer PortfolioDemo .NET Adapter Custom Server Help");
            _log.Fatal("Usage: DotNetPortfolioDemoLauncher");
            _log.Fatal("                     [/name <name>] /host <address>");
            _log.Fatal("                     /metadata_rrport <port> /data_rrport <port>");
            _log.Fatal("                     [\"<param1>=<value1>\" ... \"<paramN>=<valueN>\"]");
            _log.Fatal("Where: <name>        is the symbolic name for both the adapters (1)");
            _log.Fatal("       <address>     is the host name or ip address of LS server (2)");
            _log.Fatal("       <port>        is a tcp port number where LS proxy is listening on");
            _log.Fatal("       <paramN>      is the Nth metadata adapter parameter name (3)");
            _log.Fatal("       <valueN>      is the value of the Nth metadata adapter parameter (3)");
            _log.Fatal("Notes: (1) The adapter name is optional, if it is not given the adapter will be");
            _log.Fatal("           assigned a progressive number name like \"#1\", \"#2\" and so on");
            _log.Fatal("       (2) The communication will be from here to LS, not viceversa");
            _log.Fatal("       (3) The parameters name/value pairs will be passed to the PortfolioDemo");
            _log.Fatal("           Metadata Adapter (to be forwarded to the embedded LiteralBasedProvider)");
            _log.Fatal("           as a Hashtable in the \"parameters\" Init() argument");
            _log.Fatal("           The PortfolioDemo Data Adapter requires no parameters");
            _log.Fatal("Aborting...");

            System.Environment.Exit(9);
        }
    }

    public class ServerStarter : IExceptionHandler
    {
        private static NLog.Logger _log = NLog.LogManager.GetLogger("Lightstreamer.Adapters.PortfolioDemo.ServerStarter");

        private Server _server;
        private bool _closed;

        private string _host;
        private int _rrPort;

        public ServerStarter(string host, int rrPort)
        {
            _host = host;
            _rrPort = rrPort;
        }

        public void Launch(Server server)
        {
            _server = server;
            _closed = false;
            _server.ExceptionHandler = this;
            Thread t = new Thread(new ThreadStart(Run));
            t.Start();
        }

        public void Run()
        {
            TcpClient _rrSocket = null;

            do
            {
                _log.Info("Connecting...");

                try
                {
                    _log.Info("Opening connection on port " + _rrPort + "...");
                    _rrSocket = new TcpClient(_host, _rrPort);
                    _log.Info("Connected");

                    break;
                }
                catch (SocketException)
                {
                    _log.Warn("Connection failed, retrying in 10 seconds...");
                    Thread.Sleep(10000);
                }

            } while (true);

            Stream _rrStream = _rrSocket.GetStream();
            _server.RequestStream = _rrStream;
            _server.ReplyStream = _rrStream;

            _server.Start();
        }

        public bool handleIOException(Exception exception)
        {
            lock (this)
            {
                if (_closed)
                {
                    return false;
                }
                else
                {
                    _log.Error("Connection to Lightstreamer Server closed");
                    _closed = true;
                }
            }
            _server.Close();
            System.Environment.Exit(0);
            return false;
        }

        public bool handleException(Exception exception)
        {
            lock (this)
            {
                if (_closed)
                {
                    return false;
                }
                else
                {
                    _log.Error("Caught exception: " + exception.Message, exception);
                    _closed = true;
                }
            }
            _server.Close();
            System.Environment.Exit(1);
            return false;
        }

        // Notes about exception handling.
        // 
        // In case of exception, the whole Remote Server process instance
        // can no longer be used;
        // closing it also ensures that the Proxy Adapter closes
        // (thus causing Lightstreamer Server to close)
        // or recovers by accepting connections from a new Remote
        // Server process instance.
        // 
        // Keeping the process instance alive and replacing the Server
        // class instances would be possible.
        // This would issue new connections with Lightstreamer Server.
        // However, new instances of the Remote Adapters would also be needed
        // and a cleanup of the current instances should be performed,
        // by invoking them directly through a custom method.

    }

}