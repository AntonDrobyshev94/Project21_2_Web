using Project19.ContextFolder;
using Project19.Entitys;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Project19.AuthContactApp;
using Microsoft.AspNetCore;
using System.Data;
using System.Transactions;
using System.Data.Common;
using Newtonsoft.Json;

namespace Project19.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Метод инициализации (происходит в процессе запуска)
        /// в результате которого происходит проверка на наличие
        /// в базе данных, таблицы AspNetUser любого пользователя.
        /// Если пользователи отсутствуют, то происходит создание
        /// пароля и с помощью метода HashPassword происходит его
        /// Хэширование. Далее, создается ноый пользователь с 
        /// указанным именем и Хэш паролем.
        /// Далее, происходит открытие новой транзакции, в которой
        /// с помощью метода Add происходит добавление в базу данных 
        /// указанного пользователя. Далее, методом ExecuteSqlRawAsync
        /// происходит включение автоинкрементации, сохранение данных
        /// методом SaveChanges и отключение автоинкрементации.
        /// Методом Commit происходит отключение транзакции.
        /// Если пользователь в БД уже есть, то метод завершается (не
        /// выполняется).
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(DataContext context)
        {
            context.Database.EnsureCreated();
            if (context.Users.Any()) return;
            //Создаем пользователя
            string password = "12345Qq!";
            var passwordHasher = new PasswordHasher<User>();
            string hashedPassword = passwordHasher.HashPassword(null, password);
            User user = new User() { UserName="Admin", PasswordHash = hashedPassword, NormalizedUserName ="ADMIN" };

            using (var trans = context.Database.BeginTransaction())
            {
                context.Users.Add(user);
                context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[AspNetUsers] ON");
                context.SaveChanges();
                context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[AspNetUsers] OFF");

                trans.Commit();
            }
        }

        /// <summary>
        /// Метод инициализации роли(происходит в процессе запуска)
        /// в результате которого происходит проверка на наличие
        /// в базе данных, таблицы AspNetRoles любой роли.
        /// Если роли отсутствуют, то создается экземпляр роли 
        /// с указанием имени. ID присваивается автоматически.
        /// Далее, происходит открытие новой транзакции, в которой
        /// с помощью метода Add происходит добавление в базу данных 
        /// указанной роли. Методом ExecuteSqlRawAsync происходит 
        /// включение автоинкрементации, сохранение данных
        /// методом SaveChanges и отключение автоинкрементации.
        /// Методом Commit происходит отключение транзакции.
        /// Если роль в БД уже есть, то метод завершается (не 
        /// выполняется)
        /// </summary>
        /// <param name="context"></param>
        public static void InitializeRole(DataContext context)
        {
            if (context.Roles.Any()) return;
            //Создаем роль
            IdentityRole role = new IdentityRole {Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = null };
            
            using (var trans = context.Database.BeginTransaction())
            {
                context.Roles.Add(role);
                context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[AspNetRoles] ON");
                context.SaveChanges();
                context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[AspNetRoles] OFF");

                trans.Commit();
            }
        }

        /// <summary>
        /// Метод инициализации роли для юзера (происходит в 
        /// процессе запуска) в результате которого происходит 
        /// проверка на наличие в базе данных, таблицы 
        /// AspNetUserRoles любой роли, установленной для
        /// пользователя. Если роли для пользователя отсутствуют, 
        /// то создается экземпляр IdentityUserRole 
        /// параметризированной string переменной (является ключем).
        /// С помощью метода First происходит поиск значений ID в 
        /// таблице по ролям и в таблице пользователей, которые
        /// подставляются в создаваемый экземпляр.
        /// Далее, происходит открытие новой транзакции, в которой
        /// с помощью метода Add происходит добавление в базу данных 
        /// указанной роли пользователя. Методом ExecuteSqlRawAsync 
        /// происходит включение автоинкрементации, сохранение данных
        /// методом SaveChanges и отключение автоинкрементации.
        /// Методом Commit происходит отключение транзакции.
        /// Если роль пользователя в БД уже есть, то метод завершается 
        /// (не выполняется).
        /// </summary>
        /// <param name="context"></param>
        public static void InitializeUserRole(DataContext context)
        {
            if (context.UserRoles.Any()) return;
            var roleId = context.Roles.First().Id;
            var userId = context.Users.First().Id;
            IdentityUserRole<string> userRole = new IdentityUserRole<string> { UserId = userId, RoleId = roleId };
            using (var trans = context.Database.BeginTransaction())
            {
                context.UserRoles.Add(userRole);
                context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[AspNetUserRoles] ON");
                context.SaveChanges();
                context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[AspNetUserRoles] OFF");

                trans.Commit();
            }
        }
    }
}

