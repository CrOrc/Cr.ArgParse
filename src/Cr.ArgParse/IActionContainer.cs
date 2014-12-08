using Cr.ArgParse.Actions;

namespace Cr.ArgParse
{
    public interface IActionContainer
    {
        void RemoveAction(Action action);
    }
}