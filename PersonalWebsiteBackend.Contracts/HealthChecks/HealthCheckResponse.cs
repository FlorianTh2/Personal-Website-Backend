﻿using System;
using System.Collections.Generic;

namespace PersonalWebsiteBackend.Contracts.HealthChecks
{
    public class HealthCheckResponse
    {
        public string Status { get; set; }

        public IEnumerable<HealthCheck> Checks{ get; set; }

        public TimeSpan Duration { get; set; }
    }
}