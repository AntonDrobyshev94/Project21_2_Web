using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project19.AuthContactApp;
using Project19.ContextFolder;
using Project19.Interfaces;
using System.Security.Claims;

namespace Project19.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                RoleManager<IdentityRole> roleManager
                                
                                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Асинхронный Get запрос на открытие страницы администратора. 
        /// В результате запроса происходит формирование коллекции IList 
        /// string,в которую происходит запись ролей текущего пользователя.
        /// Полученная переменная записывается в ViewBag.Role для
        /// использования в представлении. Также происходит формирование
        /// коллекций IEnumerable имён всех пользователей и имён
        /// пользователей с правами администратора, которые записываются
        /// во ViewBag.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddRole()
        {
            IList<string> role = new List<string> {"Роль не определена"};
            var usr = await _userManager.GetUserAsync(this.User);
            if (usr != null)
            {
                role = await _userManager.GetRolesAsync(usr);
            }

            IEnumerable<User> users = await _userManager.Users.ToListAsync();
            IEnumerable<User> usersInRoleAdmin = await _userManager.GetUsersInRoleAsync("Admin");
            
            UserRegistration userRegistration = new UserRegistration();
            
            ViewBag.Role = role;
            ViewBag.AllUsers = users;
            ViewBag.Admins = usersInRoleAdmin;

            return View(userRegistration);
        }

        /// <summary>
        /// Асинхронный Post запрос создания новой роли, принимающий 
        /// string переменную roleName (название роли). В блоке
        /// try catch происходит проверка на пустую строку, а также
        /// с помощью метода RoleExistAsync производится проверка
        /// на совпадение указанной роли и имеющихся ролей.
        /// В экземпляры TempData записываются сообщения о результате
        /// выполнения операции.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNewRole(string roleName)
        {
            bool isCreate = false;
            try
            {
                if (!string.IsNullOrEmpty(roleName))
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        await _roleManager.CreateAsync(new IdentityRole()
                        {
                            Name = roleName,
                            NormalizedName = roleName
                        });
                        TempData["RoleCreateMessage"] = "Роль успешно добавлена!";
                        isCreate = true;
                    }
                    else
                    {
                        TempData["RoleCreateMessage"] = "Роль уже существует";
                        isCreate = false;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["RoleCreateMessage"] = $"{ex}";
                isCreate = false;
            }
            TempData["IsCreate"] = isCreate;
            return RedirectToAction("AddRole", "Account");
        }

        /// <summary>
        /// Асинхронный post запрос добавления роли указанному
        /// пользователю, принимающий переменные строкового типа
        /// roleName и userName (название роли и имя пользователя).
        /// Происходят проверки на наличие указанной роли и имени 
        /// пользователя в существующей базе данных (c помощью 
        /// методов RoleExistAsync и FindByNameAsync).
        /// Добавление роли осуществляется методом AddToRoleAsync.
        /// В экземпляры TempData записываются сообщения о 
        /// результатах выполнения операций.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUserRole(string roleName, string userName)
        {
            bool isRoleAvailable = false;
            bool isUserAvailable = false;
            var user = await _userManager.FindByNameAsync(userName);
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["MessageRole"] = "Роль доступна для добавления";
                isRoleAvailable = true;
                if (user !=null)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                    TempData["UserMessage"] = "Пользователь указан верно";
                    TempData["SuccessMessage"] = "Роль успешно добавлена";
                    isRoleAvailable = true;
                    isUserAvailable = true;
                }
                else
                {
                    TempData["UserMessage"] = "Пользователь отсутствует";
                    isUserAvailable= false;
                }
            }
            else
            {
                if (user != null)
                {
                    TempData["UserMessage"] = "Пользователь указан верно";
                    isUserAvailable = true;
                }
                else
                {
                    TempData["UserMessage"] = "Пользователь отсутствует";
                    isUserAvailable = false;
                }
                TempData["MessageRole"] = "Ошибка: указанная роль не существует";
                isRoleAvailable = false;
            }
            TempData["isRoleAvailable"] = isRoleAvailable;
            TempData["isUserAvailable"] = isUserAvailable;
            return RedirectToAction("AddRole", "Account");
        }

        /// <summary>
        /// Асинхронный Post запрос на удаление роли у указанного
        /// пользователя, принимающий переменные строкового типа
        /// roleName и userName (название роли и имя пользователя).
        /// Происходят проверки на наличие указанной роли и имени 
        /// пользователя в существующей базе данных (c помощью 
        /// методов RoleExistAsync и FindByNameAsync).
        /// Удаление производится методом RemoveFromRoleAsync.
        /// В экземпляры TempData записываются сообщения о 
        /// результатах выполнения операций.
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUserRole(string roleName, string userName)
        {
            bool isRoleAvailable = false;
            bool isUserAvailable = false;
            var user = await _userManager.FindByNameAsync(userName);
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                TempData["MessageDeleteRole"] = "Роль доступна для удаления";
                isRoleAvailable = true;
                if (user != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                    TempData["UserDeleteMessage"] = "Пользователь указан верно";
                    TempData["DeleteMessage"] = "Роль успешно удалена";
                    isRoleAvailable = true;
                    isUserAvailable = true;
                }
                else
                {
                    TempData["UserDeleteMessage"] = "Пользователь отсутствует";
                    isUserAvailable = false;
                }
            }
            else
            {
                if (user != null)
                {
                    TempData["UserDeleteMessage"] = "Пользователь указан верно";
                    isUserAvailable = true;
                }
                else
                {
                    TempData["UserDeleteMessage"] = "Пользователь отсутствует";
                    isUserAvailable = false;
                }
                TempData["MessageDeleteRole"] = "Ошибка: указанная роль не существует";
                isRoleAvailable = false;
            }
            TempData["isRoleAvailable"] = isRoleAvailable;
            TempData["isUserAvailable"] = isUserAvailable;
            return RedirectToAction("AddRole", "Account");
        }

        /// <summary>
        /// Асинхронный Post запрос на удаления пользователя, 
        /// принимающий переменную userName строкового типа. 
        /// Происходит проверка на наличие указанного имени 
        /// пользователя в существующей базе данных (c помощью 
        /// метода FindByNameAsync). Удаление происходит
        /// при помощи метода DeleteAsync с указанием 
        /// полученного в результате метода FindByNameAsync
        /// экземпляра. В экземпляры TempData записываются 
        /// сообщения о результате выполнения операции.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveUser(string userName)
        {
            bool isRemoveUser;
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                TempData["DeleteUserMessage"] = "Пользователь успешно удален";
                isRemoveUser = true;
            }
            else
            {
                TempData["DeleteUserMessage"] = "Пользователь отсутствует";
                isRemoveUser = false;
            }
            TempData["IsRemoveUser"] = isRemoveUser;
            return RedirectToAction("AddRole", "Account");
        }

        /// <summary>
        /// Get запрос, в результате которого происходит переход
        /// на страницу Login (входа в аккаунт). В результате
        /// запроса происходит запоминание адреса страницы 
        /// при помощи ключевого слова return для дальнейшего
        /// возврата на эту страницу.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new UserLogin()
            {
                ReturnUrl = returnUrl
            });
        }

        /// <summary>
        /// Асинхронный post запрос, в результате
        /// которого происходит обработка события входа
        /// в аккаунт. Если модель UserLogin исправна,
        /// то происходит отработка метода 
        /// PasswordSignInAsync, который принимает логин и
        /// пароль, указанный в модели представления.
        /// Если результат входа успешный, то происходит
        /// возврат на страницу, на которую пытался войти
        /// пользователь (при возможности).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                if (model.LoginProp != null)
                {
                    var loginResult = await _signInManager.PasswordSignInAsync(model.LoginProp,
                    model.Password,
                    false,
                    lockoutOnFailure: false);

                    if (loginResult.Succeeded)
                    {
                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);  
                        }
                        return RedirectToAction("Index", "MyDefault");
                    }
            }
        }
            ModelState.AddModelError("", "Пользователь не найден");
            return View(model);
        }

        /// <summary>
        /// Get запрос на открытие формы регистрации, который
        /// отправляет новый экземпляр UserRegistration в
        /// представление Register в качестве модели.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserRegistration());
        }

        /// <summary>
        /// Асинхронный Post запрос, принимающий модель регистрации,
        /// проверяющий правильность этой модели и на ее основе 
        /// создающий нового пользователя и добавляющий в базу 
        /// данных. Далее, производит вход и переадресацию на
        /// страницу Index. Если модель не корректная, то выдается
        /// ошибка.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegistration model)
        {
            if (ModelState.IsValid)
            {
                if (model.LoginProp != null)
                {
                    var user = new User { UserName = model.LoginProp };
                    var createResult = await _userManager.CreateAsync(user, model.Password);

                    if (createResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return RedirectToAction("Index", "MyDefault");
                    }
                    else//иначе
                    {
                        foreach (var identityError in createResult.Errors)
                        {
                            ModelState.AddModelError("", identityError.Description);
                        }
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// Get запрос на открытие формы регистрации нового пользователя
        /// от имени Администратора
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminRegister()
        {
            return View(new UserRegistration());
        }

        /// <summary>
        /// Асинхронный Post запрос, принимающий модель регистрации,
        /// проверяющий правильность этой модели и на ее основе 
        /// создающий нового пользователя и добавляющий в базу 
        /// данных. Если модель не корректная, то выдается
        /// сообщение об ошибке. Если действие выполнено, то 
        /// выдается сообщение об успешном выполнении.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminRegister(UserRegistration model)
        {
            bool isSucceed = false;
            if (ModelState.IsValid)
            {
                if (model.LoginProp != null)
                {
                    var user = new User { UserName = model.LoginProp };
                    var createResult = await _userManager.CreateAsync(user, model.Password);

                    if (createResult.Succeeded)
                    {
                        isSucceed = true;
                    }
                    else//иначе
                    {
                        foreach (var identityError in createResult.Errors)
                        {
                            ModelState.AddModelError("", identityError.Description);
                        }
                    }
                }
            }
            else
            {
                isSucceed = false;
            }
            ViewBag.IsSuccess = isSucceed;
            TempData["UserCreateMessage"]= isSucceed? "Пользовательский аккаунт создан": "Ошибка при создании";
            return View(model);
        }

        /// <summary>
        /// Асинхронный Post запрос выхода из учетной записи.
        /// </summary>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "MyDefault");
        }
    }
}
