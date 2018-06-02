﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesExceptionCollection : Exception
    {
        public List<RulesException> RulesExceptions { get; protected set; }

        public RulesExceptionCollection() : base()
        {
            RulesExceptions = new List<RulesException>();
        }

        public RulesExceptionCollection(IEnumerable<RulesException> rulesExceptions) : this()
        {
            RulesExceptions.AddRange(rulesExceptions);
        }

        public RulesExceptionDto GetRulesExceptionDto()
        {
            return new RulesExceptionDto(RulesExceptions);
        }

        public void ThrowException()
        {
            if (RulesExceptions.Any(x => x.ErrorMessages.Any() || x.Errors.Any()))
                throw this;
        }
    }
}
