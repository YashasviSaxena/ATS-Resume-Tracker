using System;
using System.Collections.Generic;
using System.Text;

namespace ATS.Core.Interfaces
{
    public interface IResumeParser
    {
        string ExtractText(string filePath);
    }
}
