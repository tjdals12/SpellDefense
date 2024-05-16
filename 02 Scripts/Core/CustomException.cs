using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.CustomException {
    public class ValidationException : Exception {
        public ValidationException() : base("ERR_INVALID_REQUEST") {}
        public ValidationException(string message) : base(message) {}
    }
}
