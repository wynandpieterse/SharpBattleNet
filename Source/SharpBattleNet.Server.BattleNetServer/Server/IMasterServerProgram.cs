namespace SharpBattleNet.Server.MasterServer.Server
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IMasterServerProgram
    {
        void Start();
        void Stop();
    }
}
