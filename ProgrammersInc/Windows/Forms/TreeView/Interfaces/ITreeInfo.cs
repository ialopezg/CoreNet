using System;

namespace ProgrammersInc.Windows.Forms
{
    public interface ITreeInfo
    {
        IDisposable SuspendUpdates();
    }
}