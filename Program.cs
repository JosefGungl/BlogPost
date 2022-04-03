using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace BlogsConsole
{
    class Program
    {
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                var db = new BloggingContext();

                string choice = "";
                do{
                    Console.WriteLine("1. Display all blogs");
                    Console.WriteLine("2. Add a blog");
                    Console.WriteLine("3. Create a post");
                    Console.WriteLine("4. Display posts");
                    Console.WriteLine("Anything else to quit");
                    choice = Console.ReadLine();
                    Console.WriteLine("");

                    if (choice == "1"){
                        // Display all Blogs from the database
                        var query = db.Blogs.OrderBy(b => b.Name);
                        Console.WriteLine("All blogs in the database:");
                        foreach (var item in query)
                        {
                            Console.WriteLine(item.Name);
                        }
                    }else if (choice == "2"){
                        // Create and save a new Blog
                        Console.Write("Enter a name for a new Blog: ");
                        var name = Console.ReadLine();
                        var blog = new Blog { Name = name };
                        db = new BloggingContext();
                        db.AddBlog(blog);
                        logger.Info("Blog added - {name}", name);
                    }else if (choice == "3"){

                    }else if (choice == "4"){

                    }
                }while (choice =="1" || choice =="2" || choice =="3" || choice =="4");
                
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
    }
}