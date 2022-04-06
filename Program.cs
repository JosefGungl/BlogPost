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
                        if (name != null){
                            var blog = new Blog { Name = name };
                            db = new BloggingContext();
                            db.AddBlog(blog);
                            logger.Info("Blog added - {name}", name);
                        }
                        

                    }else if (choice == "3"){
                        string select = "";
                        int i = 0;
                        Console.WriteLine("What blog do you want to post to?");
                        select = Console.ReadLine();
                        var query = db.Blogs;
                        foreach(var blog in query){
                            if(blog.Name == select){
                                i++;
                            }
                        }
                        if(i >=1){
                            foreach(var blog in query){
                            if(blog.Name == select){
                                Console.Write("Enter post name: ");
                                var postName = Console.ReadLine();
                                Console.WriteLine("Enter post content");
                                var postContent = Console.ReadLine();
                                int tempBlogID = blog.BlogId;
                                var post = new Post{Title = postName,
                                                    Content = postContent,
                                                    BlogId = tempBlogID};
                                db = new BloggingContext();
                                db.AddPost(post);
                                logger.Info("Post added - {title}", postName);
                                }    
                            }
                        }else if (i == 0){
                            logger.Info("Invalid name.");
                        }
                        
                        
                    }else if (choice == "4"){
                        int i = 0;
                        int id = 0;
                        Console.WriteLine("What blog do you want to go to?");
                        string choice2 = Console.ReadLine();
                        var query = db.Blogs;
                        foreach(var item in query){
                            if(item.Name == choice2){
                                id = item.BlogId;
                                var query2 = db.Posts;
                                foreach(var post in query2){
                                if(post.BlogId == id){
                                    i++;
                                    Console.WriteLine("Blog: "+item.Name +"\nPost Name: "+post.Title+"\nPost Content: "+post.Content);
                                    Console.WriteLine("");
                            }
                        }
                            }
                        }
                        Console.WriteLine($"Nmber of posts: {i}");
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