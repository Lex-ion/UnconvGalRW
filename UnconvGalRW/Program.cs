namespace UnconvGalRW
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            CompositeDisplayRenderObject.ImageSourcePath = @"C:\Users\Killki\source\repos\Lex-ion\Unconventional-gallery\Unconventional galery\Resources\container.png";

            App app = new App();
            app.Run();
        }
    }
}