My collection of many small useful code-snippet .net, wrapping in reusable library with unobtrusive dependencies.

Some of the most-used library are published on nuget.

https://www.nuget.org/profiles/duongphuhiep

Here is some brief introduction code. Navigate to the code and unit test samples for more information.

# ToolsPack.Displayer

* No dependencies
* Use to display some C# object.

## ArrayDisplayer

* Know to convert a IEnummerable to string in order do display in a log message
```
var arr = new string[1000] {"item1".."item1000"};
arr.Display().SeparatedBy("; ").MaxItems(4)
```
gives
```
{ item1; item2; item3; item4; ..and 996 (of 1000) more }
```

* if the some of the items is very long to display in the log file

```
var arr = new string[1000] {"Lorem ipsum kidda foom", "item2".."item1000"};
arr.Display().MaxItems(4).MaxItemLength(10)
```
gives
```
{ [[Lorem...]], item2, item3, item4, ..and 996 (of 1000) more }
```

* Fast performance it only iterate neccessary items once (complexity O(N))
* see more functionalities in code and test

## StopwatchDisplayer

convert `Stopwatch` to string
```
Stopwatch sw;
Console.WriteLine(sw.DisplayMili()); //get the display string in mili seconds "103 ms"
Console.WriteLine(sw.DisplayMicro()); //get the display string in micro seconds "103,000 mcs"
Console.WriteLine(sw.Display()); //automaticly choose a time unit (day, hour, minute, seconde..) to display
```

# ToolsPack.Log4net

## Log4NetQuickSetup

In a Unit test project, or a quick console temporary application to try things, you donnot need to configure the log4net.config.

Call 
```
Log4NetQuickSetup.SetUpConsole();
```
or 
```
Log4NetQuickSetup.SetUpFile("my_small_app.log");
```
it will setup a typical log4net appender so that you can use them in your test application. Example:
```
[TestClass]
public class ArrayDisplayerTests
{
    private static readonly ILog Log = LogManager.GetLogger(typeof (ArrayDisplayerTests));

    [ClassInitialize]
    public static void SetUp(TestContext testContext)
    {
        Log4NetQuickSetup.SetUpConsole();
    }

    [TestMethod]
    public void DisplayTest()
    {
        Log.Info("Hello it will display to the Console");
    }
}
```

See also [code-snippet to quickly configure log4net in a C# project](https://github.com/duongphuhiep/ToolsPack.Net/wiki/log4net)

## ConfigReader

use it to read `app.config`

Example `app.config` of your application
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <appSettings>
        <add key="connectionString" value="Server=localhost;Database=foo"/>
        <add key="activePingService" value="true"/>
        <add key="pollIteration" value="100"/>
    </appSettings>
</configuration>
```
You can read these value in your C# application
```
ConfigReader.Read<string>("connectionString", "a default value if config not found");
ConfigReader.Read<bool>("activePingService", false);
ConfigReader.Read<int>("pollIteration", -1);
```

## ElapsedTimeWatcher

Micro-benchmark a part of code to investigate on performance
```
class MyCalculator 
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(MyCalculator));

    public void Process()
    {
        using (var etw = ElapsedTimeWatcher.Create(Log, "blockCodeName"))
        {
            ...
            etw.Info("step 1");
            ...
            etw.DebugFormat("step 2");
            ...
            etw.Info("Step 3)");
            ...
        } //"sum up log" is displayed here 
    }
}
```
* The `etw` wrap the usual logger `Log`, we use `etw` to log message instead of the usual `Log`
* the `blockCodeName` is repeated in the start of each log message, so that we can filter log message by "blockCodeName"
* Each log message will display the elapsed time (in micro-second) since the last log message.
* A **sum up log** will display the total elapsed time (in micro-second) when the `etw` object is disposed.

```
22:56:59,866 [DEBUG] Begin blockCodeName
22:56:59,970 [INFO ] blockCodeName - 102350 mcs - step 1
22:57:00,144 [DEBUG] blockCodeName - 173295 mcs - step 2
22:57:00,259 [INFO ] blockCodeName - 114036 mcs - Step 3)
22:57:00,452 [INFO ] End blockCodeName : Total elapsed 585436 mcs
```

**Auto Jump Log Level**
```
var etw = ElapsedTimeWatcher.Create(Log, "checkIntraday").InfoEnd().AutoJump(150, 250).AutoJumpLastLog(500, 1000)
```
* The log level will auto jump to INFO if the elapsed time exceeds 150 ms
* The log level will auto jump to WARN if the elapsed time exceeds 250 ms
* The above **sum up log** will switch to INFO if the total elapsed time exceeds 500 ms 
* The above **sum up log** will switch to WARN if the total elapsed time exceeds 1 sec

# ToolsPack.Sql

Avoid redundancy of pure ADO.NET code 

* http://www.blackbeltcoder.com/Articles/ado/an-ado-net-sql-helper-class  
 * I've made an improvement so that we can declare the length of VarChar parameters. [It is recommended to always declare length of the VarChar parameters](http://blogs.msdn.com/b/psssql/archive/2010/10/05/query-performance-and-plan-cache-issues-when-parameter-length-not-specified-correctly.aspx)
* http://stackoverflow.com/a/18551053/347051

```
string qry = "SELECT.. FROM.. WHERE ArtApproved = @Approved AND ArtUpdated > @Updated AND name <> @Foo";
using (AdoHelper db = new AdoHelper(connectionString))
{
    using (SqlDataReader rdr = db.ExecDataReader(qry, 
        "@Approved", true,
        "@Fuu", "Beuh",
        "@Foo", "Bazz", 50 //50 is the parameter size to optimize query cache in some case
        "@Updated", new DateTime(2011, 3, 1)))
    {
        while (rdr.Read())
        {
            rdr.GetValue<int?>("views");
            rdr.GetValue<DateTime?>("lastModified");
        }
    }
}
```
# ToolsPack.Thread

## TimedLock

https://github.com/Haacked/TimedLock

```
using(TimedLock.Lock(obj, TimeSpan.FromSeconds(10)))
{
    //Thread safe operations
}
```
* The "synchronized code" will wait for other lock on `obj` free.
* `TimeOutException` if the lock acquiring is longer than 10 sec

## NamedLocker

```
static readonly NamedLocker<string> CustomerLocker = new NamedLocker<string>();
customerLocker.RunWithLock("Peter.Buy", () =>
{
    //synchronized code
}
```
* The "synchronized code" will wait for other "Peter.Buy"` key free.

## MultiNamedTimedLocker
```
static readonly MultiNamedTimedLocker<string> CustomerLocker = new MultiNamedTimedLocker<string>();

using (customerLocker.Lock(new[] {"peter", "david"}, 100))
{
    //synchronized code
}
```
* The "synchronized code" will wait until the `"peter"` and `"buy"` key of the `CustomerLocker` object are free.
* After 100 mili-second of waiting: `TimeOutException`
