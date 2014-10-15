using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpBattleNet.Runtime.Utilities.Logging
{
    /// <summary>
    /// Represents a way to get a <see cref="ILog"/>
    /// </summary>
    public interface ILogProvider
    {
        ILog GetLogger(string name);
    }
}
