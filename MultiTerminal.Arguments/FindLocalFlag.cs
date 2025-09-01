namespace MultiTerminal.Arguments
{
    public class FindLocalFlag
    {
        public static bool Find(string Input, string Flag, out string Output)
        {
            if (Input.Contains("--m:" + Flag))
            {
                Output = Input.Replace("--m:" + Flag, "");
                return true;
            }
            Output = Input;
            return false;
        }
    }
}
