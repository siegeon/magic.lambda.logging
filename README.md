
# Magic Lambda Logging

[![Build status](https://travis-ci.org/polterguy/magic.lambda.logging.svg?master)](https://travis-ci.org/polterguy/magic.lambda.logging)

Audit logging wrapper slots for [Magic](https://github.com/polterguy/magic). More specifically, this project provides the following slots.

* __[log.info]__ - Information log entries, typically smaller pieces of information
* __[log.debug]__ - Debug log entries, typically additional debugging information not enabled in production
* __[log.error]__ - Error log entries, typically exceptions
* __[log.fatal]__ - Fatal log entries, from which the application cannot recover from

All of the above slots also have async implementation, starting out with `wait.`.

By default, this project will log into your `magic.log_entries` database/table, using either MySQL or
Microsoft SQL Server. This allows you to use SQL to generate statistics on top of your logs. An example of
logging an info piece of information can be found below.

```
log.info:Howdy world from magic.lambda.logging!
```

## License

Although most of Magic's source code is publicly available, Magic is _not_ Open Source or Free Software.
You have to obtain a valid license key to install it in production, and I normally charge a fee for such a
key. You can [obtain a license key here](https://servergardens.com/buy/).
Notice, 7 days after you put Magic into production, it will stop functioning, unless you have a valid
license for it.

* [Get licensed](https://servergardens.com/buy/)
