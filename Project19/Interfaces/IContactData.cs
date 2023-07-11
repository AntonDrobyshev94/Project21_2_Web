using Project19.Entitys;
namespace Project19.Interfaces
{
    public interface IContactData
    {
        IEnumerable<Contact> GetContacts();
        void AddContacts(Contact contact);
        //void DeleteContact(Contact contact);
        void DeleteContact(int id);
        Task<Contact> FindContactById(int id);
        void ChangeContact(string name, string surname,
            string fatherName, string telephoneNumber, string residenceAdress, string description, int id);
    }
}
