using Project19.ContextFolder;
using Project19.Interfaces;
using Project19.Entitys;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;

namespace Project19.Data
{
    public class ContactData : IContactData
    {
        private readonly DataContext context;

        public ContactData(DataContext Context)
        {
            this.context = Context;
        }

        /// <summary>
        /// Метод добавления нового контакта, принимающий
        /// экземпляр контакта. Добавление происходит с
        /// помощью метода Add. Далее происходит сохранение
        /// БД с помощью метода SaveChanges.
        /// </summary>
        /// <param name="contact"></param>
        public void AddContacts(Contact contact)
        {
            context.Contacts.Add(contact);
            context.SaveChanges();
        }

        /// <summary>
        /// Метод, возвращающий коллекцию Contact
        /// (перечисление элементов коллекции)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Contact> GetContacts()
        {
            return this.context.Contacts;
        }

        /// <summary>
        /// Метод, который служит для передачи id с целью
        /// дальнейшего удаления контакта
        /// </summary>
        /// <param name="contact"></param>
        public void DeleteContact(int id)
        {
            
        }

        /// <summary>
        /// Асинхронный метод поиска контакта по ID, принимающий
        /// int параметр id контакта и возвращающий экземпляр
        /// контакта. Поиск осуществляется с помощью метода
        /// FirstOrDefaultAsync.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Contact> FindContactById(int id)
        {
            return await context.Contacts.FirstOrDefaultAsync(x=> x.ID == id);
        }

        /// <summary>
        /// Метод, который служит для передачи принимаемых
        /// параметров контакта с целью дальнейшего изменения
        /// данного контакта
        /// </summary>
        /// <param name="name"></param>
        /// <param name="surname"></param>
        /// <param name="fatherName"></param>
        /// <param name="residenceAdress"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        public void ChangeContact(string name, string surname,
            string fatherName, string telephoneNumber, string residenceAdress, string description, int id)
        {
            
        }
    }
}
