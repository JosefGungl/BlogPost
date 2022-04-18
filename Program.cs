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
                    Console.WriteLine("5. Delete Blog");
                    Console.WriteLine("6. Edit Blog");
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
                        var db = new BloggingContext();
                        Blog blog = InputBlog(db);
                        if (blog != null){
                            db.AddBlog(blog);
                            logger.Info("Blog added - {name}", blog.Name);
                        }
                        

                    }else if (choice == "3"){
                        string select = "";
                        int i = 0;
                        //Display available blogs
                        var query = db.Blogs.OrderBy(b => b.Name);
                        Console.WriteLine("Available blogs:");
                        foreach (var item in query)
                        {
                            Console.WriteLine(item.Name);
                        }

                        Console.Write("What blog do you want to post to: ");
                        select = Console.ReadLine();
                        var query2 = db.Blogs;
                        foreach(var blog in query2){
                            if(blog.Name == select){
                                i++;
                            }
                        }
                        if(i >=1){
                            foreach(var blog in query2){
                            if(blog.Name == select){
                                Console.Write("Enter post name: ");
                                var postName = Console.ReadLine();
                                Console.Write("Enter post content: ");
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
                        //display available blogs
                        Console.WriteLine("Available blogs:");
                        var query = db.Blogs.OrderBy(b => b.Name);
                        foreach (var item in query)
                        {
                            Console.WriteLine(item.Name);
                        }
                        //select a blog
                        Console.Write("Enter the name of the blog you would like to go to: ");
                        string choice2 = Console.ReadLine();
                        var query2 = db.Blogs;
                        foreach(var item in query2){
                            if(item.Name == choice2)
                            {
                                id = item.BlogId;
                                var query3 = db.Posts;
                                foreach(var post in query3)
                                {
                                    if(post.BlogId == id)
                                    {
                                        i++;
                                        Console.WriteLine("Blog: "+item.Name +"\nPost Name: "+post.Title+"\nPost Content: "+post.Content);
                                        Console.WriteLine("");
                                    }
                                }
                            }
                    else if (choice == "5")
                    {
                        // delete blog
                        Console.WriteLine("Choose the blog to delete:");
                        var db = new BloggingContext();
                        var blog = GetBlog(db);
                        if (blog != null)
                        {
                            //delete blog
                            db.DeleteBlog(blog);
                            logger.Info($"Blog (id: {blog.BlogId}) deleted");
                        }
                    }
                    else if (choice == "6")
                    {
                        // edit blog
                        Console.WriteLine("Choose the blog to edit:");
                        var db = new BloggingContext();
                        var blog = GetBlog(db);
                        if (blog != null)
                        {
                            // input blog
                            Blog UpdatedBlog = InputBlog(db);
                            if (UpdatedBlog != null)
                            {
                                UpdatedBlog.BlogId = blog.BlogId;
                                db.EditBlog(UpdatedBlog);
                                logger.Info($"Blog (id: {blog.BlogId}) updated");
                            }
                        }
                    }   
                    }
                    Console.WriteLine($"Nmber of posts: {i}");
                }
            }while (choice =="1" || choice =="2" || choice =="3" || choice =="4" || choice=="5" || choice=="6");
                
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }

            logger.Info("Program ended");
        }
         public static Blog GetBlog(BloggingContext db)
        {
            // display all blogs
            var blogs = db.Blogs.OrderBy(b => b.BlogId);
            foreach (Blog b in blogs)
            {
                Console.WriteLine($"{b.BlogId}: {b.Name}");
            }
            if (int.TryParse(Console.ReadLine(), out int BlogId))
            {
                Blog blog = db.Blogs.FirstOrDefault(b => b.BlogId == BlogId);
                if (blog != null)
                {
                    return blog;
                }
            }
            logger.Error("Invalid Blog Id");
            return null;
        }
        public static Blog InputBlog(BloggingContext db)
        {
            Blog blog = new Blog();
            Console.WriteLine("Enter the Blog name");
            blog.Name = Console.ReadLine();

            ValidationContext context = new ValidationContext(blog, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(blog, context, results, true);
            if (isValid)
            {
                // check for unique name
                if (db.Blogs.Any(b => b.Name == blog.Name))
                {
                    // generate validation error
                    isValid = false;
                    results.Add(new ValidationResult("Blog name exists", new string[] { "Name" }));
                }
                else
                {
                    logger.Info("Validation passed");
                }
            }
            if (!isValid)
            {
                foreach (var result in results)
                {
                    logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                }
                return null;
            }
            return blog;
        }
    }
}