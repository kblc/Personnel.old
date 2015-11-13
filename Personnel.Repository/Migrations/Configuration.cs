namespace Personnel.Repository.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Personnel.Repository.Model.RepositoryContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Personnel.Repository.Model.RepositoryContext context)
        {
            foreach (var t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces().Any(i => i.Name == "IDefaultRepositoryInitialization")))
            {
                var ci = t.GetConstructor(new Type[] { });
                if (ci != null)
                {
                    var el = ci.Invoke(new object[] { }) as Personnel.Repository.Model.IDefaultRepositoryInitialization;
                    if (el != null)
                        el.InitializeDefault(context);
                }
            }

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
