﻿using System.Collections;
using System.Collections.Generic;

namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}