﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangePi.Display.Status.Service.InfoServices
{
    public interface IDateTimeInfoService:IDisplayInfoService
    {
        Task<DateTime> GetValue();
    }
}
