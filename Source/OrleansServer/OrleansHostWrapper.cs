namespace Reaper.SharpBattleNet.BattleNetServer
{
    using System;
    using System.Net;

    using Orleans;
    using Orleans.Host;
    using Orleans.Host.SiloHost;

    internal class OrleansHostWrapper : IDisposable
    {
        private OrleansSiloHost _siloHost;

        public OrleansHostWrapper(string[] args)
        {
            ParseArguments(args);
            Setup();

            return;
        }

        public bool Debug
        {
            get 
            {
                return _siloHost != null && _siloHost.Debug; 
            }

            set
            {
                _siloHost.Debug = value;
                return;
            }
        }

        private void Setup()
        {
            _siloHost.LoadOrleansConfig();

            return;
        }

        public bool Run()
        {
            bool ok = false;
            string message = "";

            try
            {
                _siloHost.InitializeOrleansSilo();

                ok = _siloHost.StartOrleansSilo();

                if(true == ok)
                {
                    Console.WriteLine(string.Format("Successfully started Orleans silo '{0}' as a {1} node.", _siloHost.SiloName, _siloHost.SiloType));
                }
                else
                {
                    throw new SystemException(string.Format("Failed to start Orleans silo '{0}' as a {1} node.", _siloHost.SiloName, _siloHost.SiloType));
                }
            }
            catch(Exception exc)
            {
                _siloHost.ReportStartupError(exc);
                message = string.Format("{0}:\n{1}\n{2}", exc.GetType().FullName, exc.Message, exc.StackTrace);
                Console.WriteLine(message);
            }

            return ok;
        }

        public bool Stop()
        {
            bool ok = false;
            string message = "";

            try
            {
                _siloHost.StopOrleansSilo();

                Console.WriteLine(string.Format("Orleans silo '{0}' shutdown.", _siloHost.SiloName));
            }
            catch(Exception exc)
            {
                _siloHost.ReportStartupError(exc);
                message = string.Format("{0}:\n{1}\n{2}", exc.GetType().FullName, exc.Message, exc.StackTrace);
                Console.WriteLine(message);
            }

            return ok;
        }

        private bool ParseArguments(string[] args)
        {
            string deploymentId = null;
            string configFileName = "OrleansConfigurationServer.xml";
            string siloName = Dns.GetHostName(); // Default to machine name
            int argPos = 1;
            string argument = "";
            string[] split = null;

            for(int count = 0; count < args.Length; count++)
            {
                argument = args[count];
                if(argument.StartsWith("-") || argument.StartsWith("/"))
                {
                    switch(argument.ToLowerInvariant())
                    {
                        case "/?":
                        case "/help":
                        case "-?":
                        case "-help":
                            return false;

                        default:
                            Console.WriteLine("Bad command line arguments supplied: " + argument);
                            return false;
                    }
                }
                else if(argument.Contains("="))
                {
                    split = argument.Split('=');
                    if(String.IsNullOrEmpty(split[1]))
                    {
                        Console.WriteLine("Bad command line arguments supplied: " + argument);
                        return false;
                    }

                    switch(split[0].ToLowerInvariant())
                    {
                        case "deploymentid":
                            deploymentId = split[1];
                            break;

                        default:
                            Console.WriteLine("Bad command line arguments supplied: " + argument);
                            return false;
                    }
                }
                else if(argPos == 1)
                {
                    siloName = argument;
                    argPos++;
                }
                else if(argPos == 2)
                {
                    configFileName = argument;
                    argPos++;
                }
                else
                {
                    Console.WriteLine("Too many command line arguments supplied: " + argument);
                    return false;
                }
            }

            _siloHost = new OrleansSiloHost(siloName);
            _siloHost.ConfigFileName = configFileName;
            if(null != deploymentId)
            {
                _siloHost.DeploymentId = deploymentId;
            }

            return true;
        }

        private void PrintUsage()
        {
            Console.WriteLine(
                @"USAGE: 
                    OrleansHost.exe [<siloName> [<configFile>]] [DeploymentId=<idString>] [/debug]
                Where:
                    <siloName>              - Name of this silo in the Config file list (optional)
                    <configFile>            - Path to the Config file to use (optional)
                    DeploymentId=<idString> - Which deployment group this host instance should run in (optional)
                    /debug                  - Turn on extra debug output during host startup (optional)");

            return;
        }

        public void Dispose()
        {
            Dispose(true);

            return;
        }

        protected virtual void Dispose(bool dispose)
        {
            _siloHost.Dispose();
            _siloHost = null;

            return;
        }
    }
}
