using Nini.Config;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Application.Details
{
    internal sealed class ApplicationConfiguration : IDisposable
    {
        private readonly IKernel _injectionKernel = null;
        private readonly string _applicationName = "";
        private readonly string _writeDirectory = "";

        private bool _disposed = false;

        private void Configure()
        {
            string configurationFile = _applicationName + ".ini";
            string configurationBasePath = "../Configuration/" + configurationFile;
            string configurationPath = Path.Combine(_writeDirectory, configurationFile);
            DateTime configurationBaseTime = default(DateTime);
            DateTime configurationTime = default(DateTime);

            if (false == File.Exists(configurationPath))
            {
                File.Copy(configurationBasePath, configurationPath);
            }
            else
            {
                configurationBaseTime = File.GetLastWriteTimeUtc(configurationBasePath);
                configurationTime = File.GetLastWriteTimeUtc(configurationPath);

                if (configurationBaseTime > configurationTime)
                {
                    File.Delete(configurationPath);
                    File.Copy(configurationBasePath, configurationPath);
                }
            }

            _injectionKernel.Bind<IConfigSource>().ToConstant(new IniConfigSource(configurationPath)).InSingletonScope();

            return;
        }

        public ApplicationConfiguration(IKernel injectionKernel, string applicationName, string writeDirectory)
        {
            _injectionKernel = injectionKernel;
            _applicationName = applicationName;
            _writeDirectory = writeDirectory;

            Configure();

            return;
        }

        private void Dispose(bool disposing)
        {
            if (false == _disposed)
            {
                if (true == disposing)
                {
                    
                }

                _disposed = true;
            }

            return;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);

            return;
        }
    }
}
