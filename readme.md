### Description

Imagine you have a DbFunction of the form:
```C#
[DbFunction("NormalizeString", "dbo")]
public static string NormalizeString(string str)
    => throw new NotImplementedException();
```

If you were to apply an IndexOf function to the result of this function (which would be a string), it would throw a "NullReferenceException: Object reference not set to an instance of an object" error.

```C#
//Throws: 'Object reference not set to an instance of an object.'
await dbContext.People
        .Where(person => person.FullName.Contains(str))
        .OrderBy(person => DbStringFunctions.NormalizeString(person.FullName).IndexOf(str))
        .ToListAsync();
```

This used to work in .Net 3.1, but throws the NullReferenceException since .Net 5.

Could this be related to https://github.com/dotnet/runtime/issues/43736? I'm not sure how, though...

### Workaround

A workaround (also illustrated in the sample I provided) would be to create a scalar function

```SQL
CREATE FUNCTION [dbo].[IndexOf]
(
	@input nvarchar(max),
	@stringToMatch nvarchar(max)
)
RETURNS int
AS
BEGIN
	return charindex(@stringToMatch, @input)
END
```

Then create another DbFunction of it:
```C#
[DbFunction("IndexOf", "dbo")]
public static int IndexOf(string input, string stringToMatch)
    => throw new NotImplementedException();
```

And replace the usual string.IndexOf call with the DbFunction created above:
```C#
//Works
await dbContext.People
        .Where(person => person.FullName.Contains(str))
        .OrderBy(person => DbStringFunctions.IndexOf(DbStringFunctions.NormalizeString(person.FullName), str))
        .ToListAsync();
```


### Code

The relevant code is situated in `Program.cs`  for .Net 6 or `Startup.cs` for .Net 5 and .Net Core 3.1.
`EfCoreIndexOfStringBug.Database` simply deploys a test database using DbUp. You could also manually run the scripts found in the Migrations folder.
This code sample demonstrates how it works in .Ef Core 3.1 but the same code does not in .Ef Core 5 and .Ef Core 6. It also illustrates a workaround in the form of another DbFunction replacing the native string.IndexOf.

### Stack traces
```
System.NullReferenceException
  HResult=0x80004003
  Message=Object reference not set to an instance of an object.
  Source=Microsoft.EntityFrameworkCore.SqlServer
  StackTrace:
   at Microsoft.EntityFrameworkCore.SqlServer.Query.Internal.SqlServerStringMethodTranslator.Translate(SqlExpression instance, MethodInfo method, IReadOnlyList`1 arguments, IDiagnosticsLogger`1 logger)
   at Microsoft.EntityFrameworkCore.Query.RelationalMethodCallTranslatorProvider.<>c__DisplayClass7_0.<Translate>b__0(IMethodCallTranslator t)
   at System.Linq.Enumerable.SelectEnumerableIterator`2.MoveNext()
   at System.Linq.Enumerable.TryGetFirst[TSource](IEnumerable`1 source, Func`2 predicate, Boolean& found)
   at Microsoft.EntityFrameworkCore.Query.RelationalMethodCallTranslatorProvider.Translate(IModel model, SqlExpression instance, MethodInfo method, IReadOnlyList`1 arguments, IDiagnosticsLogger`1 logger)
   at Microsoft.EntityFrameworkCore.Query.RelationalSqlTranslatingExpressionVisitor.VisitMethodCall(MethodCallExpression methodCallExpression)
   at System.Linq.Expressions.MethodCallExpression.Accept(ExpressionVisitor visitor)
   at System.Linq.Expressions.ExpressionVisitor.Visit(Expression node)
   at Microsoft.EntityFrameworkCore.Query.RelationalSqlTranslatingExpressionVisitor.TranslateInternal(Expression expression)
   at Microsoft.EntityFrameworkCore.Query.RelationalQueryableMethodTranslatingExpressionVisitor.TranslateExpression(Expression expression)
   at Microsoft.EntityFrameworkCore.Query.RelationalQueryableMethodTranslatingExpressionVisitor.TranslateOrderBy(ShapedQueryExpression source, LambdaExpression keySelector, Boolean ascending)
   at Microsoft.EntityFrameworkCore.Query.QueryableMethodTranslatingExpressionVisitor.VisitMethodCall(MethodCallExpression methodCallExpression)
   at System.Linq.Expressions.MethodCallExpression.Accept(ExpressionVisitor visitor)
   at System.Linq.Expressions.ExpressionVisitor.Visit(Expression node)
   at Microsoft.EntityFrameworkCore.Query.QueryCompilationContext.CreateQueryExecutor[TResult](Expression query)
   at Microsoft.EntityFrameworkCore.Query.Internal.QueryCompiler.<>c__DisplayClass12_0`1.<ExecuteAsync>b__0()
   at Microsoft.EntityFrameworkCore.Query.Internal.CompiledQueryCache.GetOrAddQuery[TResult](Object cacheKey, Func`1 compiler)
   at Microsoft.EntityFrameworkCore.Query.Internal.QueryCompiler.ExecuteAsync[TResult](Expression query, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider.ExecuteAsync[TResult](Expression expression, CancellationToken cancellationToken)
   at Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1.GetAsyncEnumerator(CancellationToken cancellationToken)
   at System.Runtime.CompilerServices.ConfiguredCancelableAsyncEnumerable`1.GetAsyncEnumerator()
   at Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.<ToListAsync>d__65`1.MoveNext()
   at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw()
   at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
   at System.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
   at Program.<>c.<<<Main>$>b__0_1>d.MoveNext() in C:\Projects\Personal\EfCoreIndexOfStringBug\EfCoreIndexOfStringBug\EfCoreIndexOfStringBug.Net6\Program.cs:line 35

  This exception was originally thrown at this call stack:
    [External Code]
    Program.<Main>$.AnonymousMethod__0_1(EfCoreIndexOfStringBug.Net6.EfCoreIndexOfStringBugDbContext) in Program.cs
```

### Provider and version information

EF Core version: 6.0.9 (and 5.0.17)
Database provider: Microsoft.EntityFrameworkCore.SqlServer
Target framework: .NET 6 (and .NET 5)
Operating system: Windows 10 Pro 19043.2006
IDE: Microsoft Visual Studio Community 2022 (64-bit) 17.2.6
