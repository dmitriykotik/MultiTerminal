using MultiTerminal.Plugin;

namespace TestPlugin
{
    public class TestPlugin : IPlugin
    {
        public string Name => "TestPlugin";

        public void OnEnable()
        {
            Console.WriteLine($"{Name} включён!");
        }

        public void OnDisable()
        {
            Console.WriteLine($"{Name} выключен!");
        }

        public void RegisterCommands(ICommandRegistrar registrar)
        {
            registrar.RegisterCommand("hello", args =>
            {
                Console.WriteLine($"Hello, World!");
            });
        }
    }
}
