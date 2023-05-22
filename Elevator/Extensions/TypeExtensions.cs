namespace ElevatorAction.ConsoleUI.Extensions;

public static class TypeExtensions
{
    ///// <summary>
    ///// Returns a list of descriptions for a type
    ///// </summary>
    ///// <param name="type"><see cref="Type"/></param>
    ///// <returns><see cref="string[]"/></returns>
    //public static string[] GetDescriptions(this Type type)
    //{
    //    var names = Enum.GetNames(type);
    //    var descs = new string[names.Length];

    //    for (int i = 0; i < names.Length; i++)
    //    {
    //        var field = type.GetField(names[i]);
    //        var fds = field!.GetCustomAttributes(typeof(DescriptionAttribute), true);
    //        foreach (DescriptionAttribute fd in fds)
    //        {
    //            descs[i] = fd.Description;
    //        }
    //    }
    //    return descs;
    //}
}
