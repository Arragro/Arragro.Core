using Arragro.Core.Common.Logging;
using System.Diagnostics;
using Xunit;

namespace Arragro.Core.Common.Tests.Logging.UseCase
{
    public class LoggingUseCase
    {
        [Fact]
        public void LoggingUseCaseWithDebugLogManager()
        {
            LogManager.LogFactory = new DebugLogManager();
            var logger = LogManager.GetLogger("TestLogger");
            logger.Debug("Hello");
            logger.DebugFormat("Foo {0}", "Bar");
        }
    }
}
