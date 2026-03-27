using System;
using System.Collections.Generic;
using System.Text;

using ATS.Core.Interfaces;

namespace ATS.Core.Services
{
    public class ResumeService   
    {
        private readonly IResumeRepository _resumeRepository;

        public ResumeService(IResumeRepository resumeRepository)
        {
            _resumeRepository = resumeRepository;
        }
    }
}
