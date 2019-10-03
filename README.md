
# Magic Lambda Logging

[![Build status](https://travis-ci.org/polterguy/magic.lambda.logging.svg?master)](https://travis-ci.org/polterguy/magic.lambda.logging)

Log4net wrapper for [Magic](https://github.com/polterguy/magic). More specifically, this project provides the following slots.

* __[log.info]__ - Information log entries, typically smaller pieces of information
* __[log.debug]__ - Debug log entries, typically additional debugging information not enabled in production
* __[log.error]__ - Error log entries, typically exceptions
* __[log.fatal]__ - Fatal log entries, from which the application cannot recover from

## Configuration

Notice, you're responsible for configuring Log4Net yourself, which can normally be done with something such as the following
in for instance your _"Program.cs"_ as your application is starting up.

```csharp
/*
 * This piece of code depends upon that you have an XML file configuring
 * log4net called "log4net.config".
 */
var log4netConfig = new XmlDocument();
log4netConfig.Load(File.OpenRead(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
```

Then you can create a _"log4net.config"_ file with the following content.

```xml
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
  <appender name="RollingFile" type="log4net.Appender.FileAppender">
    <file value="logs/magic.log" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%-5p %d{hh:mm:ss} %message%newline" />
    </layout>
  </appender>
  <root>
    <level value="ALL" />
    <appender-ref ref="RollingFile" />
  </root>
</log4net>
```

To see more details about how to configure Log4Net, feel free to 
read the [log4net documentation](https://logging.apache.org/log4net/release/features.html).

## License

Magic is licensed as Affero GPL. This means that you can only use it to create Open Source solutions.
If this is a problem, you can contact at thomas@gaiasoul.com me to negotiate a proprietary license if
you want to use the framework to build closed source code. This will allow you to use Magic in closed
source projects, in addition to giving you access to Microsoft SQL Server adapters, to _"crudify"_
database tables in MS SQL Server. I also provide professional support for clients that buys a
proprietary enabling license.
