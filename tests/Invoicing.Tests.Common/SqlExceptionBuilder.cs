using System.Reflection;
using Microsoft.Data.SqlClient;
// ReSharper disable All
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


// DISCLAIMER: This is copied verbatim from a StackOverflow answer (https://stackoverflow.com/a/29939664) 
// Needed a way to create a SqlException since they're handled in the repository layer
// disabled all warnings because I didn't have the time to refactor it

namespace Invoicing.Application.Tests.Unit;

public class SqlExceptionBuilder
{
    private int errorNumber;
    private string errorMessage;

    public SqlException Build()
    {
        SqlError error = this.CreateError();
        SqlErrorCollection errorCollection = this.CreateErrorCollection(error);
        SqlException exception = this.CreateException(errorCollection);

        return exception;
    }

    public SqlExceptionBuilder WithErrorNumber(int number)
    {
        this.errorNumber = number;
        return this;
    }

    public SqlExceptionBuilder WithErrorMessage(string message)
    {
        this.errorMessage = message;
        return this;
    }

    private SqlError CreateError()
    {
        // Create instance via reflection...
        var ctors = typeof(SqlError).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        var firstSqlErrorCtor = ctors.FirstOrDefault(
            ctor =>
            ctor.GetParameters().Count() == 8); // .NetCore should be 8 not 7
        SqlError error = firstSqlErrorCtor.Invoke(
            new object[]
            {
                this.errorNumber,
                new byte(),
                new byte(),
                string.Empty,
                string.Empty,
                string.Empty,
                new int()
                ,new Exception()  // for .NetCore 
            }) as SqlError;

        return error;
    }

    private SqlErrorCollection CreateErrorCollection(SqlError error)
    {
        // Create instance via reflection...
        var sqlErrorCollectionCtor = typeof(SqlErrorCollection).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
        SqlErrorCollection errorCollection = sqlErrorCollectionCtor.Invoke(new object[] { }) as SqlErrorCollection;

        // Add error...
        typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(errorCollection, new object[] { error });

        return errorCollection;
    }

    private SqlException CreateException(SqlErrorCollection errorCollection)
    {
        // Create instance via reflection...
        var ctor = typeof(SqlException).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
        SqlException sqlException = ctor.Invoke(
            new object[]
            { 
                // With message and error collection...
                this.errorMessage,
                errorCollection,
                null,
                Guid.NewGuid()
            }) as SqlException;

        return sqlException;
    }
}
