using ArmatSoftware.Code.Engine.Core;
using ArmatSoftware.Code.Engine.Core.Storage;
using ArmatSoftware.Code.Engine.Storage.File;

namespace ArmatSoftware.Code.Engine.Tester.WebApi;

public class CustomCodeProvider : IActionProvider
{
    public IEnumerable<ISubjectAction<T>> Retrieve<T>() where T : class
    {
        var stringOnlyActions = new StoredActions<StringOnlySubject>();
        stringOnlyActions.Add("StringAction").Update("Subject.Data = \"hello world!\";", "author", "comment");
        
        var numAndStringActions = new StoredActions<NumericAndStringSubject>();
        numAndStringActions.Add("NumStringAction").Update("Subject.StringData = \"hello world!\";", "author", "comment");

        
        switch (typeof(T).FullName)
        {
            case nameof(StringOnlySubject):
                return stringOnlyActions.AsEnumerable().Select(a => a as ISubjectAction<T>);
            case nameof(NumericAndStringSubject):
                return numAndStringActions.AsEnumerable().Select(a => a as ISubjectAction<T>);
            default:
                throw new ArgumentOutOfRangeException(nameof(T), typeof(T).FullName, null);
        }
    }
}